namespace GRPP.Patches;

using System;
using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using HarmonyLib;
using LabApi.Features.Wrappers;
using PlayerRoles;

// todo fix idk what i was tryna do atm

// [HarmonyPatch(typeof(Scp1344Detected), nameof(Scp1344Detected.ServerRegisterObserver))]
// public static class Scp1344Patch
// {
//     public static bool Prefix(Scp1344Detected __instance)
//     {
//         if (Plugin.Singleton.Config?.Scp1344NoclipExclusion == null || Plugin.Singleton.Config.Scp1344NoclipDetection)
//             return true;
//         
//         if (!(_excludedRoleTypeIds?.Any() ?? false))
//             return ExclusionHandler(__instance);
//         
//         return _excludedRoleTypeIds.Any(roleId => __instance.Hub.GetRoleId() == roleId) || ExclusionHandler(__instance);
//     }
//     
//     private static bool ExclusionHandler(Scp1344Detected instance)
//     {
//         if (Plugin.Singleton.Config?.Scp1344NoclipExclusion == null || Plugin.Singleton.Config.Scp1344NoclipDetection) return true;
//         if (_excludedRoleTypeIds == null) return !Player.Get(instance.Hub).IsNoclipEnabled; // return false if noclip enabled, stopping the event
//         
//         var rolesToCheck = Plugin.Singleton.Config.Scp1344NoclipExclusion.Where(str => !string.IsNullOrWhiteSpace(str)).ToList();
//         
//         if (!rolesToCheck.Any())
//         {
//             _excludedRoleTypeIds = null;
//             return Player.Get(instance.Hub).IsNoclipEnabled;
//         }
//         
//         var roleIdList = new List<RoleTypeId>();
//         foreach (var role in rolesToCheck)
//             if (Enum.TryParse(role, true, out RoleTypeId roleId))
//                 roleIdList.Add(roleId);
//         
//         _excludedRoleTypeIds = roleIdList.Any() ? roleIdList : null;
//         
//         if (roleIdList.Any(roleId => instance.Hub.GetRoleId() == roleId))
//             return true;
//         
//         return !Player.Get(instance.Hub).IsNoclipEnabled;
//     }
//     
//     private static List<RoleTypeId>? _excludedRoleTypeIds = [];
// }