namespace Site12.API.Features.Other;

using System;
using Attributes;
using CommandSystem;
using Exiled.API.Features;
using Extensions;

public static class TeslaGate12
{
    public static bool IsEnabled = true;

    [OnPluginEnabled]
    public static void InitEvents()
    {
        Exiled.Events.Handlers.Player.TriggeringTesla += TeslaGate12.TeslaGates; 
        ServerHandlers.WaitingForPlayers += WaitingForPlayers;
    }

    private static void TeslaGates(Exiled.Events.EventArgs.Player.TriggeringTeslaEventArgs ev)
    {
        if (!IsEnabled)
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

        response = "<color=red>Tesla Gates are already</color> <color=green>Enabled</color>";
        if (TeslaGate12.IsEnabled)
            return false;

        TeslaGate12.IsEnabled = !TeslaGate12.IsEnabled;
        response = "<color=green>Tesla Gates are now Enabled</color>";
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

        response = "<color=green>Tesla Gates are already</color> <color=red>Disabled</color>";
        if (!TeslaGate12.IsEnabled)
            return false;
        TeslaGate12.IsEnabled = !TeslaGate12.IsEnabled;
        response = "<color=green>Tesla Gates are now</color> <color=red>Disabled</color>";
        return true;
    }
}