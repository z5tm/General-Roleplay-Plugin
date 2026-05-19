namespace GRPP.API.Features.GRPPCommands;

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using GRPP.API.Attributes;
using GRPP.Extensions;
using JetBrains.Annotations;
using UnityEngine;

public abstract class Height
{
    public static bool IsEnabled;
    
    [UsedImplicitly]
    [OnPluginEnabled]
    public static void InitEvents() => ServerHandlers.WaitingForPlayers += WaitingForPlayers;

    private static void WaitingForPlayers() => IsEnabled = false;
}

[UsedImplicitly]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class HeightEnable : ICommand
{
    public string Command => "HeightOn";
    public string[] Aliases => ["EnableHeight", "OnHeight", "HeightEnable", "h1"];
    public string Description => "Enables the client command .height.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;

        response = "<color=red>Height Client Command is already</color> <color=green>Enabled</color>";
        if (Height.IsEnabled)
            return false;

        Height.IsEnabled = !Height.IsEnabled;

        response = "<color=green>Height Client Command is now Enabled</color>";
        return true;
    }
}

[UsedImplicitly]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class HeightDisable : ICommand
{
    public string Command => "HeightOff";
    public string[] Aliases => ["DisableHeight", "OffHeight", "HeightDisable", "h0"];
    public string Description => "Disables the client command .height.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;

        response = "<color=green>Height Client Command is already</color> <color=red>Disabled</color>";
        if (!Height.IsEnabled)
            return false;

        Height.IsEnabled = !Height.IsEnabled;

        response = "<color=green>Height Client Command is now</color> <color=red>Disabled</color>";
        return true;
    }
}

[UsedImplicitly]
[CommandHandler(typeof(ClientCommandHandler))]
public class HeightClient : ICommand
{
    public string Command => "Height";
    public string[] Aliases => ["setheight", "size"];
    public string Description => "Sets a custom height to enhance RP.";


    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!Height.IsEnabled)
        {
            response = "This feature is currently disabled!";
            return false;
        }

        var player = ExPlayer.Get((CommandSender)sender);

        if (arguments.Count == 0)
        {
            player.Scale = Vector3.one;
            response = "Height reset successfully!";
            return false;
        }

        float valueCm;
        if (arguments.At(0).Contains("'"))
        {
            var parts = arguments.At(0).Split('\'');
            if (!float.TryParse(parts[0], out var feet))
            {
                response = "Invalid height formatting for feet";
                return false;
            }
            if (!float.TryParse(parts[1], out var inches))
            {
                response = "Invalid height formatting for inches";
                return false;
            }
            valueCm = (feet * 30.48f) + (inches * 2.54f);
        }
        else if (!float.TryParse(arguments.At(0), out valueCm))
        {
            response = "Invalid height formatting for centimetres";
            return false;
        }
        var final = valueCm / 183f;
        final = Mathf.Clamp(final, 
            min:Plugin.Singleton.Config.MinHeight ?? Defaults.MinHeight, 
            max:Plugin.Singleton.Config.MaxHeight ?? Defaults.MaxHeight);

        response = "You are an SCP!"; if (player.IsScp) return false;
        
        player.Scale = Vector3.one * final;
        response = $"Height changed successfully to {Mathf.RoundToInt(final * 183f)}cm";
        return true;
    }
}
