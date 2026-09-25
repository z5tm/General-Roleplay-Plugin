namespace GRPP.API.Features.Items.Scp1499;

using System;
using System.ComponentModel;
using System.Threading;
using Core.CustomItems;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Extensions;
using Hints;
using InventorySystem.Items;

public sealed class Scp1499Item : CustomItemHandler
{
    public CustomItemContainer Container { get; } = new();
    public const ItemType CustomItemType = ItemType.SCP268;
    
    public override string Name => "Scp1499";
    public override string[] Alias => ["1499"];
    public const int Cooldown = 10;
    public static CancellationTokenSource? CancellationTokenSource;
    public DateTime? LastUsed;
    
    public override void EnableEvents()
    {
        LastUsed = null;
        CancellationTokenSource = new CancellationTokenSource();
        
        PlayerHandlers.UsingItem += UsingItem;
        PlayerHandlers.ChangingItem += ChangingItem;
    }
    
    public override void DisableEvents()
    {
        PlayerHandlers.UsingItem -= UsingItem;
        PlayerHandlers.ChangingItem -= ChangingItem;
        
        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = null;
        Container.ClearItems();
        
        LastUsed = null;
    }
    
    public async void UsingItem(UsingItemEventArgs ev)
    {
        try
        {
            if (!ev.Item.Base || !HasItem(ev.Item.Base) || CancellationTokenSource is null or { IsCancellationRequested: true })
                return;
            
            if (LastUsed is not null && (DateTime.UtcNow - LastUsed) < TimeSpan.FromSeconds(Cooldown))
            {
                ev.Player.ClearAllTags();
                ev.Player.RueIMessage("This item is on cooldown!", position: 80f);
                ev.IsAllowed = false;
                return;
            }
            
            ev.Item.Destroy();
            ev.IsAllowed = false;
            ev.Player.ClearAllTags();
            ev.Player.RueIMessage("You put on the gasmask..");
            await Scp1499Manager.SendTo1499(ev.Player);
            
            GiveItem(ev.Player);
            
            LastUsed = DateTime.UtcNow;
        }
        catch (OperationCanceledException)
        {
            Log.Debug("Operation canceled whilst using SCP-1499.");
        }
        catch (Exception e)
        {
            Log.Error($"Exception caught whilst attempting to use SCP-1499! {e}");
        }
    }
    
    public void ChangingItem(ChangingItemEventArgs ev)
    {
        if (ev.Item == null)
            return;
        
        if (!HasItem(ev.Item.Base))
            return;
        
        ev.Player.ClearAllTagsExcept([Defaults.Tagging.ErrorTag, Defaults.Tagging.WarningTag]);
        ev.Player.RueIMessage("<size=23><space=2.6em><b>SCP-1499</b></space></size>");
        // <mark> is a background color setter
    }
    
    public override bool HasItem(ushort serial) => Container.HasItem(serial);
    
    public override void ClearItems() => Container.ClearItems();
    
    public override ItemBase GiveItem(ExPlayer player)
    {
        var item = player.AddItem(CustomItemType);
        Container.RegisterItem(item.Base);
        return item.Base;
    }
}