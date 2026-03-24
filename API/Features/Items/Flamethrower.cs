namespace GRPP.API.Features.Items;

using CustomItems;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using InventorySystem.Items;
using LabApi.Events.Arguments.Scp914Events;

public sealed class FlameThrowerHandler : CustomItemHandler
{
    private FlameThrowerHandler()
    {
    }

    public CustomItemContainer Container { get; } = new();

    public override string Name => "Flamethrower";
    public override string[] Alias => ["flame", "ft"];

    public override void EnableEvents()
    {
        PlayerHandlers.Hurting += Flame;
        PlayerHandlers.ChangingItem += ChangingItem;
        PlayerHandlers.ReloadingWeapon += ReloadFlame;
        PlayerHandlers.UnloadingWeapon += UnloadFlame;
    }

    public void ChangingItem(ChangingItemEventArgs ev)
    {
        if (ev.Item == null)
            return;

        if (!HasItem(ev.Item.Base))
            return;
        
        ev.Player.ShowHint("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<size=27><mark=#367ce8><size=23>|🔥|</size></mark><mark=#595959>||</mark><mark=#393D4780> <size=23><space=2.6em><b>Flamethrower</size><space=2.6em><size=0.1>.</size></mark></size>");
    }

    public override bool HasItem(ushort serial) => Container.HasItem(serial);

    public override void ClearItems() => Container.ClearItems();

    private void Flame(HurtingEventArgs ev)
    {
        if (ev.Attacker?.CurrentItem == null)
            return;

        if (!HasItem(ev.Attacker.CurrentItem.Base))
            return;

        if (ev.Attacker == ev.Player)
            return;

        if (ev.DamageHandler.Type != DamageType.Logicer)
            return;

        if (ev.Player.IsGodModeEnabled)
            return;

        ev.Amount = 1f;

        ev.Player.ShowHint("You are currently being burnt by a flamethrower.", 10f);
        ev.Player.EnableEffect(EffectType.Burned, 10f);
        ev.Player.EnableEffect(EffectType.Poisoned, 2, 15f);
    }

    public void UnloadFlame(UnloadingWeaponEventArgs ev)
    {
        if (ev.Item == null)
            return;

        if (!HasItem(ev.Item.Base))
            return;
        ev.IsAllowed = false;
        ev.Player.ShowHint("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<size=27><mark=#367ce8><size=23>|🔥|</size></mark><mark=#595959>||</mark><mark=#393D4780> <size=23><space=2.6em><b>You tried to unload the flamethrower, but it failed.</size><space=2.6em><size=0.1>.</size></mark></size>");
        ev.Item.Destroy();
    }
    public void ReloadFlame(ReloadingWeaponEventArgs ev)
    {
        if (ev.Item == null)
            return;

        if (!HasItem(ev.Item.Base))
            return;
        Log.Info("Player attempted to reload Flamethrower.");
        ev.Player.ShowHint("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<size=27><mark=#367ce8><size=23>|🔥|</size></mark><mark=#595959>||</mark><mark=#393D4780> <size=23><space=2.6em><b>You tried to reload the flamethrower, but it failed.</size><space=2.6em><size=0.1>.</size></mark></size>");
        ev.IsAllowed = false;
    }
    public override ItemBase GiveItem(ExPlayer player)
    {
        var item = player.AddItem(ItemType.GunLogicer);
        var firearm = (Exiled.API.Features.Items.Firearm)item;
        Container.RegisterItem(item.Base);
        firearm.MagazineAmmo = 50;
        firearm.MaxMagazineAmmo = 50;

        return item.Base;
    }
}