namespace Site12.API.Features.Other;

using System;
using Attributes;
using CommandSystem;
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
            ev.IsAllowed = false;
            ev.DisableTesla = true;
            ev.IsTriggerable = false;
        }
    }

    private static void WaitingForPlayers() => IsEnabled = true;
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class TeslaGateToggle : ICommand
{
    public string Command => "TeslaGateToggle";
    public string[] Aliases => ["ToggleTeslaGate", "ToggleTeslas", "TeslasToggle", "ToggleTesla", "TeslaToggle"];
    public string Description => "Toggles the Tesla Gates";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;

        if (TeslaGate12.IsEnabled)
        {
            response = "<color=red>Tesla Gates are now Disabled</color>";
        }

        if (!TeslaGate12.IsEnabled)
        {
            response = "<color=green>Tesla Gates are now Enabled</color>";
        }
        
        TeslaGate12.IsEnabled = !TeslaGate12.IsEnabled;
        return true;
}