namespace GRPP.Extensions;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using API.Features;
using Exiled.API.Enums;
using MEC;
using PlayerRoles;
using PlayerRoles.Voice;
using API.Features.Department;
using API.Features.GRPPCommands;
using API.Features.Lobby;
using CustomPlayerEffects;
using Exiled.API.Features;
using RueI.API;
using RueI.API.Elements;
using UnityEngine;
using Player = LabApi.Features.Wrappers.Player;
using RadioItem = InventorySystem.Items.Radio.RadioItem;

public static class PlayerExtensions
{
    extension(ExPlayer player)
    {
        public bool SetRole(string roleName, string department = null, RoleSpawnFlags roleSpawnFlags = RoleSpawnFlags.None)
        {
            var scomPlayer = player.ScomPlayer();
            var curRole = scomPlayer.CurrentRole;
            
            if (!curRole.SetRole(roleName, department))
                return false;
            
            player.Role.Set(curRole.Rank.RoleTypeID, SpawnReason.None, roleSpawnFlags.HasFlag(RoleSpawnFlags.UseSpawnpoint) ? RoleSpawnFlags.UseSpawnpoint : RoleSpawnFlags.None);
            var random = new System.Random();
            var finalName = curRole.RoleEntry.Role.Prefix;
            if (!string.IsNullOrEmpty(curRole.RoleEntry.Role.Prefix))
            {
                var index = random.Next(Name.LastNames.Count);
                finalName = finalName.Replace("[L]", Name.LastNames[index]);

                index = random.Next(Name.FirstNames.Count);
                finalName = finalName.Replace("[F]", Name.FirstNames[index]);

                finalName = finalName.Replace("[N]", random.Next(1, 100000).ToString().PadLeft(5, '0'));
            }
            
            player.DisplayNickname = finalName;
            player.CustomInfo = $"[- {curRole.RankName} -]\n{curRole.RoleEntry.Role.CustomI}";
            player.Scale = Vector3.one * URandom.Range(0.9f, 1.1f);
            
            if (roleSpawnFlags.HasFlag(RoleSpawnFlags.AssignInventory) && Main.HasRoleplayStarted || !Main.IsRoleplay && roleSpawnFlags.HasFlag(RoleSpawnFlags.AssignInventory))
            {
                player.ClearAmmo();
                player.ClearInventory();
                foreach (var item in curRole.Rank.LoadOut)
                {
                    BeginRoleplay.GetItem(item, out var cost, player).GiveItem(player);
                    if (!Main.IsRoleplay) continue;
                    Department.DepartmentsData[Department.GetDepartmentByRole(curRole.RoleEntry)].Balance -= cost;
                    
                    Department.UpdateDepartmentData(Department.GetDepartmentByRole(curRole.RoleEntry));
                }
            }
            
            if (Main.HasRoleplayStarted)
                Timing.RunCoroutine(player.ScomPlayer().TrackHours());
            return true;
        }
        
        public bool InventoryFull() => player.Inventory.UserInventory.Items.Count >= 8;
        public RueDisplay Display => RueDisplay.Get(player);
        
        public void RueIMessage(string message, Tag? messageIdentifierTag = null, float timeSeconds = 5f, float position = 100f, IEnumerable<Tag>? tagsToClear = null)
        {
            if (tagsToClear != null)
                player.ClearTheseTags(tagsToClear);
            
            player.Display.Show(messageIdentifierTag?.Id == null ? Defaults.Tagging.GenericTag : messageIdentifierTag, new BasicElement(position, message), timeSeconds);
        }
        
        public void RueIMessage(Element message, Tag? messageIdentifierTag = null, float timeSeconds = 5f)
        {
            if (messageIdentifierTag?.Id == null)
            {
                player.Display.Show(message, timeSeconds);
                return;
            }
            player.Display.Show(messageIdentifierTag, message, timeSeconds);
        }
        
        public void RemoveAllTags()
        {
            foreach (var tag in Defaults.Tagging.All.Where(tag => tag?.Id != null))
                player?.Display?.Remove(tag);
        }
        public void ClearAllTags() => player.RemoveAllTags();
        
        public void RemoveAllTagsExcept(Tag tagToExclude)
        {
            foreach (var tag in Defaults.Tagging.All.Where(tag => tag?.Id != null && tag != tagToExclude))
                player?.Display?.Remove(tag);
        }
        public void RemoveAllTagsExcept(IEnumerable<Tag> tagsToExclude)
        {
            foreach (var tag in Defaults.Tagging.All.Where(tag => tag?.Id != null && !tagsToExclude.Contains(tag)))
                player?.Display?.Remove(tag);
        }
        
        public void ClearAllTagsExcept(Tag tagToExclude) => player.RemoveAllTagsExcept(tagToExclude);
        public void ClearAllTagsExcept(IEnumerable<Tag> tagsToExclude) => player.RemoveAllTagsExcept(tagsToExclude);

