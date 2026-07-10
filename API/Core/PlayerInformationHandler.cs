namespace GRPP.API.Core;

using System.Collections.Generic;
using Attributes;
using Exiled.Events.EventArgs.Player;
using Extensions;
using JetBrains.Annotations;

public static class PlayerInformationHandler
{
    public static readonly Dictionary<string, int> Players = [];
    
    [UsedImplicitly]
    [OnPluginEnabled]
    public static void InitializeLogging()
    {
        PlayerHandlers.Verified += LogPlayerName;
        PlayerHandlers.Left += RemovePlayerName;
    }
    
    private static void LogPlayerName(VerifiedEventArgs ev)
    {
        if (Players.ContainsKey(ev.Player.Nickname))
        {
            ev.Player.ClearAllTags();
            ev.Player.RueIMessage("<color=yellow>Warning!</color> <color=orange>You have the same name as another player in the server. " +
                                  "Some features may not be available for you!</color>");
            return;
        }
        
        Players.Add(ev.Player.Nickname, ev.Player.Id);
    }
    
    private static void RemovePlayerName(LeftEventArgs ev) => Players.Remove(ev.Player.Nickname);
}