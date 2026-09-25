namespace GRPP.API.Features.GRPPCommands;

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using EasyTmp;
using Exiled.Permissions.Extensions;
using Extensions;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class FreezePlayerMovement : ICommand, IUsageProvider
{
    public string Command => "freeze";
    public string[] Aliases => ["freezeplayer"];
    public string Description => "Freezes a player.";
    public string[] Usage => ["player 0/1/off/on"];
    private static readonly string CommandUsage = EasyArgs.Build().CmdArguments("freeze player 0/1/off/on").Done();
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (arguments.Count <= 1)
            return ResponseDefaults.InvalidOperationResponses.InvalidArgumentCount(CommandUsage, validArgumentCount:1, out response);
        
        if (!sender.CheckPermission("grpp.freezeplayer"))
            return ResponseDefaults.PermissionResponses.NoPermission("grpp.freezeplayer", out response);
        
        if (!arguments.At(0).GetPlayer(out var player))
        {
            response = EasyArgs.Build().Red($"Error! \"{arguments.At(0)}\" is not a player!").Done();
            return false;
        }
        
        if (arguments.Count >= 2 && byte.TryParse(arguments.At(1), out var state) && state <= 1)
        {
            response = EasyArgs.Build()
                .Green("Success!")
                .Space().Orange("Toggled player freeze state.")
                .Done();
            player.ToggleFrozenPlayer(state == 1);
            return true;
        }
        
        if (arguments.Count >= 2 && bool.TryParse(arguments.At(1), out var stateBool))
        {
            response = EasyArgs.Build()
                .Green("Success!")
                .Space().Orange("Toggled player freeze state.")
                .Done();
            player.ToggleFrozenPlayer(stateBool);
            return true;
        }
        
        if (arguments.Count >= 2 && arguments.At(1).ToLower() is "off")
        {
            response = EasyArgs.Build()
                .Green("Success!")
                .Space().Orange("Toggled freeze state")
                .Space().Red("off").Orange(".")
                .Done();
            player.ToggleFrozenPlayer(false);
            return true;
        }
        if (arguments.Count >= 2 && arguments.At(1).ToLower() is "on")
        {
            response = EasyArgs.Build()
                .Green("Success!")
                .Space().Orange("Toggled freeze state")
                .Space().Green("on").Orange(".")
                .Done();
            player.ToggleFrozenPlayer(true);
            return true;
        }
        
        player.ToggleFrozenPlayer();
        response = EasyArgs.Build().Green("Success!").Done();
        return true;
    }
}