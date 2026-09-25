namespace GRPP.API.Features.GRPPCommands;

using System;
using System.Globalization;
using System.Linq;
using CommandSystem;
using EasyTmp;
using Exiled.API.Features;
using Extensions;
using Logger = LabApi.Features.Console.Logger;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class HintWrapper : ICommand
{
    public string Command => "ghint";
    public string[] Aliases => ["grpphint", "hint", "showhint"];
    public string Description => "A simple wrapper for hints.";
    
    private static readonly string Arguments = EasyArgs.Build().CmdArguments("hint playerid/all seconds hint").Done();
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;
        
        if (arguments.Count <= 2)
        { 
            response = Arguments;
            return false;
        }
        
        if (!arguments.At(0).GetPlayer(out var player) && arguments.At(0) is not "all" and not "everyone" and not "*")
        {
            response = $"<color=orange>Could</color> <color=red>not</color> <color=orange>find</color> <color=blue>player</color> <color=orange>'</color><color=blue>{arguments.At(0)}</color><color=orange>'</color><color=orange>.</color>";
            return false;
        }
        
        if (!int.TryParse(arguments.At(1), NumberStyles.Number, CultureInfo.CurrentCulture, out var timeToShow))
            return false;
        
        if (player == null)
        {
            foreach (var plr in ExPlayer.List)
            {
                plr?.ClearAllTagsExcept([Defaults.Tagging.ErrorTag, Defaults.Tagging.WarningTag]);
                plr?.RueIMessage(string.Join(" ", arguments.Skip(2)), timeSeconds:timeToShow);
            }
            
            response = EasyArgs.Build().Green("Successfully").Space().Orange("sent your hint to all players!").Done();
            return true;
        }
        
        player.ClearAllTagsExcept([Defaults.Tagging.ErrorTag, Defaults.Tagging.WarningTag]);
        player.RueIMessage(string.Join(" ", arguments.Skip(2)), timeSeconds:timeToShow);
        
        response = $"<color=blue>Hint</color> <color=green>successfully</color> <color=orange>sent to</color> <color=blue>{ExPlayer.Get(arguments.At(0)).Nickname}</color><color=orange>/</color><color=blue>{ExPlayer.Get(arguments.At(0)).CustomName}";
        return true;
    }
}