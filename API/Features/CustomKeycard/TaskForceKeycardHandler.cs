namespace GRPP.API.Features.CustomKeycard;

using System;
using System.Collections.Generic;
using CommandSystem;
using EasyTmp;
using Enums;
using Extensions;
using Interactables.Interobjects.DoorUtils;
using Items;
using LabApi.Features.Wrappers;
using UnityEngine;

public static class TaskForceKeycardHandler
{
    private const string CommandHintHelper = "\nInstructions ^"; // to prevent the little ingame command hint thing from not showing our epic instructions

    private static readonly string NtfCommandUsage = EasyArgs.Build().NewLine().CmdArguments("ckeycard " +
                                                                                             "plrNameOrId " +
                                                                                             "ntf " +
                                                                                             "itemName " +
                                                                                             "containmentLevel " +
                                                                                             "armoryLevel " +
                                                                                             "adminLevel " +
                                                                                             "permissionColor " +
                                                                                             "keycardColor " +
                                                                                             "holderName " +
                                                                                             "serialLabel " +
                                                                                             "rankIndex").Done() + CommandHintHelper;
    
    public static bool TaskForceKeycardCreator(ArraySegment<string> uncutArguments, ExPlayer playerToGiveCard, ICommandSender commandSender)
    {
        if (uncutArguments.Count < 3)
        {
            commandSender.Respond(EasyArgs.Build()
                .Red("Error!")
                .Space().Orange("No item name specified!")
                .NewLine().Orange("Formatting information:").NewLine().Blue("Use underscores for spaces.").Done() + NtfCommandUsage);
            return false;
        }
        
        var itemName = uncutArguments.At(2);
        
        if (uncutArguments.Count < 4)
        {
            commandSender.Respond(EasyArgs.Build()
                .Red("Error!")
                .Space().Orange("No containment level specified!")
                .NewLine().Orange("Formatting information:").NewLine().Blue("Use numbers 0-5.").Done() + NtfCommandUsage);
            return false;
        }
        
        var argument3 = uncutArguments.At(3);
        
        if (!argument3.GetCustomKeycardLevel(out var containmentLevel, out var cKeycardContainmentLevel) || containmentLevel == null || cKeycardContainmentLevel == null)
        {
            commandSender.Respond(EasyArgs.Build()
                .Red($"Error!")
                .Space().Orange($"Invalid containment level! \"{argument3}\"")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use numbers 0-5.").Done() + NtfCommandUsage);
            return false;
        }
        
        if (uncutArguments.Count < 5)
        {
            commandSender.Respond(EasyArgs.Build()
                .Red("Error!")
                .Space().Orange("No armory level specified!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use numbers 0-5.").Done() + NtfCommandUsage);
            return false;
        }
        
        var argument4 = uncutArguments.At(4);
        
        if (!argument4.GetCustomKeycardLevel(out var armoryLevel, out var cKeycardArmoryLevel) || armoryLevel == null || cKeycardArmoryLevel == null)
        {
            commandSender.Respond(EasyArgs.Build()
                .Red($"Error!")
                .Space().Orange($"Invalid armory level! \"{argument4}\"")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use numbers 0-5.").Done() + NtfCommandUsage);
            return false;
        }
        
        if (uncutArguments.Count < 6) // arg5
        {
            commandSender.Respond(EasyArgs.Build().Red("Error!").Space().Orange("No administration/engineering level specified!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use numbers 0-5.").Done() + NtfCommandUsage);
            return false;
        }
        
        var argument5 = uncutArguments.At(5);
        
        if (!argument5.GetCustomKeycardLevel(out var adminLevel, out var cKeycardAdminLevel) || adminLevel == null || cKeycardAdminLevel == null)
        {
            commandSender.Respond(EasyArgs.Build()
                .Red($"Error!")
                .Space().Orange($"Invalid admin level! \"{argument5}\"")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use numbers 0-5.").Done() + NtfCommandUsage);
            return false;
        }
        
        switch (uncutArguments.Count)
        {
            // arg6
            case < 7:
                commandSender.Respond(EasyArgs.Build().Red("Error!")
                                          .Space().Orange("No permission color specified!")
                                          .NewLine().Orange("Formatting information:")
                                          .NewLine()
                                          .Blue("You can use color names such as \"blue\", alongside raw HTML tags such as \"<noparse>#</noparse>000000\" (note the hashtag!)")
                                          .Done() +
                                      NtfCommandUsage);
                return false;
            // arg7
            case < 8:
                commandSender.Respond(EasyArgs.Build().Red("Error!")
                                          .Space().Orange("No keycard color specified!")
                                          .NewLine().Orange("Formatting information:")
                                          .NewLine()
                                          .Blue("You can use color names such as \"blue\", alongside raw HTML tags such as \"<noparse>#</noparse>000000\" (note the hashtag!)")
                                          .Done() +
                                      NtfCommandUsage);
                return false;
        }
        
        var argument6 = uncutArguments.At(6);
        var argument7 = uncutArguments.At(7);
        
        if (!ColorUtility.TryParseHtmlString(argument6, out var permissionsColor) || !ColorUtility.TryParseHtmlString(argument7, out var keycardColor))
        {
            commandSender.Respond(EasyArgs.Build()
                                      .Red("Error!")
                                      .Space().Orange("Invalid color.")
                                      .Space().Orange($"{(!ColorUtility.TryParseHtmlString(argument6, out _)
                                          ? $"PermissionsColor cannot be {argument6}"
                                          : $"KeycardColor cannot be {argument7}")}!")
                                      .NewLine().Orange("Formatting information:")
                                      .NewLine()
                                      .Blue("You can use color names such as \"blue\", alongside raw HTML tags such as \"<noparse>#</noparse>000000\" (note the hashtag!)").Done() +
                                  NtfCommandUsage);
            return false;
        }
        
        switch (uncutArguments.Count)
        {
            // arg8, holderName
            case < 9:
                commandSender.Respond(EasyArgs.Build()
                    .Red("Error!")
                    .Space().Orange("No holder name specified!")
                    .NewLine().Orange("Formatting information:")
                    .NewLine().Blue("Use \"_\" for spaces! To use underscores, use \"\\_\"!").Done() + NtfCommandUsage);
                return false;
            // arg9, serialLabel
            case < 10:
                commandSender.Respond(EasyArgs.Build()
                    .Red("Error!")
                    .Space().Orange("No serial label specified!")
                    .NewLine().Orange("Formatting information:")
                    .NewLine().Blue("Use \"_\" for spaces! To use underscores, use \"\\_\"!").Done() + NtfCommandUsage);
                return false;
            // arg10, rankIndex
            case < 11:
                commandSender.Respond(EasyArgs.Build().Red("Error!")
                    .Space().Orange("No rank index specified!")
                    .NewLine().Orange("Formatting information:")
                    .NewLine().Blue("Use any number -2147483647 to 2147483647. No decimals.").Done() + NtfCommandUsage);
                // -2147483648 causes an exception and adds a ghost item to the inventory - while technically valid, not here - since Math.Abs is used by SL regardless.
                return false;
        }
        
        var holderName = uncutArguments.At(8).HandleTightInput();
        var serialLabel = uncutArguments.At(9).HandleTightInput();
        var argument10 = uncutArguments.At(10);
        
        if (!int.TryParse(argument10, out var rankIndex))
        {
            commandSender.Respond($"Error! Invalid rank index. \"{argument10}\"");
            return false;
        }
        
        // ckeycard gets the absolute value of rankIndex regardless, and it's not used by grppkeycard. "-2147483648", or int.MinValue - throws an exception when being Math.Abs'd.
        if (rankIndex == int.MinValue)
            rankIndex = int.MaxValue;
        
        var taskForceKeycard = KeycardItem.CreateCustomKeycardTaskForce(
            playerToGiveCard,
            itemName,
            holderName,
            new KeycardLevels((int)cKeycardContainmentLevel, (int)cKeycardArmoryLevel, (int)cKeycardAdminLevel),
            keycardColor,
            permissionsColor,
            serialLabel,
            rankIndex
        );
        
        if (taskForceKeycard == null)
        {
            commandSender.Respond(
                EasyArgs.Build()
                    .Red("Error!")
                    .Space().Orange("TaskForceKeycard failed to create.")
                    .NewLine().Blue("IsPlayerInventoryFull:")
                    .Space()
                    .Done()
                + $"{(playerToGiveCard.IsInventoryFull ? "<color=green>True</color>" : "<color=red>False</color>")}"
                + EasyArgs.Build()
                    .NewLine().Blue("IsAlive:")
                    .Space()
                    .Done()
                + $"{(playerToGiveCard.IsAlive ? "<color=green>True</color>" : "<color=red>False</color>")}"
                + EasyArgs.Build()
                    .NewLine().Blue("IsSCP:")
                    .Space()
                    .Done()
                + $"{(playerToGiveCard.IsScp ? "<color=green>True</color>" : "<color=red>False</color>")}" + NtfCommandUsage);
            return false;
        }
        
        IEnumerable<int> integers = [(int)containmentLevel, (int)armoryLevel, (int)adminLevel];
        List<KeycardSubLevels> allowedSubLevels = [];
        
        if (containmentLevel != 0)
            allowedSubLevels.Add(KeycardSubLevels.Containment);
        
        if (armoryLevel != 0)
            allowedSubLevels.Add(KeycardSubLevels.Security);
        
        if (adminLevel != 0)
            allowedSubLevels.Add(KeycardSubLevels.Engineering);
        
        var grppKeycard = new KeycardHandler.GrppKeycard(holderName, serialLabel, KeycardHandler.GetKeycardChecksum(), integers.GetHighestInteger(), allowedSubLevels);
        KeycardHandler.Instance.GiveCustomKeycard(playerToGiveCard, taskForceKeycard, grppKeycard, commandSender);
        
        commandSender.Respond(EasyArgs.Build().Green("Success!").Space().Orange($"Created a custom card, and gave it to {playerToGiveCard.Nickname}!").Done());
        return true;
    }
}