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

    private readonly string _arguments = EasyArgs.Build().CmdArguments("hint playerid seconds hint").Done();
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;
        
        if (arguments.Count <= 2 || arguments.At(0) == null)
        { 
            //{Clr.Blue}Arguments{Clr.End}{Clr.Orange}:{Clr.End}
            // {Clr.Blue}hint{Clr.End} {Clr.Orange}[{Clr.End}{Clr.Blue}playerid{Clr.End}<color=orange>]</color> <color=orange>[</color><color=blue>seconds</color><color=orange>]</color> <color=orange>[</color><color=blue>hint</color><color=orange>]</color>
            // response = Clr.Build()
            //     .Blue("Arguments")
            //     .Orange(":")
            //     .NewLine()
            //     .Parameter("hint")
            //     .Space()
            //     .Parameter("playerid")
            //     .Space()
            //     .Parameter("seconds")
            //     .Space()
            //     .Parameter("hint")
            //     .Done();
            // AHHAHA I HAVE BUILT THE ULTIMATE WEAPON. PARAMETERS! !!!!!!!!!!!
            // response = Clr.Build().Orange("Arguments:").NewLine()
            
            // PFF ANOTHER ITERATION. EZZZPZZZZ.
            // response = EasyArgs.Build().CmdArguments("hint playerid seconds hint").Done(); // so i just built this - should be the BEST, EASIEST version. i am SO happy with myself. - also renamed to EasyArgs, cause it's not just a color thing anymore
            response = _arguments; // okay put it there
            return false;
        }
        if (!ExPlayer.TryGet(arguments.At(0), out var player))
        {
            response = $"<color=orange>Could</color> <color=red>not</color> <color=orange>find</color> <color=blue>player</color> <color=orange>'</color><color=blue>{arguments.At(0)}</color><color=orange>'</color><color=orange>.</color>";
            return false;
        } // will add "*" functionality soon, with a simple foreach

        if (!int.TryParse(arguments.At(1), NumberStyles.Number, CultureInfo.CurrentCulture, out var timeToShow))
            return false;
        
        player.ShowHint(string.Join(" ", arguments.Skip(2)), timeToShow);
        response = $"<color=blue>Hint</color> <color=green>successfully</color> <color=orange>sent to</color> <color=blue>{ExPlayer.Get(arguments.At(0)).Nickname}</color><color=orange>/</color><color=blue>{ExPlayer.Get(arguments.At(0)).CustomName}";

        response = "Command failed in some way.";
        return true;
    }
}
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class ConsoleHintSendTest : ICommand
{
    public string Command => "hint6";
    public string[] Aliases => [];
    public string Description => "A simple wrapper for hints.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Log.Debug(EasyArgs.Build().CmdArguments("hint playerid seconds hint").Done());
        Logger.Debug($"{string.Join(" ", arguments.Skip(1))}");
        response = "Command succeeded in some way.";
        return true;
    }
}