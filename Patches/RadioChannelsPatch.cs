namespace GRPP.Patches;

using System;
using API.Attributes;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features;
using Mirror;
using VoiceChat;
using VoiceChat.Networking;

// Yes, this is a modified version of my radio channels plugin code in https://github.com/Mrhootyhoot1/RadioChannels. I, hooty, own this repository and put this code here myself.

[HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    public static class RadioChannelsPatch
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            MethodInfo canReceiveMessage = AccessTools.Method(
	            typeof(RadioChannelsPatch), 
	            nameof(RadioChannelsPatch.CanReceiveMessage), 
	            new [] { typeof(NetworkConnection), typeof(ReferenceHub) }
	        );
            
            MethodInfo usingRadioChannel = AccessTools.Method(
                typeof(RadioChannelsPatch), 
                nameof(UsingRadio), 
                new Type[] { typeof(VoiceMessage) }
            );
            
            MethodInfo usingProximityChat = AccessTools.Method(
                typeof(RadioChannelsPatch), 
                nameof(UsingProximityChat), 
                new Type[] { typeof(VoiceMessage) }
            );
            
            FieldInfo voiceMessageChannel = AccessTools.Field(typeof(VoiceMessage), nameof(VoiceMessage.Channel));
            
            Label retLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            Label continueFromRevert = generator.DefineLabel();
            LocalBuilder messageModified = generator.DeclareLocal(typeof(bool));

            CodeMatcher matcher = new CodeMatcher(instructions);
            matcher.Start();
            CodeInstruction ret = matcher.MatchEndForward(new CodeMatch(OpCodes.Ret)).Instruction; 
            ret.labels.Add(retLabel); 
            
            matcher.Start();

            if (Plugin.Singleton.Config.RadioChannelsEnabled)
            {
	            matcher = matcher.MatchEndForward(
                    new CodeMatch(OpCodes.Callvirt),
                    new CodeMatch(OpCodes.Ldobj),
                    new CodeMatch(OpCodes.Starg_S),
                    new CodeMatch(OpCodes.Ldarg_1),
                    new CodeMatch(i =>
                        i.opcode == OpCodes.Ldfld &&
                        ((FieldInfo)i.operand).FieldType == typeof(VoiceChatChannel)), 
                    new CodeMatch(OpCodes.Brfalse_S)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1), //load the voice message to the stack.
                    new CodeInstruction(OpCodes.Call, usingProximityChat), //check if the message is proximity chat.
                    new CodeInstruction(OpCodes.Brtrue_S, continueLabel), //if the message is proximity chat, skip the rest of the inserted code.
                    new CodeInstruction(OpCodes.Ldarg_1), //load the voice message to the stack again.
                    new CodeInstruction(OpCodes.Call, usingRadioChannel), //check if the voice message is using radio channels
                    new CodeInstruction(OpCodes.Brfalse_S, continueLabel), //if it is, jump to the callSendMessage label.
                    
                    new CodeInstruction(OpCodes.Ldarg_0), //load the network connection that sent the message
                    new CodeInstruction(OpCodes.Ldloc_S, 4), //load the current referencehub
                    new CodeInstruction(OpCodes.Call, canReceiveMessage), //check if the player can receive your message 
                    new CodeInstruction(OpCodes.Brtrue, continueLabel), //if they cannot, send a proximity thing
                    
                    new CodeInstruction(OpCodes.Ldarga, 1), //load the address of the voice message
                    new CodeInstruction(OpCodes.Ldc_I4, (int)VoiceChatChannel.Proximity), //load the prox chat voice channel
                    new CodeInstruction(OpCodes.Stfld, voiceMessageChannel), //change the voice channel to prox chat
                    new CodeInstruction(OpCodes.Ldloc, messageModified.LocalIndex), //load the messageModified local
                    new CodeInstruction(OpCodes.Ldc_I4_1), //load 1 (true)
                    new CodeInstruction(OpCodes.Stloc, messageModified.LocalIndex), //save messageModified
                    
                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel) 
                );

	            matcher.Start();
	            matcher.MatchEndForward(
			            new CodeMatch(OpCodes.Ldloca_S),
			            new CodeMatch(OpCodes.Call)
		            )
		            .Insert(
			            new CodeInstruction(OpCodes.Ldloc, messageModified.LocalIndex), //load message modified
			            new CodeInstruction(OpCodes.Brfalse_S, continueFromRevert), //if it's not continue
			            new CodeInstruction(OpCodes.Ldarga, 1), //load the voice message address
			            new CodeInstruction(OpCodes.Ldc_I4_0, (int)VoiceChatChannel.Radio), //change it back to radio 
			            new CodeInstruction(OpCodes.Stfld, voiceMessageChannel), //store it back
			            
			            new CodeInstruction(OpCodes.Nop).WithLabels(continueFromRevert) //continue
		            );
            }
            
            foreach (var instruction in matcher.InstructionEnumeration())
            {
                yield return instruction; 
            }
        }

        static bool CanReceiveMessage(NetworkConnection senderConnection, ReferenceHub recipient)
        {
            Player playerSender = Player.Get(senderConnection);
            Player playerRecipient = Player.Get(recipient);
            
            if (playerRecipient.IsNPC)
            {
                return false;
            }
            
            string senderChannel = RadioChannelHandler.GetPlayerRadioChannel(playerSender);
            string recipientChannel = RadioChannelHandler.GetPlayerRadioChannel(playerRecipient);
            
            if (senderChannel == recipientChannel)
            {
                return true;
            }
            return false;
        }

        static bool UsingRadio(VoiceMessage message)
        {            
            return message.Channel == VoiceChatChannel.Radio;
        }

        static bool UsingProximityChat(VoiceMessage message)
        {
            return message.Channel == VoiceChatChannel.Proximity;
        }
    }

    public static class RadioChannelHandler
    {
        private static Dictionary<Player, string> _playerChannels = new Dictionary<Player, string>();

        [OnPluginEnabled]
        public static void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Verified += OnPlayerJoined;
            Exiled.Events.Handlers.Player.TogglingRadio += OnTogglingRadio;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
        }

        static void OnTogglingRadio(TogglingRadioEventArgs ev)
        {
            string channel = GetPlayerRadioChannel(ev.Player);
            string newChannel = Plugin.Singleton.Config.Channels[(Plugin.Singleton.Config.Channels.IndexOf(channel) + 1) % Plugin.Singleton.Config.Channels.Length];
            
            if (ev.NewState)
            {
                ev.IsAllowed = true;
                SetPlayerRadioChannel(ev.Player, Plugin.Singleton.Config.Channels[0]);
                ev.Player.ShowHint(Plugin.Singleton.Config.Channels[0]);
                return;
            }
            if (newChannel.ToLower() == "off" && Plugin.Singleton.Config.OffChannelEnabled)
            {
                ev.IsAllowed = true;
                ev.Player.ShowHint("<color=red> Radio Off </color>");
                SetPlayerRadioChannel(ev.Player, "off");
                return;
            }
            ev.IsAllowed = false;
            SetPlayerRadioChannel(ev.Player, newChannel);
            ev.Player.ShowHint(newChannel);
        }
           
        static void OnPlayerJoined(VerifiedEventArgs ev)
        {
            _playerChannels.Add(ev.Player, Plugin.Singleton.Config.Channels[0]);
        }

        static void OnPlayerLeft(LeftEventArgs ev)
        {
            if (_playerChannels.ContainsKey(ev.Player))
            {
                _playerChannels.Remove(ev.Player);
            }
        }
        
        public static string GetPlayerRadioChannel(Player player)
        {
            if (!_playerChannels.ContainsKey(player))
            {
                _playerChannels.Add(player, Plugin.Singleton.Config.Channels[0]);
            }
            return _playerChannels[player];
        }

        public static void SetPlayerRadioChannel(ExPlayer player, string radioChannel)
        {
            _playerChannels[player] = radioChannel;
        }
    }