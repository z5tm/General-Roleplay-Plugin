//
//
//  NOTE: THIS IS COMPLETELY BROKEN, AND IT CAUSESS TWO PLAYERS INVENTORIES TO SYNC. I mean, it could work like that actuallly - wait... IF I ADD A RESET I WILL TRY THIS. THANK YOU.
//
//

// namespace GRPP.API.Features.GRPPCommands;
//
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using CommandSystem;
// using Exiled.Events.EventArgs.Player;
// using GRPP.API.Attributes;
// using GRPP.Extensions;
// using LabApi.Events.Arguments.PlayerEvents;
// using PlayerRoles;
// using VoiceChat;
// using VoiceChat.Networking;
//
// [CommandHandler(typeof(RemoteAdminCommandHandler))]
// public class CopyPlayer : ICommand
// {
//     public string Command => "copyplayer";
//     public string[] Aliases => ["playercopy", "copy"];
//     public string Description => "Toggles the input player's ability to use SCP chat chat.";
//     
//     public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
//     {
//         if (!sender.CheckRemoteAdmin(out response))
//             return false;
//
//         ExPlayer player;
//
//         if (arguments.Count == 0)
//         {
//             response = "<color=orange>This command requires one argument.\nUsage: `<color=blue>copy [id]</color>`";
//             return false;
//         }
//
//         if (arguments.Count != 1)
//         {
//             response = "<color=orange>This command requires one argument.\nUsage: `<color=blue>copy [id]</color>`";
//             return false;
//         }
//
//         if (!ExPlayer.TryGet(arguments.At(0), out player))
//         {
//             response = $"<color=red>Could not find player with ID</color> '<color=blue>{arguments.At(0)}</color>'.";
//             return false;
//         }
//
//         var rankName = ExPlayer.Get(arguments.At(0))?.RankName;
//         var customInfo = ExPlayer.Get(arguments.At(0))?.CustomInfo;
//         var nickname = ExPlayer.Get((arguments.At(0)))?.Nickname;
//         var inventory = ExPlayer.Get(arguments.At(0))?.Inventory;
//         var badgeHidden = ExPlayer.Get(arguments.At(0)).BadgeHidden;
//         var emotion = ExPlayer.Get(arguments.At(0)).Emotion;
//         var scale = ExPlayer.Get(arguments.At(0)).Scale;
//         var cuffed = ExPlayer.Get(arguments.At(0)).Cuffer;
//         var role = ExPlayer.Get(arguments.At(0)).Role.Type;
//         
//         player = ExPlayer.Get(sender);
//         player.Role?.Set(role, RoleSpawnFlags.None);
//         // player.ClearInventory();
//         player.CustomInfo = customInfo;
//         player.DisplayNickname = nickname;
//         // player.RankName = rankName;
//         player.Emotion = emotion;
//         // player.BadgeHidden = badgeHidden;
//         player.Scale = scale;
//         player.Cuffer = cuffed;
//         response = "Done.";
//         return true;
//     }
//
//     [OnPluginEnabled]
//     private static void InitEvents()
//     {
//         ServerHandlers.WaitingForPlayers += WaitingForPlayers;
//     }
//     
//
//     private static void WaitingForPlayers()
//     {
//         // ToggledPlayers.Clear();
//     }
// }