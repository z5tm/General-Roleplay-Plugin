namespace GRPP.Patches;

using CommandSystem.Commands.RemoteAdmin.Inventory;
using Player = LabApi.Features.Wrappers.Player;
using API.Features.CustomKeycard;
using CommandSystem;
using HarmonyLib;
using API.Core;
using EasyTmp;
using System;
using Extensions;

[HarmonyPatch(typeof(CustomKeycardCommand), nameof(CustomKeycardCommand.Usage), MethodType.Getter)]
public class CKeycardUsagePatch
{
    public static bool Prefix(ref string[] __result)
    {
        __result = ["runCmdForArgs"];
        return false;
    }
}

[HarmonyPatch(typeof(CustomKeycardCommand), nameof(CustomKeycardCommand.Description), MethodType.Getter)]
public class CKeycardDescriptionPatch
{
    public static bool Prefix(ref string __result)
    {
        __result = string.Empty;
        return false;
    }
}

[HarmonyPatch(typeof(CustomKeycardCommand), nameof(CustomKeycardCommand.Execute))]
public class CKeycardPatch
{
    private const string CommandHintHelper = "\nInstructions ^"; // to prevent the little ingame command hint thing from not showing our epic instructions
    private enum KeycardType : byte
    { KeycardCustomTaskForce = 0, KeycardCustomSite02 = 1, KeycardCustomManagement = 2, KeycardCustomMetalCase = 3 }
    
    private static KeycardType? GetKeycardType(string keycardType)
    {
        switch (keycardType.ToLowerInvariant())
        {
            case "mt":
            case "ntf":
            case "mtf":
            case "taskforce":
            case "customtaskforce":
            case "keycardcustomtaskforce":
                return KeycardType.KeycardCustomTaskForce;
            
            case "me":
            case "metal":
            case "metalcase":
            case "keycardcustommetalcase":
                return KeycardType.KeycardCustomMetalCase;
            
            case "ma":
            case "man":
            case "admin":
            case "management":
            case "managementcard":
            case "keycardcustommanagement":
                return KeycardType.KeycardCustomManagement;
            
            case "s":
            case "02":
            case "site":
            case "site02":
            case "keycardcustomsite02":
                return KeycardType.KeycardCustomSite02;
            
            default:
                return null;
        }
    }
    
    private static readonly string Usage = EasyArgs.Build().NewLine().CmdArguments("ckeycard userId/playerName keycardType").Done();
    
    public static bool Prefix(ArraySegment<string> arguments, ICommandSender sender, ref string response)
    {
        if (!Plugin.Singleton.Config.OverrideCKeycard)
            return true;
        
        if (arguments.Count == 0)
        {
            response = EasyArgs.Build().Orange("Error!").Space().Orange("No arguments.").Done() + Usage + CommandHintHelper;
            return false;
        }
        
        Player? playerToGiveCard;
        
        if (PlayerInformationHandler.Players.TryGetValue(arguments.At(0), out var userId))
            Player.TryGet(userId, out playerToGiveCard);
        
        else if (!PlayerExtensions.TryGetPlayerById(arguments.At(0), out playerToGiveCard))
        {
            response = EasyArgs.Build().Orange("Error!").Space().Orange("Invalid player name/ID.").Done() + Usage + CommandHintHelper;
            return false;
        }
        
        if (playerToGiveCard == null)
        {
            response = EasyArgs.Build().Orange("Error!").Space().Orange("Invalid player name/ID. Player is null.").Done() + Usage + CommandHintHelper;
            return false;
        }
        
        if (playerToGiveCard.IsInventoryFull)
        {
            response = EasyArgs.Build().Red("Error!").Space().Orange("Player's inventory is full.").Done() + Usage + CommandHintHelper;
            return false;
        }

        if (arguments.Count == 1)
        {
            response = EasyArgs.Build().Orange("Error!")
                .Space().Orange("No keycard type specified.")
                .NewLine().Orange("Types:")
                .Space().Blue(string.Join(", ", Enum.GetNames(typeof(KeycardType)))).Done() + Usage + CommandHintHelper;
            return false;
        }
        
        var keycardType = GetKeycardType(arguments.At(1));
        if (keycardType == null)
        {
            response = EasyArgs.Build()
                .Orange("Error!")
                .Space().Orange("Incorrect keycard type.")
                .NewLine().Orange("Keycard types:").Space().Blue(string.Join(", ", Enum.GetNames(typeof(KeycardType))))
                .NewLine()
                .Space().Orange(Usage).Done() + CommandHintHelper;
            return false;
        }
        
        switch (keycardType)
        {
            case KeycardType.KeycardCustomTaskForce:
                TaskForceKeycardHandler.TaskForceKeycardCreator(arguments, playerToGiveCard, sender);
                break;
            case KeycardType.KeycardCustomSite02:
                Site02KeycardHandler.Site02KeycardCreator(arguments, playerToGiveCard, sender);
                break;
            case KeycardType.KeycardCustomManagement: 
                ManagementKeycardHandler.ManagementKeycardCreator(arguments, playerToGiveCard, sender);
                break;
            case KeycardType.KeycardCustomMetalCase:
                MetalCaseKeycardHandler.MetalCaseKeycardCreator(arguments, playerToGiveCard, sender);
                break;
            default:
                sender.Respond($"Error! Unknown keycard type. {keycardType}" + Usage + CommandHintHelper);
                break;
        }
        return false;
    }
}