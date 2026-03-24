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
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using Extensions;
using GRPPCommands;
using Interactables.Interobjects.DoorUtils;
using Items;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using ProjectMER.Features.ToolGun;
using UnityEngine;
using Broadcast = Broadcast;
using Door = Exiled.API.Features.Doors.Door;
using Random = System.Random;
using Round = Exiled.API.Features.Round;

public abstract class Lobby
{
    public static bool IsLobby;
    public static bool IsRoleplay;
    public static bool HasRoleplayStarted;
    public static SchematicObject Schematic;
    public static bool RestrictPermissions { get; set; } // false by default btw


    public static string Site = Plugin.Singleton?.Config?.SiteName;

    [OnPluginEnabled]
    public static void InitEvents()
    {
        ServerHandlers.WaitingForPlayers += WaitingForPlayers;
        PlayerHandlers.Dying += OnLeaving;

        StaticUnityMethods.OnUpdate -= (Action)DeadmanSwitch.OnUpdate;
    }

    private static void WaitingForPlayers() // reset methodd !
    {
        PlayerHandlers.Verified -= OnJoined;

        if(Scp2536Controller.Singleton)
            UObject.Destroy(Scp2536Controller.Singleton);
        Scp559Cake.PossibleSpawnpoints?.Clear();
        IsRoleplay = false;
        IsLobby = false;
        HasRoleplayStarted = false;
        RestrictPermissions = false;
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
        if (!sender.CheckPermission("grpp.lobby"))
        {
            response = "<color=orange>This command has</color> <color=red>failed</color><color=orange>.</color>\n<color=orange>You are missing the <color=blue>grpp.lobby</color><color=orange> permission.</color>";
            return false;
        }

        if (Lobby.IsLobby)
        {
            response =
                "<color=blue>Lobby</color> <color=orange>is already</color> <color=green>enabled</color><color=orange>!\nUse</color> <color=blue>l0</color> <color=orange>or </color><color=blue>endlobby</color><color=orange> to disable lobby!</color>";
            return false;
        }

        if(Lobby.Site.IsEmpty())
            Lobby.Site = new Random().Next(10, 99).ToString();
        var exUser = ExPlayer.Get(sender);
        // response = "<color=red>No Permission.</color>";
        // if (!sender.CheckPermission("scombat.lobby")) // socmbat.lobby > grpp.lobby has bene done btw
            // return false;

        // OHHH i see what it's doing now! it overrides the `response` var each time, then if an if check fails there it just sends the latest response. smart.

        exUser.Broadcast(5, "REMEMBER TO USE \"BEGINROLEPLAY\"", Broadcast.BroadcastFlags.AdminChat);

        if (Lobby.IsRoleplay)
        {
            foreach (var player in ExPlayer.List)
                if (player.Role.IsDead)
                    Lobby.Action(player);
        }
        else
        {
            var lobbySchematic = Plugin.Singleton?.Config?.LobbySchematic;
            if (!string.IsNullOrEmpty(lobbySchematic))
                Lobby.Schematic = ObjectSpawner.SpawnSchematic(lobbySchematic, Vector3.zero, Vector3.zero);
        }

        Round.Start();
        Round.IsLocked = true;

        Lobby.IsLobby = true;
        Lobby.IsRoleplay = true;

        Shiv.IsEnabled = false;
        Mining.IsEnabled = false;

        Height.IsEnabled = true;
        Name.IsEnabled = true;
        Scp914.IsEnabled = true;
        Info.IsEnabled = true;

        TeslaGate12.IsEnabled = false;
        SpawnWaves.IsEnabled = false;

        Door.LockAll(999999, DoorLockType.Lockdown079);
        foreach (var player in ExPlayer.List) Lobby.Action(player);

        PlayerHandlers.Verified += Lobby.OnJoined;
        response = "<color=blue>Lobby</color> <color=orange>is now</color> <color=green>on</color>";
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
        if (!sender.CheckPermission("grpp.lobby"))
        {
            response = "<color=orange>You need the permission:</color> <color=blue>grpp.lobby</color><color=orange> to run this command.</color>";
            return false;
        }
        
        if (!Lobby.IsLobby)
        {
            response = "<color=blue>Lobby</color> <color=orange>is already</color> <color=red>off.</color>";
            return false;
        }

        if (Lobby.Schematic)
        {
            var a = Lobby.Schematic.GetComponent<MapEditorObject>();
            ToolGunHandler.DeleteObject(a);
        }

        Lobby.IsLobby = false;

        Name.IsEnabled = false;
        Height.IsEnabled = false;
        Info.IsEnabled = false;

        response = "<color=blue>Lobby</color> <color=orange>is now</color> <color=red>off.</color>";
        return true;
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class BeginRoleplay : ICommand
{
    public string Command => "StartRoleplay";
    public string[] Aliases => ["BeginRoleplay", "RoleplayStart", "startrp", "rp1"];
    public string Description => $"Usage: \n{Usage}";
    public string Usage => "`rp1 <color=blue>[number]</color><color=orange>(Optional: Site number)</color> <color=blue>[1/yes 0/no]</color><color=orange>(Restrict Permissions of others)</color> `";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        // Errors go first, cuz, like they fail first before checkin anythin so we can THEN check for args and allat !
        if (!sender.CheckPermission("grpp.lobby"))
        {
            // should probably rename this permission to something, maybe GRPP.* ? or grpp for general roleplay plugin
            response = "<color=orange>Sorry, but you do not have the</color> <color=blue>grpp.lobby</color> <color=orange>permission.</color>";
            return false;
        }

        if (Lobby.HasRoleplayStarted)
        {
            response =
                "<color=orange>Sorry, there is an ongoing</color> <color=blue>roleplay.</color>\n<color=orange>Note: There is no reason to run this command twice.</color>";
            return false;
        }

        // int startTime = 0600;
        // int secondsPerHour = 600;

        // if (arguments.Count == 2 && (!int.TryParse(arguments.At(0), out startTime) || int.TryParse(arguments.At(1), out secondsPerHour) || startTime < 0 || secondsPerHour < 0))
        //     return false;
        //
        // if (arguments.Count == 2)
        // RoleplayTime.StartClock(startTime, secondsPerHour); // in roleplaytime.cs
        // if (arguments.Count > 0 && arguments.At(0) == "z") 
        //     Log.Error("Argument 0 provided.");
        //arguments.Count >= 1 && arguments.At(1) == "1" || arguments.Count >= 1 && arguments.At(1) == "yes" && sender.CheckPermission("scombat.restrictpermissions") //  ignored scombat.* here, grpp.* is valid tho
        if (arguments.Count >= 1 && (arguments.At(1) == "1" || arguments.At(1) == "yes") && sender.CheckPermission("grpp.restrictpermissions"))
            if (Plugin.Singleton.Config.RestrictiveMode)
                Lobby.RestrictPermissions = true;

        if (arguments.Count > 0 && uint.TryParse(arguments.At(0), out var r))
            Lobby.Site = r.ToString();

        // noting that hasroleplaystarted used to be here, just in case moving it under this foreach broke anything
        foreach (var player in ExPlayer.List)
        {
            var scom = player.ScomPlayer();
            if (scom ==null) continue;
            // Timing.RunCoroutine(scom.TrackHours()); // -- yeahhh i dunno bout using this one - it IS dnt compliant but just. prolly nah
        }
        Lobby.HasRoleplayStarted = true;
        GiveInventories(); // DEPENDS ON HASROLEPLAYSTARTED
        // response = "<color=green>Roleplay has begun. All EZ doors have been unlocked.</color>";
        Door.LockAll(9999, ZoneType.Entrance); // duration, where
        if (arguments.Count <= 0)
        {
            response = Lobby.RestrictPermissions 
                ? $"<color=green>Roleplay has begun.</color> <color=orange>The site name is</color> <color=blue>{Lobby.Site}</color><color=orange>, and restrictive mode is</color> <color=green>enabled</color><color=orange>. Not sure how, but..</color>" 
                : $"<color=green>Roleplay has begun.</color> <color=orange>The site name is</color> <color=blue>{Lobby.Site}</color><color=orange>, and restrictive mode is</color> <color=red>disabled</color>.\n<color=orange>The server has set the allowance of RestrictiveMode to:</color> <color=blue>{Plugin.Singleton.Config.RestrictiveMode}</color><color=orange>.</color>\n<color=orange>Note: To set RestrictiveMode, use</color> <color=blue>`rp1 sitenumber yes/no`</color> <color=orange>or</color> <color=blue>`rp1 sitenumber 1/0`</color>";
            return true;
        }

        // Note: value = `conditon` ? valueiftrue : valueiffalse

        if (arguments.Count >= 1)
        {
            response = Lobby.RestrictPermissions 
                ? $"<color=green>Roleplay has begun.</color> <color=orange>The site name is</color> <color=blue>{Lobby.Site}</color><color=orange>, and restrictive mode is</color> <color=green>enabled</color>." 
                    : $"<color=green>Roleplay has begun.</color> <color=orange>The site name is</color> <color=blue>{Lobby.Site}</color><color=orange>, and restrictive mode is</color> <color=red>disabled</color>.\n<color=orange>Note that restrictive mode is, by default, disabled. Also, the server has set the allowance of RestrictiveMode to: <color=blue>{Plugin.Singleton.Config.RestrictiveMode}</color><color=orange>.</color>";
            return true;
        }

        // response = arguments.Count switch
        // {
        //     1 => $"gaming {arguments.At(0)}",
        //     0 =>  $"gaming {arguments.At(0)}"
        // }; i have no idea how to use switch statements. i'll try this again later.

        response = $"Some error occured.\n Notable values:\nSite:{Lobby.Site}\nRestrictPermissions:{Lobby.RestrictPermissions}\nRestrictAllowed:{Plugin.Singleton.Config.RestrictiveMode}\n ";
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
    public string[] Aliases => ["SiteSet", "sitenum", "site", $"sitenumber"];
    public string Description => "Sets site number. This affects cards, and more in the future.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;
        if (Lobby.RestrictPermissions && !sender.CheckPermission("grpp.bypassrestrict"))
        {
            response = "<color=orange>Restrictive mode is currently enabled. You also do not have the:</color> <color=blue>grpp.bypassrestrict</color><color=orange> permission.\nThis command has been ignored.</color>";
            return false;
        }
        if (arguments.Count == 0)
        {
            response = $"<color=orange>>The</color> <color=blue>site's number</color> <color=orange>is currently:</color><color=blue>{Lobby.Site}</color><color=orange>.</color>";
            return true;
        }

        Lobby.Site = arguments.At(0);
        response = $"<color=orange>>The</color> <color=blue>site's number</color> <color=orange>has been set to</color> <color=blue>{arguments.At(0)}</color>";
        return true;
    }
}