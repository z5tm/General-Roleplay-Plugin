namespace GRPP.API.Features.GRPPCommands;

using System;
using CommandSystem;
using EasyTmp;
using Exiled.Permissions.Extensions;
using Lobby;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class TaserMod : ICommand
{
    public string Command => "tasermod";

    public string[] Aliases => ["taser", "changetasers", "modtaser"];

    public string Description => "Modifies how tasers behave. Usage: `taser 1`/`taser 0` (basic for test, 1 = cardiac on, 0 = cardiac off)";
    public static bool TaserCardiac { get; set; }

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
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
        if (!sender.CheckPermission("grpp.taser"))
        {
            response = "<color=orange>You do not have the</color> <color=blue>grpp.taser</color> <color=orange>permission. This command has been ignored.</color>";
            return false;
        }

        if (arguments.Count > 0 && (arguments.At(0) == "0" || arguments.At(0) == "off"))
                TaserCardiac = false;
        if (arguments.Count > 0 && (arguments.At(0) == "1" || arguments.At(0) == "on"))
                TaserCardiac = true;
        response = $"<color=orange>Taser cardiac is currently: </color><color=blue>{TaserCardiac}</color>";
        return true;
    }
}