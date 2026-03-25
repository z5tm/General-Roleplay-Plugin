namespace GRPP.API.Features.GRPPCommands;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;
using GRPP.API.Attributes;
using GRPP.API.Features.CustomItems;
using GRPP.API.Features.Items;
using GRPP.Extensions;

public abstract class Info
{
    public static bool IsEnabled;
    
    [OnPluginEnabled]
    public static void InitEvents() => ServerHandlers.WaitingForPlayers += WaitingForPlayers;

    private static void WaitingForPlayers() => IsEnabled = false;
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class InfoEnable : ICommand
{
    public string Command => "InfoEnable";
    public string[] Aliases => ["InfoOn", "OnInfo", "EnableInfo", "customion", "inf1"];
    public string Description => "Enables the Information command, that allows users to set their customi.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;

        response = "<color=orange>Information Command is already</color> <color=green>Enabled</color>";
        if (Info.IsEnabled)
            return false;

        Info.IsEnabled = !Info.IsEnabled;

        response = "<color=orange>Information Command is now</color> <color=green>Enabled</color>";
        return true;
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class InfoDisable : ICommand
{
    public string Command => "InfoDisable";
    public string[] Aliases => ["InfoOff", "OffInfo", "DisableInfo", "NoInfo", "customioff", "inf0"];
    public string Description => "Disables the Information Command";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;

        response = "<color=orange>Information Command is already</color> <color=red>Disabled</color>";
        if (!Info.IsEnabled)
            return false;

        Info.IsEnabled = !Info.IsEnabled;

        response = "<color=orange>Information Command is now</color> <color=red>Disabled</color>";
        return true;
    }
}


[CommandHandler(typeof(ClientCommandHandler))]
public class InfoClient : ICommand
{
    public string Command => "Info";
    public string[] Aliases => ["Information", "custominfo", "inf"];
    public string Description => "Gives you a custom information tag for RP purposes : Info (customi, use `\n` to make a newline.)";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!Info.IsEnabled)
        {
            response = "This feature is currently disabled!";
            return false;
        }

        var player = ExPlayer.Get((CommandSender)sender);
        if (arguments.Count == 0)
        {
            player.CustomInfo = string.Empty;
            response = "Customi reset successfully!";
            return true;
        }
        foreach (var word in arguments)
        foreach (var _ in Plugin.Singleton.Config.Blocklist.Where(target => word.Equals(target, StringComparison.OrdinalIgnoreCase))) player.Ban(1577000000, "Automated ban. Appeal on the discord if you believe this was false.");

        player.CustomInfo = string.Join(" ", arguments);

        // foreach (var item in player.Items)
        //     if (CustomItemsManager.Get<KeycardHandler>().Container.HasItem(item.Base, out var idCard))
        //     {
        //         // idCard.Name = player.DisplayNickname;
        //         break;
        //     }

        response = "Customi changed successfully!";
        return true;
    }
}
