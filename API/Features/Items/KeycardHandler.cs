namespace GRPP.API.Features.Items;

using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Core;
using CustomItems;
using Enums;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Extensions;
using InventorySystem;
using InventorySystem.Items;
using LabApi.Features.Wrappers;
using Lobby;
using NorthwoodLib.Pools;
using Door = Exiled.API.Features.Doors.Door;
using Item = Exiled.API.Features.Items.Item;

public sealed class KeycardHandler : CustomItemHandler
{
    public static KeycardHandler Instance { get; private set; }
    
    private KeycardHandler() { Instance = this; }
    public override string Name => "DefaultKeycard";
    public override string[] Alias => ["Keycard"];
    
    public CustomItemContainer<GrppKeycard> Container { get; } = new();

    public override void EnableEvents()
    {
        Inventory.OnCurrentItemChanged += CurrentItemChanged;
        PlayerHandlers.ChangingItem += PickedUpItem;
        PlayerHandlers.InteractingLocker += InteractingLocker;
        PlayerHandlers.InteractingDoor += InteractingDoor;
    }

    public override bool HasItem(ushort serial) => Container.HasItem(serial);

    public override ItemBase GiveItem(ExPlayer player)
    {
        var item = player.AddItem(ItemType.KeycardJanitor);
        Container.RegisterItem(item.Base, new GrppKeycard("Default Name", "Default Role", "01ax1", 1, []));
        return item.Base;
    }

    public static Action<ItemBase> GiveItemCallback(ItemType type, string role, int currentLevel, params KeycardSubLevels[] levels)
    {
        return item =>
        {
            if (!CustomItemsManager.Get<KeycardHandler>().Container.HasItem(item, out var card))
                return;

            item.ItemTypeId = type;
            item.OwnerInventory.SendItemsNextFrame = true;

            card.Name = item.Owner.nicknameSync.DisplayName;
            card.Role = role;
            card.Checksum = new Random().Next(10, 99) + "ax" + new Random().Next(0, 9);
            card.CurrentLevel = currentLevel;
            card.CurrentSubLevels = levels.ToList();
        };
    }

    public static ItemGiver CreateInstance(ItemType item, string role, int currentLevel, params KeycardSubLevels[] levels) => (CustomItemsManager.Get<KeycardHandler>(), GiveItemCallback(item, role, currentLevel, levels));

    public void GiveCard(ExPlayer player, ItemType item, string name, string role, string checksum, int currentLevel,
        List<KeycardSubLevels> levels)
    {
        if (player.InventoryFull()) return;
        var val = Item.Create(item);
        player.AddItem(val);

        Container.RegisterItem(val.Base, new GrppKeycard(name, role, checksum, currentLevel, levels));
    }

    public bool GiveCustomKeycard(ExPlayer player, KeycardItem keycard, GrppKeycard grppKeycard, ICommandSender? sender = null)
    {
        if (player.InventoryFull())
        {
            sender?.Respond("Error! The user's inventory is full.");
            return false;
        }
        
        return Container.RegisterItem(keycard.Serial, grppKeycard);
    }
    public static string GetKeycardChecksum() => URandom.Range(10, 99) + "ax1";
    public override void ClearItems() => Container.ClearItems();

    private void PickedUpItem(ChangingItemEventArgs ev)
    {
        if (ev.Item == null)
            return;
        if (!ev.Item.IsKeycard || !Main.IsRoleplay || Container.HasItem(ev.Item.Serial) || CustomItemsManager.Get<CustomHandler>().Container.HasItem(ev.Item.Serial)) return;
        var role = "None";
        var currentLevel = 0;
        List<KeycardSubLevels> levels = [];

        switch (ev.Item.Type)
        {
            case ItemType.KeycardJanitor:
                role = "Janitor";
                currentLevel = 1;
                break;
            case ItemType.KeycardScientist:
                role = "Researcher";
                currentLevel = 2;
                levels = [KeycardSubLevels.Containment];
                break;
            case ItemType.KeycardResearchCoordinator:
                role = "Supervisor";
                currentLevel = 3;
                levels = [KeycardSubLevels.Containment, KeycardSubLevels.Engineering];
                break;
            case ItemType.KeycardZoneManager:
                role = "Zone Manager";
                currentLevel = 1;
                break;
            case ItemType.KeycardGuard:
                role = "Officer";
                currentLevel = 2;
                levels = [KeycardSubLevels.Containment, KeycardSubLevels.Security];
                break;
            case ItemType.KeycardMTFPrivate:
                role = "Private";
                currentLevel = 2;
                levels = [KeycardSubLevels.Containment, KeycardSubLevels.Security];
                break;
            case ItemType.KeycardContainmentEngineer:
                role = "Engineer";
                currentLevel = 3;
                levels = [KeycardSubLevels.Containment, KeycardSubLevels.Engineering];
                break;
            case ItemType.KeycardMTFOperative:
                role = "Operative";
                currentLevel = 3;
                levels = [KeycardSubLevels.Containment, KeycardSubLevels.Engineering, KeycardSubLevels.Security];
                break;
            case ItemType.KeycardMTFCaptain:
                role = "Captain";
                currentLevel = 4;
                levels = [KeycardSubLevels.Containment, KeycardSubLevels.Engineering, KeycardSubLevels.Security];
                break;
            case ItemType.KeycardFacilityManager:
                role = "Manager";
                currentLevel = 4;
                levels = [KeycardSubLevels.Containment, KeycardSubLevels.Engineering, KeycardSubLevels.Security];
                break;
            case ItemType.KeycardChaosInsurgency:
                role = "Chaos Insurgency";
                currentLevel = 4;
                levels = [KeycardSubLevels.Containment, KeycardSubLevels.Engineering, KeycardSubLevels.Security];
                break;
            case ItemType.KeycardO5:
                currentLevel = 5;
                levels = [KeycardSubLevels.Containment, KeycardSubLevels.Engineering, KeycardSubLevels.Security];
                break;
        }

        Container.RegisterItem(ev.Item.Base, new GrppKeycard(
            name: $"{GRPPCommands.Name.FirstNames[new Random().Next(GRPPCommands.Name.FirstNames.Count)]} " +
            $"{GRPPCommands.Name.LastNames[new Random().Next(GRPPCommands.Name.LastNames.Count)]}",
            role,
            GetKeycardChecksum(), currentLevel, levels));
    }

