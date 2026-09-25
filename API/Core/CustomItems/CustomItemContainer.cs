namespace GRPP.API.Core.CustomItems;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Exiled.API.Features;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Logger = LabApi.Features.Console.Logger;

public sealed class CustomItemContainer
{
    public readonly HashSet<ushort> Serials = [];
    
    public void RegisterItem(ItemPickupBase? pickup)
    {
        if (!pickup)
        {
            Logger.Warn($"WARNING: NullRef suppressed. Pickup was null in RegisterItem.\nNo information can be provided, as pickup was the only required component.");
            throw new ArgumentNullException(nameof(pickup));
        }

        Serials.Add(pickup.Info.Serial);
    }
    
    public void RegisterItem(ItemBase item)
    {
        if (!item)
            throw new ArgumentNullException(nameof(item));

        Serials.Add(item.ItemSerial);
    }
    
    public void RegisterItem(ushort serial)
    {
        Serials.Add(serial);
    }
    
    public void RemoveItem(ItemPickupBase pickup)
    {
        if (pickup == null)
            return;
        
        Serials.Remove(pickup.Info.Serial);
    }
    
    public void RemoveItem(ItemBase item)
    {
        if (item == null)
            return;
        
        Serials.Remove(item.ItemSerial);
    }
    
    public void RemoveItem(ushort serial)
    {
        Serials.Remove(serial);
    }
    
    public bool HasItem(ItemPickupBase pickup)
    {
        return pickup && Serials.Contains(pickup.Info.Serial); // return pickup is LITERALLY Just /* if (pickup == null) return; _serials.Contains(pickup.Info.Serial); */ - which is SO cool.
    }
    
    public bool HasItem(ItemBase item)
    {
        return item && Serials.Contains(item.ItemSerial);
    }
    
    public bool HasItem(ushort serial)
    {
        return Serials.Contains(serial);
    }
    
    public void ClearItems()
    {
        Serials.Clear();
    }
}

public sealed class CustomItemContainer<T>
{
    private readonly Dictionary<ushort, T> _serials = new();
    
    public void RegisterItem(ItemPickupBase pickup, T value)
    {
        if (pickup == null)
            throw new ArgumentNullException(nameof(pickup));
        
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        
        _serials.Add(pickup.Info.Serial, value);
    }
    
    public void RegisterItem(ItemBase item, T value)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        
        _serials.Add(item.ItemSerial, value);
    }
    
    public bool RegisterItem(ushort serial, T value)
    {
        if (Equals(value, default(T)))
        {
            Log.Error("Error! Value in RegisterItem is null or default!");
            return false;
        }
        
        _serials.Add(serial, value);
        return true;
    }
    
    public void SetItemValue(ItemPickupBase pickup, T value)
    {
        if (pickup == null)
            throw new ArgumentNullException(nameof(pickup));
        
        if (!HasItem(pickup))
            throw new ArgumentException("Key is not present in the dictionary.", nameof(pickup));
        
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        
        _serials[pickup.Info.Serial] = value;
    }
    
    public void SetItemValue(ItemBase item, T value)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        
        if (!HasItem(item))
            throw new ArgumentException("Key is not present in the dictionary.", nameof(item));
        
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        
        _serials[item.ItemSerial] = value;
    }
    
    public void SetItemValue(ushort serial, T value)
    {
        if (!HasItem(serial))
            throw new ArgumentException("Key is not present in the dictionary.", nameof(serial));
        
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        
        _serials[serial] = value;
    }
    
    public void RemoveItem(ItemPickupBase pickup)
    {
        if (pickup == null)
            return;
        
        _serials.Remove(pickup.Info.Serial);
    }
    
    public void RemoveItem(ItemBase item)
    {
        if (item != null)
            _serials.Remove(item.ItemSerial);
    }
    
    public void RemoveItem(ushort serial) => _serials.Remove(serial);
    
    
    public bool HasItem(ItemPickupBase pickup) => pickup != null && _serials.ContainsKey(pickup.Info.Serial);
    
    
    public bool HasItem(ItemBase item) => item != null && _serials.ContainsKey(item.ItemSerial);
    
    
    public bool HasItem(ushort serial) => _serials.ContainsKey(serial);
    
    [MemberNotNullWhen(true)]
    public bool HasItem(ItemPickupBase pickup, out T? value)
    {
        if (pickup != null)
            return _serials.TryGetValue(pickup.Info.Serial, out value);
        
        value = default;
        return false;
    }
    
    [MemberNotNullWhen(true)]
    public bool HasItem(ItemBase item, out T? value)
    {
        if (item != null)
            return _serials.TryGetValue(item.ItemSerial, out value);
        
        value = default;
        return false;
    }
    
    public bool HasItem(ushort serial, out T value) => _serials.TryGetValue(serial, out value);
    public void ClearItems() => _serials.Clear();
}