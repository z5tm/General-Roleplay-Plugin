namespace GRPP.API.Features.GRPPCommands;

using System;
using CommandSystem;
using EasyTmp;
using Exiled.Permissions.Extensions;
using GRPP.API.Attributes;
using GRPP.Extensions;
using Lobby;

public static class TeslaGate12
{
    public static bool IsEnabled = true;

    [OnPluginEnabled]
    public static void InitEvents()
    {
        Exiled.Events.Handlers.Player.TriggeringTesla += TeslaGates; 
        ServerHandlers.WaitingForPlayers += WaitingForPlayers;
    }

    private static void TeslaGates(Exiled.Events.EventArgs.Player.TriggeringTeslaEventArgs ev)
    {
        if (!IsEnabled) // if is enabled is disabled, disable tesla gates
        {
            ev.DisableTesla = true;
            ev.IsTriggerable = false;
            ev.IsInHurtingRange = false;
            ev.IsInIdleRange = false;
        }
    }

    private static void WaitingForPlayers() => IsEnabled = true;
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class TeslaGateEnable : ICommand
{
    public string Command => "TeslaGateOn";
    public string[] Aliases => ["EnableTeslaGate", "TeslaGateEnable"];
    public string Description => "Enables the Tesla Gates";

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

        response = "<color=orange>Tesla gates are already</color> <color=green>enabled</color><color=orange>.</color>";
        if (TeslaGate12.IsEnabled)
            return false;
        
        TeslaGate12.IsEnabled = !TeslaGate12.IsEnabled;
        response = "<color=orange>Tesla gates are now </color><color=green>enabled</color><color=orange>.</color>";
        return true;
    }
}
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class TeslaGateDisable : ICommand
{
    public string Command => "TeslaGateOff";
    public string[] Aliases => ["DisableTeslaGate", "TeslaGateDisable"];
    public string Description => "Disables Tesla Gates";

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

        response = "<color=orange>Tesla gates are already</color> <color=red>disabled</color><color=orange>.</color>";
        if (!TeslaGate12.IsEnabled)
            return false;
        TeslaGate12.IsEnabled = !TeslaGate12.IsEnabled;
        response = "<color=orange>Tesla gates are now</color> <color=red>disabled</color><color=orange>.</color>";
        return true;
    }
}