    private void CurrentItemChanged(ReferenceHub hub, ItemIdentifier oldItem, ItemIdentifier newItem)
    {
        var player = ExPlayer.Get(hub);

        string hintToShow = null;
        if (HasItem(oldItem.SerialNumber)) hintToShow = "";

        if (Container.HasItem(newItem.SerialNumber, out var card) && Item.Get(newItem.SerialNumber).Type != ItemType.KeycardChaosInsurgency)
        {
            var sb = StringBuilderPool.Shared.Rent();

            sb.AppendLine($"<align=left><size=17>");
            sb.AppendLine($"┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            sb.AppendLine($"┃<b>SCP</b><pos=612>┃");
            sb.AppendLine($"┃Secure. Contain. Protect.<space=300>ID# {card.Checksum}<pos=612>┃");
            sb.AppendLine($"┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫");
            sb.AppendLine($"┃┏━━━━━━┓ <b>FULL NAME</b><pos=612>┃");
            sb.AppendLine($"┃┃<pos=136>┃ {card.Name}<pos=612>┃");
            sb.AppendLine($"┃┃<pos=136>┃ <b>OPERATIONAL UNDER</b><pos=467><b>POSITION</b><pos=612>┃");
            sb.AppendLine($"┃┗━━━━━━┛ S.C.P Foundation<space=182>{card.Role}<pos=612>┃");
            sb.AppendLine($"┃<b>CLEARANCE CODES</b><pos=467><b>Level</b><pos=612>┃");
            sb.AppendLine($"┃{(card.CurrentSubLevels.Any()? string.Join(", ", card.CurrentSubLevels) : "No Clearance Codes...")}<pos=467>{card.CurrentLevel}<pos=612>┃");
            sb.AppendLine($"┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫");
            sb.AppendLine($"┃Site-{Main.Site}<pos=350>Iss: {DateTime.Today.AddYears(30):d}   Exp: {DateTime.Today.AddYears(32):d}<pos=612>┃");
            sb.AppendLine($"┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            
            hintToShow = StringBuilderPool.Shared.ToStringReturn(sb);
        }

        if (hintToShow != null) player.ShowHint(hintToShow, hintToShow == "" ? 0 : 900);
    }

    private void InteractingLocker(InteractingLockerEventArgs ev)
    {
        if (!Main.IsRoleplay)
            return;

        if (ev.Player?.IsBypassModeEnabled ?? true)
            return;

        if (ev.Player.CurrentItem == null)
            return;

        GrppKeycard card;

        switch (ev.InteractingLocker?.Type)
        {
            case LockerType.LargeGun:
            case LockerType.RifleRack:
                if (ev.InteractingChamber?.RequiredPermissions == KeycardPermissions.None)
                    return;
                ev.IsAllowed = false;
                if (!Container.HasItem(ev.Player.CurrentItem.Serial, out card))
                    return;

                if (card.CurrentLevel < 2 || !card.CurrentSubLevels.Contains(KeycardSubLevels.Security))
                    return;
                ev.IsAllowed = true;
                break;
            case LockerType.AntiScp207Pedestal: // temporary, cuz like. they made every pedestal into its own separate thing apparently !
                ev.IsAllowed = false;
                if (!Container.HasItem(ev.Player.CurrentItem.Serial, out card))
                    return;
                if (card.CurrentLevel < 3 || !card.CurrentSubLevels.Contains(KeycardSubLevels.Containment))
                    return;
                ev.IsAllowed = true;
                break;
            case LockerType.Unknown:
                if (ev.InteractingChamber.RequiredPermissions == KeycardPermissions.None)
                    return;
                ev.IsAllowed = false;
                if (!Container.HasItem(ev.Player.CurrentItem.Serial, out card))
                    return;
                if (card.CurrentLevel < 4 || !card.CurrentSubLevels.Contains(KeycardSubLevels.Security) && !card.CurrentSubLevels.Contains(KeycardSubLevels.Containment))
                    return;
                ev.IsAllowed = true;
                break;
            case LockerType.Misc:
            case LockerType.Medkit:
            case LockerType.Adrenaline:
            default:
                break;
        }
    }

    private void InteractingDoor(InteractingDoorEventArgs ev)
    {
        if (!Main.IsRoleplay)
            return;

        if (ev.Player.IsBypassModeEnabled)
        {
            ev.IsAllowed = true;
            return;
        }

        if (ev.Door.IsLocked)
            return;

        if (ev.Player.CurrentItem == null || !Container.HasItem(ev.Player.CurrentItem.Serial, out var card))
            card = null; // Yes I know this is just to prevent a null reference exception whenever interacting with a door with no items.
        ev.IsAllowed = IsDoorAccessible(ev.Player, ev.Door, card);
    }

    private class Permissions(int level, params KeycardSubLevels[] subLevels)
    {
        public readonly KeycardSubLevels[] SubLevels = subLevels;
        public readonly int Level = level;
    }

    public static bool IsDoorAccessible(ExPlayer player, Door door, GrppKeycard card)
    {
        bool HasValidCard(int minLevel, params KeycardSubLevels[] requiredLevels) => card.CurrentLevel >= minLevel && requiredLevels.All(level => card.CurrentSubLevels.Contains(level));
        bool DoorHavePermissions(out Permissions permissions)
        {
            permissions = door.Type switch
            {
                DoorType.HeavyContainmentDoor => new Permissions(-2),
                DoorType.LightContainmentDoor => new Permissions(-2),
                DoorType.EntranceDoor => new Permissions(-2),
                DoorType.UnknownDoor => new Permissions(-2),

                DoorType.Scp330 => new Permissions(2, KeycardSubLevels.Containment),
                DoorType.Scp914Gate => new Permissions(2, KeycardSubLevels.Containment),
                DoorType.GR18Inner => new Permissions(2, KeycardSubLevels.Containment),
                DoorType.Scp079First => new Permissions(2, KeycardSubLevels.Containment),
                DoorType.Scp079Second => new Permissions(2, KeycardSubLevels.Containment),


                DoorType.Scp096 => new Permissions(3, KeycardSubLevels.Containment),
                DoorType.Scp106Primary => new Permissions(3, KeycardSubLevels.Containment),
                DoorType.Scp106Secondary => new Permissions(3, KeycardSubLevels.Containment),
                DoorType.Scp173Gate => new Permissions(3, KeycardSubLevels.Containment),
                DoorType.Scp173NewGate => new Permissions(3, KeycardSubLevels.Containment),
                DoorType.Scp049Gate => new Permissions(3, KeycardSubLevels.Containment),

                DoorType.Scp049Armory => new Permissions(2, KeycardSubLevels.Containment, KeycardSubLevels.Security),

                DoorType.CheckpointLczA => new Permissions(1),
                DoorType.CheckpointLczB => new Permissions(1),
                DoorType.CheckpointEzHczA => new Permissions(1),
                DoorType.CheckpointEzHczB => new Permissions(1),

                DoorType.CheckpointArmoryA => new Permissions(1, KeycardSubLevels.Security),
                DoorType.CheckpointArmoryB => new Permissions(1, KeycardSubLevels.Security),

                DoorType.GateA => new Permissions(3, KeycardSubLevels.Security),
                DoorType.GateB => new Permissions(3, KeycardSubLevels.Security),

                DoorType.PrisonDoor => new Permissions(2, KeycardSubLevels.Security),
                DoorType.HczArmory => new Permissions(2, KeycardSubLevels.Security),
                DoorType.LczArmory => new Permissions(2, KeycardSubLevels.Security),
                DoorType.Scp079Armory => new Permissions(2, KeycardSubLevels.Security),

                DoorType.HIDChamber => new Permissions(2, KeycardSubLevels.Containment),
                DoorType.HIDLab => new Permissions(2, KeycardSubLevels.Containment),

                DoorType.Intercom => new Permissions(4),

                DoorType.NukeSurface => new Permissions(5, KeycardSubLevels.Containment, KeycardSubLevels.Engineering, KeycardSubLevels.Security),
                _ => new Permissions(-1),
            };

            return permissions.Level >= 0;
        }

        if (!DoorHavePermissions(out var perms) && perms.Level != -2)
            return true;

        if (door.Type is DoorType.HeavyContainmentDoor or DoorType.LightContainmentDoor or DoorType.EntranceDoor
            or DoorType.UnknownDoor)
        {
            return door.KeycardPermissions switch
            {
                KeycardPermissions.Intercom => HasValidCard(4),
                KeycardPermissions.ArmoryLevelOne => HasValidCard(2, KeycardSubLevels.Security),
                KeycardPermissions.ArmoryLevelTwo => HasValidCard(3, KeycardSubLevels.Security),
                KeycardPermissions.ArmoryLevelThree => HasValidCard(3, KeycardSubLevels.Engineering),

                KeycardPermissions.Checkpoints => HasValidCard(1, KeycardSubLevels.Security),

                KeycardPermissions.ContainmentLevelOne => HasValidCard(2, KeycardSubLevels.Containment),
                KeycardPermissions.ContainmentLevelTwo => HasValidCard(3, KeycardSubLevels.Containment),
                KeycardPermissions.ContainmentLevelThree => HasValidCard(4, KeycardSubLevels.Containment),

                // KeycardPermissions.ExitGates => expr,
                // KeycardPermissions.AlphaWarhead => expr,
                _ => true
            };
        }

        if (player.CurrentItem == null || card == null)
            return false;

        return HasValidCard(perms.Level, perms.SubLevels);
    }

    public sealed class GrppKeycard(string name, string role, string checksum, int currentLevel, IEnumerable<KeycardSubLevels> currentSubLevels)
    {
        public string Name { get; set; } = name;
        public string Role { get; set; } = role;
        public string Checksum { get; set; } = checksum;
        public int CurrentLevel { get; set; } = currentLevel;
        public IEnumerable<KeycardSubLevels> CurrentSubLevels { get; set; } = currentSubLevels;
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class GiveKeycard : ICommand
{
    public string Command => "GiveKeycard";
    public string[] Aliases => ["gid", "GiveId"];
    public string Description => "Give a player a new Identification";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;

        if (arguments.Count < 5)
        {
            response = "<color=red>Arguments : giveid (Player Id / Name) (Item Id) (Perms) (Checkpoint Checksum) ('Position')";
            return false;
        }

        response = "<color=red>Invalid Player...\nUse the Players <color=yellow><b>In-Game ID</b></color> or <color=yellow><b>Steam UsernameName</b>";
        if (!ExPlayer.TryGet(arguments.At(0), out var player))
            return false;

        response = "<color=red>Invalid Item...\nFind the ItemID in the <color=yellow>\"Inventory\"</color> tab in <color=yellow><b>your RA</b>";
        if (!Enum.TryParse(arguments.At(1), out ItemType itemType))
            return false;

        response = "<color=red>Invalid Level...\n<color=yellow>Here's an example on what to use... currently <b>LEVEL</b> is <color=red><b>Incorrect...\n<color=green><b>Example: (Level){0-5}(SubPermissions)C(Containment)E(Engineering)Security}";
        if (!int.TryParse(arguments.At(2)[0].ToString(), out var i))
            return false;

        if (i is < 0 or > 5)
            return false;

        List<KeycardSubLevels> levels = [];

        if (arguments.At(2).ToLower().Contains("c"))
            levels.Add(KeycardSubLevels.Containment);

        if (arguments.At(2).ToLower().Contains("e"))
            levels.Add(KeycardSubLevels.Engineering);

        if (arguments.At(2).ToLower().Contains("s"))
            levels.Add(KeycardSubLevels.Security);

        response = "<color=red>Too many characters at <color=yellow>CHECKSUM\n<color=yellow>The limit is 9 characters";
        if (arguments.At(3).Length > 9)
            return false;

        // Get the arguments after the first one
        var additionalArguments = new ArraySegment<string>(arguments.Array!, arguments.Offset + 4, arguments.Count - 4);

        // Join the additional arguments into a single string if needed
        var additionalArgumentsString = string.Join(" ", additionalArguments);
        response = "<color=red>Too many characters at <color=yellow>Position\n<color=yellow>The limit is 17";

        if (additionalArgumentsString.Length > 17)
            return false;

        KeycardHandler.Instance.GiveCard(player, itemType, player.DisplayNickname, additionalArgumentsString, arguments.At(3), i, levels);
        response = $"<color=green>Successfully gave ID card to {player.DisplayNickname}" +
                   $"\nItem: {itemType}" +
                   $"\nPermissions:" +
                   $"\n> {i}" +
                   $"\n> {string.Join(", ", levels)}" +
                   $"\nPosition: {additionalArgumentsString}</color>";
        return true;
    }
}