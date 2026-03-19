namespace GRPP.API.Features;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Attributes;
using Christmas.Scp2536;
using CommandSystem;
using Core;
using CustomItems;
using Department;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using Extensions;
using GameCore;
using Items;
using MEC;
using Other;
using PlayerRoles;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using ProjectMER.Features.ToolGun;
using UnityEngine;
using Broadcast = Broadcast;
using Random = System.Random;

public abstract class Lobby
{
    public static bool IsLobby;
    public static bool IsRoleplay;
    public static bool HasRoleplayStarted;
    public static SchematicObject Schematic;

    public static string Site = Plugin.Singleton?.Config?.SiteName;

    [OnPluginEnabled]
    public static void InitEvents()
    {
        ServerHandlers.WaitingForPlayers += WaitingForPlayers;
        PlayerHandlers.Dying += OnLeaving;

        StaticUnityMethods.OnUpdate -= (Action)DeadmanSwitch.OnUpdate;
    }

    private static void WaitingForPlayers()
    {
        PlayerHandlers.Verified -= OnJoined;

        if(Scp2536Controller.Singleton)
            UObject.Destroy(Scp2536Controller.Singleton);
        Scp559Cake.PossibleSpawnpoints?.Clear();
        IsRoleplay = false;
        IsLobby = false;
        HasRoleplayStarted = false;
        RoleplayTime.StopClock();
    }

    private static void OnLeaving(DyingEventArgs ev)
    {
        if (ev.DamageHandler.Type == DamageType.Unknown)
            ev.IsAllowed = false;
    }

    public static void OnJoined(VerifiedEventArgs ev) => Timing.CallDelayed(2f, () => Action(ev.Player));

