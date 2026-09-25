namespace GRPP.API.Features.Items.Scp1499;

using System;
using System.Collections.Generic;
using Attributes;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Extensions;
using JetBrains.Annotations;
using LabApi.Events.Handlers;
using ProjectMER.Features;
using UnityEngine;

public static class Scp1499Manager
{
    public static bool Scp1499DimensionLoaded => MapUtils.LoadedMaps.ContainsKey(Plugin.Singleton.Config.Scp1499Map);
    private static Dictionary<ExPlayer, Vector3>? _scp1499Positions = [];
    private static Dictionary<ExPlayer, IEnumerable<StatusEffectBase>>? _scp1499PriorEffects = [];
    
    private static readonly Effect[] Scp1499Effects = [new (EffectType.FogControl, 0, 7)];
    
    public static void GetEffectByString(string effectName, out Effect effect) { effect = new Effect { Type = Enum.TryParse(effectName, out EffectType type) ? type : EffectType.None }; }
    
    [UsedImplicitly]
    [OnPluginEnabled]
    public static void Initialize()
    {
        _scp1499PriorEffects = [];
        _scp1499Positions = [];
        ServerEvents.WaitingForPlayers += LoadScp1499;
    }
    
    [UsedImplicitly]
    [OnPluginDisabled]
    public static void Deinitialize()
    {
        ServerEvents.WaitingForPlayers -= LoadScp1499;
        _scp1499PriorEffects = null;
        _scp1499Positions = null;
    }
    
    public static async Awaitable SendTo1499(ExPlayer player)
    {
        if (_scp1499Positions == null)
        {
            Log.Error("Scp1499Positions is null! We recommend disabling the plugin, or contacting the main developer.");
            return;
        }
        
        player.ClearAllTags();
        player.RueIMessage("You put the gasmask on..");
        _scp1499PriorEffects?[player] = player.ActiveEffects;
        
        var origPos = player.Position;
        player.Teleport(!_scp1499Positions.TryGetValue(player, out var pos) ? Plugin.Singleton.Config.Scp1499PlayerSpawnPoint : pos);
        player.GrppEnableEffects(Scp1499Effects);
        
        await Awaitable.WaitForSecondsAsync(Plugin.Singleton.Config.Scp1499Time);
        
        _scp1499Positions[player] = player.Position;
        player.Teleport(origPos);
        player.DisableAllEffects();
        
        if (_scp1499PriorEffects != null)
            player.GrppEnableEffects(_scp1499PriorEffects[player]);
    }
    
    public static void LoadScp1499()
    {
        if (MapUtils.LoadedMaps.ContainsKey(Plugin.Singleton.Config.Scp1499Map))
            return;
        
        if (string.IsNullOrWhiteSpace(Plugin.Singleton.Config.Scp1499Map))
        {
            Log.Warn("Not loading SCP-1499, as schematic name is empty.");
            return;
        }
        
        MapUtils.LoadMap(Plugin.Singleton.Config.Scp1499Map);
    }
}