        public void RemoveTheseTags(IEnumerable<Tag> tagsToClear)
        {
            foreach (var tag in Defaults.Tagging.All.Where(tag => tag?.Id != null && tagsToClear.Contains(tag)))
                player?.Display?.Remove(tag);
        }
        public void ClearTheseTags(IEnumerable<Tag> tagsToClear) => player.RemoveTheseTags(tagsToClear);
        public void RemoveTag(Tag tag) => player.Display.Remove(tag);
        public void ClearTag(Tag tag) => player.RemoveTag(tag);
    }
    
    public static bool InventoryFull(this ReferenceHub player) => player.inventory.UserInventory.Items.Count >= 8;
    
    extension(ExPlayer sender)
    {
        public void SendScomMessage(ExPlayer receiver, string message)
        {
            if (sender.ScomPlayer().CurrentRole.RoleEntry == null)
            {
                sender.SendConsoleMessage("You aren't any Role Playing role...?", "red");
                return;
            }
            if (sender.ScomPlayer().CurrentRole.Rank == null)
            {
                sender.SendConsoleMessage($"You aren't any Rank somehow... what the fuck Code: {sender.ScomPlayer().CurrentRole.Rank}?", "red");
                return;
            }
            if (!sender.ScomPlayer().CurrentRole.Rank.HasPda)
            {
                sender.SendConsoleMessage("mmm...", "red");
                return;
            }
        
            if (!sender.ScomPlayer().ScomEnabled)
            {
                sender.SendConsoleMessage("> Your SCOM is deactivated, reactivate it to use it...", "red");
                return;
            }
        
            var unknown = sender.Role.Team != Team.ChaosInsurgency && sender.Role.Type != RoleTypeId.Scp079;
        
            sender.SendConsoleMessage("> Your SCOM message is being received please be patient...", "red");
            receiver.ReceiveScomMessage(sender, message, unknown);
        }

        public void ReceiveScomMessage(ExPlayer sender1, string message, bool unknown)
        {
            var name = unknown ? sender1.DisplayNickname : "Unknown";
            var id = unknown ? sender1.Id : -1;

            if (sender.ScomPlayer().CurrentRole.RoleEntry == null || sender.ScomPlayer().CurrentRole.Rank == null || !sender.ScomPlayer().CurrentRole.Rank.HasPda)
            {
                sender1.SendConsoleMessage("> Unknown SCOM User", "red");
                return;
            }

            if (!sender.ScomPlayer().ScomEnabled)
            {
                sender1.SendConsoleMessage("> Their SCOM System is deactivated, you cannot message them until they have reactivated it...", "red");
                return;
            }

            sender.SendConsoleMessage($"> Loading SCOM Message\n> Scom User: {name}\n> Scom User ID:{id}\n> (Moderation){sender1.Id}\n> {message}", "white");
            const string attached = "You have one new unread message...";
            sender.ShowHint($"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<size=27><mark=#367ce8><size=23>|🌐|</size></mark><mark=#595959>||</mark><mark=#393D4780> <size=23><space=2.6em><b>{attached.ReplaceLetters()}</size><space=2.6em><size=0.1>.</size></mark></size>");
            sender1.SendConsoleMessage("> Your message has been sent, and received on their end...", "green");
        }
        
        public bool HasRadio([NotNullWhen(true)] out RadioItem? radio)
        {
            if (sender.RoleManager.CurrentRole is IVoiceRole { VoiceModule: IRadioVoiceModule radioModule })
                return radioModule.RadioPlayback.TryGetUserRadio(out radio);
            radio = null;
            return false;
        }
        
        public void GrppEnableEffects(IEnumerable<Effect> effects)
        {
            foreach (var effect in effects)
                sender.EnableEffect(effect.Type, effect.Intensity, effect.Duration);
        }

        public void GrppEnableEffects(IEnumerable<StatusEffectBase> effects)
        {
            foreach (var effect in effects)
                sender.EnableEffect(effect);
        }
    }
    
    public static bool HasRadio(this ReferenceHub player, [NotNullWhen(true)] out RadioItem? radio)
    {
        if (player.roleManager.CurrentRole is IVoiceRole { VoiceModule: IRadioVoiceModule radioModule })
            return radioModule.RadioPlayback.TryGetUserRadio(out radio);
        
        radio = null;
        return false;
    }
    
    public static bool TryGetExiledPlayerById(string id, [NotNullWhen(true)] out ExPlayer? player)
    {
        if (int.TryParse(id, out var playerId))
            return ExPlayer.TryGet(playerId, out player); 
        
        player = null;
        return false;
    }
    
    public static bool TryGetPlayerById(string id, [NotNullWhen(true)] out Player? player)
    {
        if (int.TryParse(id, out var playerId))
            return Player.TryGet(playerId, out player); 
        
        player = null;
        return false;
    }
}