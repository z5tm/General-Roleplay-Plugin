// namespace GRPP.API.Features.GRPPCommands;
//
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using CommandSystem;
// using Exiled.Events.EventArgs.Player;
// using GRPP.API.Attributes;
// using GRPP.Extensions;
// using VoiceChat;
// using VoiceChat.Networking;
//
// [CommandHandler(typeof(RemoteAdminCommandHandler))]
// public class GhostGaming
// {
//     public static List<string> ToggledPlayers;
//     private static void VoiceChatting(VoiceChattingEventArgs ev)
//     {
//         VoiceMessage msgCopy = ev.VoiceMessage;
//
//         if (ToggledPlayers.Contains(ev.Player) && !ev.Player.IsScp)
//         {
//             foreach (ExPlayer player in ExPlayer.List.Where(p => p != ev.Player && (p.IsScp || ToggledPlayers.Contains(p))))
//             {
//                 msgCopy.Channel = VoiceChatChannel.ScpChat; // 1507/1576/smt chat for ghosts
//                 player.ReferenceHub.connectionToClient.Send(msgCopy);
//             }
//
//             ev.IsAllowed = false;
//             return;
//         }
//
//         if (msgCopy.Channel == VoiceChatChannel.ScpChat)
//             foreach (ExPlayer player in ToggledPlayers)
//             {
//                 msgCopy.Channel = VoiceChatChannel.Intercom;
//                 player.ReferenceHub.connectionToClient.Send(msgCopy);
//             }
//     }
//
//     private static void WaitingForPlayers()
//     {
//         ToggledPlayers.Clear();
//     }
// }