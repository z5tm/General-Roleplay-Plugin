// NOTE: THIS IS COMPLETELY BROKEN, AND IT CAUSESS TWO PLAYERS INVENTORIES TO SYNC. I mean, it could work like that actuallly - wait... IF I ADD A RESET I WILL TRY THIS. THANK YOU.

namespace GRPP.API.Features.GRPPCommands;

using System;
using CommandSystem;
using EasyTmp;
using Extensions;
using PlayerRoles;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CopyPlayer : ICommand
{
    // public string Command => "copyplayer";
    public string Command => "experimental_copyplayer";
    // public string[] Aliases => ["playercopy", "copy"];
    public string[] Aliases => [""];
    public string Description => "[EXPERIMENTAL. PLEASE DO NOT COUNT ON FOR ANY FUNCTIONALITY..]";

    private string _usage = EasyArgs.Build().CmdArguments("copy id").Done();

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;

        ExPlayer player;

        if (arguments.Count != 1)
        {
            response = $"<color=orange>This command requires one argument.\nUsage: {_usage}`";
            return false;
        }
        var arg0 = arguments.At(0);

        if (!ExPlayer.TryGet(arg0, out player))
        {
            response = $"<color=red>Could not find player with ID</color> '<color=blue>{arg0}</color>'.";
            return false;
        }

        // var rankName = ExPlayer.Get(arg0)?.RankName;
        var customInfo = ExPlayer.Get(arg0)?.CustomInfo;
        var nickname = ExPlayer.Get((arg0))?.Nickname;
        // var inventory = ExPlayer.Get(arg0)?.Inventory;
        var inv = ExPlayer.Get(arg0).Inventory.UserInventory.Items;
        // var badgeHidden = ExPlayer.Get(arg0).BadgeHidden;
        var emotion = ExPlayer.Get(arg0).Emotion;
        var scale = ExPlayer.Get(arg0).Scale;
        // var cuffed = ExPlayer.Get(arg0).Cuffer; // okay so "cuffer" == the person who is cuffing the person. SO weird and interesting :D
        var cuffed = ExPlayer.Get(arg0).IsCuffed;
        var role = ExPlayer.Get(arg0).Role.Type;

        ExPlayer.Get(arg0).BadgeHidden = true;
        
        player = ExPlayer.Get(sender);

        player.Role?.Set(role, RoleSpawnFlags.None);
        player.Inventory.UserInventory.Items = inv;
        // player.ClearInventory();
        player.CustomInfo = customInfo;
        player.DisplayNickname = nickname;
        // player.RankName = rankName;
        player.Emotion = emotion;
        player.BadgeHidden = true;
        player.Scale = scale;
        // player.Cuffer = cuffed;
        if (cuffed) player.Handcuff(); else player.RemoveHandcuffs();
        response = "Done.";
        return true;
    }
}
// [OnPluginEnabled]
    // private static void InitEvents()
    // {
    //     ServerHandlers.WaitingForPlayers += WaitingForPlayers;
    // }
    //
    //
    // private static void WaitingForPlayers()
    // {
    //     ToggledPlayers.Clear();
    // }
