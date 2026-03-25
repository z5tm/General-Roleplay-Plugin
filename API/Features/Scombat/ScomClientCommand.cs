namespace GRPP.API.Features.Scombat;

using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.Permissions.Extensions;
using Extensions;
using GRPPCommands;
using PlayerRoles;

[CommandHandler(typeof(ClientCommandHandler))]
public class ScomClientCommand : ICommand
{
    public string Command => "scom";

    public string[] Aliases => ["signalcommunicate"];

    public string Description => "Allows you to use your PDA (RP) to message others";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = ExPlayer.Get((CommandSender)sender);

        if (sender.CheckPermission("grpp.scomban"))
        {
            response = "<color=orange>You have been</color> <color=red>banned</color> <color=orange>from using the</color> <color=blue>SCOM</color> <color=orange>system.</color>";
            return false;
        }

        response = "<color=orange>> You must select someone to</color> <color=blue>SCOM" +
                   "\nSCOM</color> <color=orange>{</color><color=blue>ID</color><color=orange>}</color> <color=orange>{</color><color=blue>MESSAGE</color><color=orange>} (Sends a </color><color=blue>message</color> <color=orange>to a</color> <color=blue>user</color><color=orange>.)" +
                   "\n<color=blue>SCOM</color> <color=orange>{</color><color=blue>LIST</color><color=orange>} (Shows a</color> <color=blue>list</color> <color=orange>of everyone registered to</color> <color=blue>SCOM</color><color=orange>)</color>" +
                   "\n<color=blue>SCOM</color> <color=orange>{</color><color=blue>TIME</color><color=orange>} (Shows the current</color> <color=blue>time in <color=blue>roleplay)" +
                   "\n<color=blue>SCOM TOGGLE</color><color=orange> (</color><color=yellow>Toggles</color> <color=orange>your ability to receive</color> <color=blue>SCOM</color><color=orange>s.)</color>";
        if (arguments.Count == 0)
            return false;

        switch (arguments.At(0).ToLower())
        {
            case "list":
                response = "<color=red>mmm...";
                if (player.ScomPlayer().CurrentRole.RoleEntry == null)
                    return false;
                if (!player.ScomPlayer().CurrentRole.Rank.HasPda)
                    return false;
                foreach (var ply in ExPlayer.List)
                {
                    if (ply.ScomPlayer().CurrentRole.RoleEntry == null)
                        continue;
                    if (ply.ScomPlayer().CurrentRole.Rank == null)
                        continue;
                    if (!ply.ScomPlayer().ScomEnabled)
                        continue;
                    if (!ply.ScomPlayer().CurrentRole.Rank.HasPda)
                        continue;
                    if (ply.Role.Type == RoleTypeId.Scp079 || ply.IsCHI)
                        continue;
                    player.SendConsoleMessage($"> {ply.CustomName} - {ply.Id}", "green");
                }

                response = "<color=green>> Completed";
                return true;
            case "toggle":
                response = "<color=red>mmm...";
                if (player.ScomPlayer().CurrentRole.RoleEntry == null)
                    return false;
                if (!player.ScomPlayer().CurrentRole.Rank.HasPda)
                    return false;

                player.ScomPlayer().ScomEnabled = !player.ScomPlayer().ScomEnabled;
                response = $"<color=green>> Scom is now toggled ({player.ScomPlayer().ScomEnabled})";
                return true;

            case "time":
                response = "<color=red>mmm...";
                if (player.ScomPlayer().CurrentRole.RoleEntry == null)
                    return false;
                if (!player.ScomPlayer().CurrentRole.Rank.HasPda)
                    return false;

                response = $"<color=green>> Current Time: {RoleplayTime.GetFormattedTime()}";
                return true;
        }

        response = "<color=red>> A Message is required...";
        if (arguments.Count < 2)
            return false;

        if (arguments.Skip(1).Any(arg => Plugin.Singleton.Config.Blocklist.Contains(arg)))
        {
            player.Ban(1577000000, "Automated ban for Rule 3. Appeal on the discord if you believe this was false.\nhttps://discord.gg/site27");
            response = "Really?";
            return false;
        }

        if (arguments.At(0) == "*")
        {
            foreach (var ply in ExPlayer.List)
            {
                if (ply.ScomPlayer().CurrentRole.RoleEntry == null)
                    continue;
                if (!ply.ScomPlayer().ScomEnabled)
                    continue;
                if (!ply.ScomPlayer().CurrentRole.Rank.HasPda)
                    continue;
                if (ply.Role.Type == RoleTypeId.Scp079 || ply.IsCHI)
                    continue;
                player.SendScomMessage(ply, string.Join(" ", arguments.ToArray()));
            }

            response = "<color=green>> Completed";
            return true;
        }

        if (!ExPlayer.TryGet(arguments.At(0), out var receiver))
            return false;

        // Get the arguments after the first one
        var additionalArguments = new ArraySegment<string>(arguments.Array!, arguments.Offset + 1, arguments.Count - 1);

        // Join the additional arguments into a single string if needed
        var additionalArgumentsString = string.Join(" ", additionalArguments);
        player.SendScomMessage(receiver, additionalArgumentsString);
        response = "<color=green>> Completed";
		return true;
	}
}