    public static void Action(ExPlayer player)
    {
        if (!IsLobby) return;

        player?.Role.Set(RoleTypeId.Tutorial, SpawnReason.None, RoleSpawnFlags.All);
        if(Plugin.Singleton.Config.ConfigurationComplete)
            Timing.CallDelayed(Timing.WaitForOneFrame, () => player?.Position = new Vector3(Plugin.Singleton.Config.LobbySpawnLocationX, Plugin.Singleton.Config.LobbySpawnLocationY, Plugin.Singleton.Config.LobbySpawnLocationZ));
        Timing.CallDelayed(0.2f, () => player?.ShowHint("<b>Welcome to the lobby!</b>\n<b>Pick a role in the Server-Specific tab in your Settings!</b>",10f));
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class UseLobbyCommand : ICommand
{
    public string Command => "UseLobby";
    public string[] Aliases => ["LobbyUse", "OpenLobby", "LobbyOpen", "lobopen", "openlob", "lobon", "l1"];
    public string Description => "Open the Lobby so no people can spawn, with teleporting everyone";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if(Lobby.Site.IsEmpty())
            Lobby.Site = new Random().Next(10, 99).ToString();
        var exUser = ExPlayer.Get(sender);
        response = "<color=red>No Permission.</color>";
        if (!sender.CheckPermission("scombat.lobby"))
            return false;

        response = "<color=orange>Lobby has already been used. `relob` or `reuselobby` are the appropriate commands for this scenario.</color>";
        // OHHH i see what it's doing now! it overrides the `response` var each time, then if an if check fails there it just sends the latest response. smart.
        if (Lobby.IsRoleplay)
            return false;

        exUser.Broadcast(5, "REMEMBER TO USE \"BEGINROLEPLAY\"", Broadcast.BroadcastFlags.AdminChat);

        var lobbySchematic = Plugin.Singleton?.Config?.LobbySchematic;
        if(!string.IsNullOrEmpty(lobbySchematic))
            Lobby.Schematic = ObjectSpawner.SpawnSchematic(lobbySchematic, Vector3.zero, Vector3.zero);

        Round.Start();
        Round.IsLocked = true;

        Lobby.IsLobby = true;
        Lobby.IsRoleplay = true;

        Height.IsEnabled = true;
        Name.IsEnabled = true;
        Scp914.IsEnabled = true;

        TeslaGate12.IsEnabled = false;
        SpawnWaves.IsEnabled = false;

        Door.LockAll(999999, ZoneType.Entrance, DoorLockType.AdminCommand);

        foreach (var player in ExPlayer.List) Lobby.Action(player);

        PlayerHandlers.Verified += Lobby.OnJoined;
        response = "<color=green>Lobby is now on";
        return true;
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ReuseLobbyCommand : ICommand
{
    public string Command => "ReuseLobby";
    public string[] Aliases => ["ReopenLobby", "LobbyReopen", "LobbyReuse", "Relob"];
    public string Description => "Reopens the Lobby so no people can spawn, without teleporting everyone";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var exUser = ExPlayer.Get(sender);
        response = "<color=red>No Permission.";
        if (!sender.CheckPermission("scombat.lobby"))
            return false;

        if (!Lobby.IsRoleplay)
            sender.Respond("<color=red>Please use \"UseLobby\" instead of Reusing Lobby, it causes some issues...");

        response = "<color=red>Lobby is already on...";
        if (Lobby.IsLobby)
            return false;

        var lobbySchematic = Plugin.Singleton?.Config?.LobbySchematic;
        if(!string.IsNullOrEmpty(lobbySchematic))
            Lobby.Schematic = ObjectSpawner.SpawnSchematic(Plugin.Singleton.Config.LobbySchematic, Vector3.zero, Vector3.zero);

        Lobby.IsLobby = true;

        Height.IsEnabled = true;
        Name.IsEnabled = true;

        foreach (var player in ExPlayer.List)
            if (player.Role.IsDead)
                Lobby.Action(player);

        response = "<color=green>Lobby is now on";
        return true;
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class StopLobbyCommand : ICommand
{
    public string Command => "StopLobby";
    public string[] Aliases => ["CloseLobby", "EndLobby", "LobbyClose", "LobbyEnd", "loboff", "lobbyoff", "l0"];
    public string Description => "Closes the Lobby so no one can spawn";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        response = "<color=red>No Permission.";
        if (!sender.CheckPermission("scombat.lobby"))
            return false;

        response = "<color=red>Lobby is already off...";
        if (!Lobby.IsLobby)
            return false;

        if (Lobby.Schematic)
        {
            var a = Lobby.Schematic.GetComponent<MapEditorObject>();
            ToolGunHandler.DeleteObject(a);
        }

        Lobby.IsLobby = false;

        Name.IsEnabled = false;
        Height.IsEnabled = false;

        response = "<color=green>Lobby is now off";
        return true;
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class BeginRoleplay : ICommand
{
    public string Command => "StartRoleplay";
    public string[] Aliases => ["BeginRoleplay", "RoleplayStart", "startrp", "rp1"];
    public string Description => $"Usage: {Usage}";
    public string Usage => "`rp1 [restrictperms 1/yes 0/no] [optional sitenumber]`";
    public bool Restrict { get; set; }

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        response = "<color=red>No Permission.</color>";
        if (!sender.CheckPermission(
                "scombat.lobby")) // should probably rename this permission to something, maybe GRPP.* ? or grpp for general roleplay plugin
            return false;

        response = "<color=orange>There is an ongoing roleplay.</color>";
        if (Lobby.HasRoleplayStarted)
            return false;

        // int startTime = 0600;
        // int secondsPerHour = 600;

        // if (arguments.Count == 2 && (!int.TryParse(arguments.At(0), out startTime) || int.TryParse(arguments.At(1), out secondsPerHour) || startTime < 0 || secondsPerHour < 0))
        //     return false;
        //
        // if (arguments.Count == 2)
        // RoleplayTime.StartClock(startTime, secondsPerHour); // in roleplaytime.cs
        // if (arguments.Count > 0 && arguments.At(0) == "z") 
        //     Log.Error("Argument 0 provided.");
        if (arguments.Count > 0 && arguments.At(0) == "1" || arguments.Count > 0 && arguments.At(0) == "yes")
        {
            response = !Plugin.Singleton.Config.RestrictiveMode 
                ? "Restrictive mode enabled. Other users may have less permissions. ((UNIMPLEMENTED))" 
                : "Sorry, but restrictive mode has been disabled via config. Server owners, check your configuration to ensure you use plain true/false in RestrictiveMode.";
            Restrict = false;
        }
        if (arguments.Count > 1)
            Lobby.Site = arguments.At(0); 

        Lobby.HasRoleplayStarted = true;
        foreach (var player in ExPlayer.List)
        {
            var scom = player.ScomPlayer();
            if (scom ==null) continue;
            Timing.RunCoroutine(scom.TrackHours());
        }
        GiveInventories();
        response = "<color=green>Roleplay has begun. All EZ doors have been unlocked.</color>";
        Door.UnlockAll(ZoneType.Entrance);
        return true;
    }

    public static void GiveInventories()
    {
        if (!Lobby.HasRoleplayStarted)
            return;
        foreach (var player in ExPlayer.List)
        {
            player.ClearInventory(); // ClearAmmo is redundant here, ClearInventory already clears ammo & items. ClearItems clears only items, ClearAmmo clears only ammo, but this clears everything
            var scom = player.ScomPlayer();
            if (scom?.CurrentRole?.RoleEntry == null)
                continue;
            if (scom.CurrentRole?.Rank == null)
                continue;
            foreach (var item in scom.CurrentRole.Rank.LoadOut)
            {
                var itemGiver = GetItem(item, out var cost, player);
                if (itemGiver == null)
                    continue;
                Department.Department.DepartmentsData[Department.Department.GetDepartmentByRole(scom.CurrentRole.RoleEntry)].Balance -= cost;
                itemGiver.GiveItem(player);
            }

            Department.Department.UpdateDepartmentData(Department.Department.GetDepartmentByRole(scom.CurrentRole.RoleEntry));
        }
    }

    public static Dictionary<string, float> ItemCosts = new()
    {
        // Custom Items
        {"Taser", 400},
        {"Baton", 25},
        {"Smoke", 100},
        {"Teargas", 100},

        // -- Base Items -- \\

        // Utilities
        {"Radio", 100},
        {"Flashlight", 40},
        {"Lantern", 100},

        // Medical Items
        {"Adrenaline", 50},
        {"Medkit", 30},
        {"Painkillers", 15},

        // Armor -- Security Only
        {"ArmorHeavy", 845},
        {"ArmorCombat", 560},
        {"ArmorLight", 110},

        // Ammunition -- GL + Security Only
        {"Ammo9x19", 5},
        {"Ammo44cal", 60},
        {"Ammo556x45", 30},
        {"Ammo12gauge", 90},

        // Weapons -- GL + Security Only
        {"GunCOM15", 400},
        {"GunCOM18", 800},
        {"GunRevolver", 800},
        {"GunFSP9", 1250},
        {"GunCrossvec", 1650},
        {"GunE11SR", 1800},
        {"GunShotgun", 3482}
    };

    public static ItemGiver GetItem(LoadOutItem item, out float cost, ExPlayer player = null)
    {
        if (!ItemCosts.TryGetValue(item.ItemType, out cost))
            cost = 0;

        if (!Enum.TryParse(item.ItemType, out ItemType itemType))
            return item.ItemType.ToLower() switch
            {
                "taser" => CustomItemsManager.Get<TaserHandler>(),
                "baton" => CustomItemsManager.Get<BatonHandler>(),
                "smoke" => CustomItemsManager.Get<SmokeGrenadeHandler>(),
                "teargas" => CustomItemsManager.Get<TearGasHandler>(),
                _ => null
            };

        var roleName = "SCP";
        if (player != null)
        {
            var scom = player.ScomPlayer();
            roleName = scom?.CurrentRole?.RoleName ?? roleName;
        }

        return item.ItemType.ToLower().StartsWith("keycard") ? KeycardHandler.CreateInstance(itemType, roleName, item.Level.Value, item.Permissions.ConvertAll(x => (KeycardHandler.Levels)Enum.Parse(typeof(KeycardHandler.Levels), x)).ToArray()) : itemType;
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SetSite : ICommand
{
    public string Command => "SetSite";
    public string[] Aliases => ["SiteSet", "unusedpos"];
    public string Description => "Sets the Site of the current Roleplay";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;

        response = "<color=red>You require one or more arguments...";
        if (arguments.Count == 0)
            return false;

        Lobby.Site = arguments.At(0);
        response = $"<color=green>Site's Number has been set to {arguments.At(0)}";
        return true;
    }
}