using CommandSystem;
using Exiled.API.Features;
using GRPP;
using GRPP.API.Core.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;

public class ModmailHandler
{
    public static int index;
    public static Dictionary<int, Player> modmailInstances = new Dictionary<int, Player>();

    public static void InitEvents() => ServerHandlers.WaitingForPlayers += WaitingForPlayers;

    private static void WaitingForPlayers() => index = 0;
}

[CommandHandler(typeof(ClientCommandHandler))]
public class Modmail : ICommand
{
    public string Command { get; } = "modmail";
    public string[] Aliases { get; } = new string[] { "mm" };
    public string Description { get; } = "Sends a message to all moderators currently in the server.";
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);

        if (arguments.Count == 0)
        {
            response = "Usage: .mm [message]";
            return false;
        }

        ModmailHandler.index += 1;
        var currentIndex = ModmailHandler.index;
        ModmailHandler.modmailInstances.Add(currentIndex, player);

        var message = string.Join(" ", arguments.ToArray());

        foreach (Player target in Player.List)
        {
            if (target.RemoteAdminAccess)
            {
                target.SendConsoleMessage($"[ModMail] {player.DisplayNickname} ({player.UserId}) (Modmail Index: {currentIndex}): {message}", "yellow");
                target.Broadcast(10, $"New modmail from {player.DisplayNickname} ({player.UserId}). Check the client console!");
            }
        }
        if (!Plugin.Singleton.Config.ModmailCommandWebhookUrl.IsEmpty())
            _ = AsyncWebhookHandler.LogMessage(
                webhookNameToUse: "ModmailLogger",
                webhookUrl: Plugin.Singleton.Config.ModmailCommandWebhookUrl,
                title: "New ModMail Message",
                description: $"A user has sent a modmail message. Index: {currentIndex}.\nName: \"{player.DisplayNickname}\"\nSteamID64: \"{player.UserId}\"\nMessage: \"{message}\"",
                color: "088808");
        response = "Your message has been sent to the moderators!";
        return true;
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class Reply : ICommand
{
    public string Command { get; } = "reply";
    public string[] Aliases { get; } = new string[] { "r" };
    public string Description { get; } = "Replies to a modmail message using the index provided in the message.";
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        if (arguments.Count < 2)
        {
            response = "Usage: .reply [index] [message]";
            return false;
        }
        if (!int.TryParse(arguments.At(0), out int index))
        {
            response = "The index must be a valid integer.";
            return false;
        }
        if (!ModmailHandler.modmailInstances.ContainsKey(index))
        {
            response = "No modmail message found with the provided index.";
            return false;
        }

        var target = ModmailHandler.modmailInstances[index];
        var message = string.Join(" ", arguments.Skip(1).ToArray());

        target.SendConsoleMessage($"[ModMail Reply] {player.DisplayNickname}[Staff]: {message}", "yellow");
        target.Broadcast(10, "You have received a reply to your modmail message. Check the client console!");

        response = $"Reply sent to {target.DisplayNickname}!";
        return true;
    }
}