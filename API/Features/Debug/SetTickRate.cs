namespace GRPP.API.Features.Debug;

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using EasyTmp;
using Exiled.Permissions.Extensions;
using UnityEngine;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SetTickRate : ICommand
{
    public string Command => "tickrate";
    public string[] Aliases => [];
    public string Description => "Sets the server tickrate!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var player = ExPlayer.Get(sender);
        if (player == null)
            return ResponseDefaults.ConsoleResponses.CannotBeRunByConsole(out response);
        
        if (!player.CheckPermission("grpp.tickrate") || (player.CheckPermission("allperms") && !player.CheckPermission(PlayerPermissions.ServerConsoleCommands)))
            return ResponseDefaults.PermissionResponses.NoPermission("grpp.tickrate (or the RA perm for server-console-commands)", out response);
        
        if (arguments.Count > 2)
            return ResponseDefaults.InvalidOperationResponses.InvalidArgumentCount(1, out response);
        
        if (!int.TryParse(arguments.At(0), out var tickRate))
        {
            response = EasyArgs.Build().Red("Error!")
                .Space().Orange("TickRate must be an integer!")
                .Space().Orange("(")
                .Space().Blue(int.MinValue.ToString())
                .Space().Orange("to")
                .Space().Blue(int.MaxValue.ToString())
                .Space().Orange(")")
                .Done();
            return false;
        }
        
        if (arguments.Count > 1 && int.TryParse(arguments.At(1), out var updateRate))
            Time.fixedDeltaTime = 1.0f / updateRate;
        
        Application.targetFrameRate = tickRate;
        // do something with these eventually !! these exist
        // Application.OpenURL();
        // Application.Unload();
        
        // UPDATE, at a later date - these probably won't be used in the actual GRPP, but I'll definitely look into these for other, more debug-based plugins ^^
        response = EasyArgs.Build().Green("Success!").Space().Orange($"Set the tickrate to {arguments.At(0)}!").Done();
        return true;
    }
}