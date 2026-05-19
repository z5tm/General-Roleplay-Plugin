namespace GRPP.API.Features.GRPPCommands;

using System;
using Attributes;
using CommandSystem;
using EasyTmp;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp914;
using Exiled.Permissions.Extensions;
using GRPP.Extensions;
using LabApi.Features.Wrappers;
using Lobby;
using MEC;
using UnityEngine;
using Door = Exiled.API.Features.Doors.Door;

public static class LockDownBase
{
    public static bool IsEnabled = true;

    [OnPluginEnabled]
    public static void InitEvents()
    {
        PlayerHandlers.InteractingDoor += DoorInteracted;
    }

    public static void DoorInteracted(InteractingDoorEventArgs ev)
    {
        if (IsEnabled)
        {
            if (!ev.Door.IsLocked)
            {
                ev.Door.Lock(600, DoorLockType.AdminCommand);
            }
            else
            {
                ev.Door.Unlock(600, DoorLockType.AdminCommand);
            }

            IsEnabled = false;
        }
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class Lockdoors : ICommand
{
    public string Command => "lockthemdoors";

    public string[] Aliases => ["lockdoors"];

    public string Description => "locktype";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;

        if (Main.RestrictPermissions && (!sender.CheckPermission("grpp.bypassrestrict") || !Main.MainHosters.Contains(ExPlayer.Get(sender).UserId)))
        {
            response = EasyArgs.Build()
                .Blue("Restrictive permissions")
                .Space().Orange("mode is currently")
                .Space().Green("enabled").Orange(". You also do not have the ")
                .Space().Blue("\"grpp.bypassrestrict\"")
                .Space().Orange("permission, nor are you the").Space().Blue("main hoster").Space()
                .Orange("of the roleplay. \nThis command has been")
                .Space().Red("ignored").Orange(".").Done();
            return false;
        }

        if (arguments.Count == 0)
        {
            response = "<color=orange>Toggled lock on a random door. Don't abuse my commands ever again.</color>\n<color=orange>(</color><color=red>KIDDING</color><color=orange>.).\nUsage:\n</color><color=blue>lockdoors u</color><color=orange> (</color><color=green>Unlocks</color> <color=blue>all doors</color><color=orange>.)</color>\n<color=blue>lockdoors bulk</color><color=orange> (</color><color=red>Locks</color> <color=blue>all bulkhead doors</color><color=orange>!)</color>\n<color=blue>lockdoors bc</color><color=orange> (</color><color=red>Locks</color> <color=orange>AND</color> <color=red>closes</color> <color=blue>all bulkhead doors</color><color=orange>!)</color>\n<color=blue>lockdoors c</color><color=orange> (</color><color=yellow>Toggles</color> <color=orange>the</color> <color=red>lock</color> <color=orange>on the nearest door.</color>";
            return false;
        }
        if ((arguments.At(0) == "reset" || arguments.At(0) == "unlock" || arguments.At(0) == "u"))
        {
            foreach (var bzz in Door.List)
            {
                if (bzz.IsLocked)
                {
                    bzz.Unlock();
                    Log.Debug($"Unlocked {bzz.Base.DoorName}");
                }
            }
            response = "<color=orange>All doors have been</color> <color=green>unlocked.</color>";
            return true;
        }
        var player = ExPlayer.Get(sender);
        
        if (arguments.At(0) == "closest" || arguments.At(0) == "close" || arguments.At(0) == "c" || arguments.At(0) == "cl")
        {
            Door.GetClosest(player.Position, out float distance);
            if (distance < 6 && Door.GetClosest(player.Position, out _).IsLocked)
            {
                Door.GetClosest(player.Position, out float dist).Unlock();
                Log.Debug("The distance was lower than 6, and we just ran the unlock command on the player's position, since GetClosest.IsLocked returned false.");
                response = "<color=orange>The closest</color> <color=blue>door</color> <color=orange>has been</color> <color=green>unlocked</color><color=orange>.</color>";
                return true;

            }
            if (distance < 6 && !Door.GetClosest(player.Position, out _).IsLocked)
            {
                Door.GetClosest(player.Position, out float dist).Lock(DoorLockType.AdminCommand);
                Log.Debug("The distance was lower than 6, and we just ran the lock command on the player's position, since GetClosest.IsLocked returned true.");
                response = "<color=orange>The closest</color> <color=blue>door</color> <color=orange>has been</color> <color=red>locked</color><color=orange>.</color>";
                return true;
            }

            response = "<color=orange>Please get next to the door you want to</color> <color=yellow>toggle</color> <color=orange>the lock of. Then, run this command again.\nThis command has</color><color=red> not</color> <color=orange>been run.</color>";
            return false;
        }
        if (arguments.At(0) == "bulk" || arguments.At(0) == "bulkdoors" || arguments.At(0) == "bulkhead" || arguments.At(0) == "b")
        {
            Door.Get(doorType:DoorType.HeavyBulkDoor).Lock(DoorLockType.AdminCommand);
            response = "<color=red>Locked</color> <color=orange>all</color> <color=blue>bulkhead</color> <color=orange>doors.</color>";
            return true;
        }
        if (arguments.At(0) == "bulkc" || arguments.At(0) == "bulkdoorsclose" || arguments.At(0) == "bulkheadcl" || arguments.At(0) == "bc")
        {
            Door.Get(doorType:DoorType.HeavyBulkDoor).Lock(DoorLockType.AdminCommand);
            Door.Get(doorType: DoorType.HeavyBulkDoor).IsOpen = false;
            response = "<color=red>Locked</color> <color=orange>and</color> <color=red>closed</color> <color=orange>all</color> <color=blue>bulkhead</color> <color=orange>doors.</color>";
            return true;
        }


        response = "<color=orange>Toggled lock on a random door. Don't abuse my commands ever again.</color>\n<color=orange>(</color><color=red>KIDDING</color><color=orange>.).\nUsage:\n</color><color=blue>lockdoors u</color><color=orange> (</color><color=green>Unlocks</color> <color=blue>all doors</color><color=orange>.)</color>\n<color=blue>lockdoors bulk</color><color=orange> (</color><color=red>Locks</color> <color=blue>all bulkhead doors</color><color=orange>!)</color>\n<color=blue>lockdoors bc</color><color=orange> (</color><color=red>Locks</color> <color=orange>AND</color> <color=red>closes</color> <color=blue>all bulkhead doors</color><color=orange>!)</color>\n<color=blue>lockdoors c</color><color=orange> (</color><color=yellow>Toggles</color> <color=orange>the</color> <color=red>lock</color> <color=orange>on the nearest door.</color>";
        // if (Plugin.Singleton.Config.Debug)
        //     response = "<color=orange>Toggled lock on a random door. Don't abuse my commands ever again.</color>\n<color=orange>(</color><color=red>KIDDING</color><color=orange>.).\nUsage:\n</color><color=blue>lockdoors u</color><color=orange> (</color><color=green>Unlocks</color> <color=blue>all doors</color><color=orange>.)</color>\n<color=blue>lockdoors bulk</color><color=orange> (</color><color=red>Locks</color> <color=blue>all bulkhead doors</color><color=orange>!)</color>\n<color=blue>lockdoors bc</color><color=orange> (</color><color=red>Locks</color> <color=orange>AND</color> <color=red>closes</color> <color=blue>all bulkhead doors</color><color=orange>!)</color>\n<color=blue>lockdoors c</color><color=orange> (</color><color=yellow>Toggles</color> <color=orange>the</color> <color=red>lock</color> <color=orange>on the nearest door.</color>";
        
        return false;
    }
}