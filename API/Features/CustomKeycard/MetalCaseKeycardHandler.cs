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

public class MetalCaseKeycardHandler
{
    private const string CommandHintHelper = "\nInstructions ^"; // to prevent the little ingame command hint thing from not showing our epic instructions
    
    private static readonly string MetalCaseCommandUsage = EasyArgs.Build()
        .NewLine()
        .CmdArguments("ckeycard " +
                      "plrNameOrId " + // arg0
                      "man " + // arg1
                      "itemName " + // arg2
                      "containmentLevel " + // arg3
                      "armoryLevel " + // arg4
                      "adminLevel " + // arg5
                      "permissionColor " + // arg6
                      "keycardColor " + // arg7
                      "holderName " + // arg8
                      "label " + // arg9
                      "labelColor " + // arg10
                      "wearLevel " + // arg11
                      "serialLabel " // arg12
        )
        .Done() + CommandHintHelper;
    
    public static bool MetalCaseKeycardCreator(ArraySegment<string> uncutArguments, ExPlayer playerToGiveCard, ICommandSender commandSender)
    {
        if (uncutArguments.Count < 3) // arg2, itemName
        {
            commandSender.Respond(EasyArgs.Build()
                .Red("Error!")
                .Space().Orange("No item name specified!")
                .NewLine().Orange("Formatting information:").NewLine().Blue("Use underscores for spaces.").Done() + MetalCaseCommandUsage);
            return false;
        }
        
        var itemName = uncutArguments.At(2);
        
        if (uncutArguments.Count < 4) // arg3, containmentLevel
        {
            commandSender.Respond(EasyArgs.Build()
                .Red("Error!")
                .Space().Orange("No containment level specified!")
                .NewLine().Orange("Formatting information:").NewLine().Blue("Use numbers 0-5.").Done() + MetalCaseCommandUsage);
            return false;
        }
        
        var argument3 = uncutArguments.At(3);
        
        if (!argument3.GetCustomKeycardLevel(out var containmentLevel, out var cKeycardContainmentLevel) || containmentLevel == null || cKeycardContainmentLevel == null)
        {
            commandSender.Respond(EasyArgs.Build()
                .Red($"Error!")
                .Space().Orange($"Invalid containment level! \"{argument3}\"")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use numbers 0-5.").Done() + MetalCaseCommandUsage);
            return false;
        }
        
        if (uncutArguments.Count < 5) // arg4, armoryLevel
        {
            commandSender.Respond(EasyArgs.Build()
                .Red("Error!")
                .Space().Orange("No armory level specified!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use numbers 0-5.").Done() + MetalCaseCommandUsage);
            return false;
        }
        
        var argument4 = uncutArguments.At(4);
        
        if (!argument4.GetCustomKeycardLevel(out var armoryLevel, out var cKeycardArmoryLevel) || armoryLevel == null || cKeycardArmoryLevel == null)
        {
            commandSender.Respond(EasyArgs.Build()
                .Red($"Error!")
                .Space().Orange($"Invalid armory level! \"{argument4}\"")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use numbers 0-5.").Done() + MetalCaseCommandUsage);
            return false;
        }
        
        if (uncutArguments.Count < 6) // arg5, adminLevel
        {
            commandSender.Respond(EasyArgs.Build().Red("Error!").Space().Orange("No administration/engineering level specified!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use numbers 0-5.").Done() + MetalCaseCommandUsage);
            return false;
        }
        
        var argument5 = uncutArguments.At(5);
        
        if (!argument5.GetCustomKeycardLevel(out var adminLevel, out var cKeycardAdminLevel) || adminLevel == null || cKeycardAdminLevel == null)
        {
            commandSender.Respond(EasyArgs.Build()
                .Red($"Error!")
                .Space().Orange($"Invalid admin level! \"{argument5}\"")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use numbers 0-5.").Done() + MetalCaseCommandUsage);
            return false;
        }
        
        if (uncutArguments.Count < 7) // arg6, permissionColor
        {
            commandSender.Respond(EasyArgs.Build()
                .Red("Error!")
                .Space().Orange("No permission color specified!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("You can use color names such as \"blue\", alongside raw HTML tags such as \"<noparse>#</noparse>000000\" (note the hashtag!)")
                .Done() + MetalCaseCommandUsage);
            return false;
        }
        
        var argument6 = uncutArguments.At(6); // permissionsColor
        
        if (!ColorUtility.TryParseHtmlString(argument6, out var permissionsColor))
        {
            commandSender.Respond(EasyArgs
                .Build()
                .Red("Error!")
                .Space().Orange("Invalid color.")
                .Space().Orange($"PermissionsColor cannot be \"{argument6}\"!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("You can use color names such as \"blue\", alongside raw HTML tags such as \"<noparse>#</noparse>000000\" (note the hashtag!)")
                .Done() + MetalCaseCommandUsage);
            return false;
        }
        
        if (uncutArguments.Count < 8) // arg7, keycardColor
        {
            commandSender.Respond(EasyArgs
                .Build()
                .Red("Error!")
                .Space().Orange("No keycard color specified!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("You can use color names such as \"blue\", alongside raw HTML tags such as \"<noparse>#</noparse>000000\" (note the hashtag!)")
                .Done() + MetalCaseCommandUsage);
            return false;
        }
        
        var argument7 = uncutArguments.At(7); // keycardColor
        
        if (!ColorUtility.TryParseHtmlString(argument7, out var keycardColor))
        {
            commandSender.Respond(EasyArgs
                .Build()
                .Red("Error!")
                .Space().Orange("Invalid color.")
                .Space().Orange($"KeycardColor cannot be \"{argument7}\"!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("You can use color names such as \"blue\", alongside raw HTML tags such as \"<noparse>#</noparse>000000\" (note the hashtag!)")
                .Done() + MetalCaseCommandUsage);
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
                    .NewLine().Blue("Use \"_\" for spaces! To use underscores, use \"\\_\"!").Done() + MetalCaseCommandUsage);
                return false;
            // arg9, label
            case < 10:
                commandSender.Respond(EasyArgs.Build()
                    .Red("Error!")
                    .Space().Orange("No label specified!")
                    .NewLine().Orange("Formatting information:")
                    .NewLine().Blue("Use \"_\" for spaces! To use underscores, use \"\\_\"!").Done() + MetalCaseCommandUsage);
                return false;
        }
        
        var holderName = uncutArguments.At(8).HandleTightInput();
        var label = uncutArguments.At(9).HandleTightInput();

        if (uncutArguments.Count < 11) // arg10, labelColor
        {
            commandSender.Respond(EasyArgs
                .Build()
                .Red("Error!")
                .Space().Orange("No label color specified!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("You can use color names such as \"blue\", alongside raw HTML tags such as \"<noparse>#</noparse>000000\" (note the hashtag!)")
                .Done() + MetalCaseCommandUsage);
            return false;
        }
        
        var argument10 = uncutArguments.At(10);
        
        if (!ColorUtility.TryParseHtmlString(argument10, out var labelColor))
        {
            commandSender.Respond(EasyArgs
                .Build()
                .Red("Error!")
                .Space().Orange("Invalid color.")
                .Space().Orange($"LabelColor cannot be \"{argument7}\"!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("You can use color names such as \"blue\", alongside raw HTML tags such as \"<noparse>#</noparse>000000\" (note the hashtag!)")
                .Done() + MetalCaseCommandUsage);
            return false;
        }
        
        if (uncutArguments.Count < 12) // arg11, wearLevel
        {
            commandSender.Respond(EasyArgs
                .Build()
                .Red("Error!")
                .Space().Orange("No wear level specified!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use any number 0 to 255. No decimals.")
                .Done() + MetalCaseCommandUsage);
            return false;
        }
        
        var argument11 = uncutArguments.At(11);
        
        if (!byte.TryParse(argument11, out var wearLevel))
        {
            commandSender.Respond(EasyArgs
                .Build()
                .Red("Error!")
                .Space().Orange("No wear level specified!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use any number 0 to 255. No decimals.")
                .Done() + MetalCaseCommandUsage);
            return false;
        }
        
        if (uncutArguments.Count < 13) // arg12, serialLabel
        {
            commandSender.Respond(EasyArgs
                .Build()
                .Red("Error!")
                .Space().Orange("No serial label specified!")
                .NewLine().Orange("Formatting information:")
                .NewLine().Blue("Use \"_\" for spaces! To use underscores, use \"\\_\"!")
                .Done() + MetalCaseCommandUsage);
            return false;
        }
        
        var serialLabel = uncutArguments.At(12).HandleTightInput();
        
        var metalKeycard = KeycardItem.CreateCustomKeycardMetal(
            playerToGiveCard,
            itemName,
            holderName,
            label,
            new KeycardLevels((int)cKeycardContainmentLevel, (int)cKeycardArmoryLevel, (int)cKeycardAdminLevel),
            keycardColor,
            permissionsColor,
            labelColor,
            wearLevel,
            serialLabel
        );
        
        if (metalKeycard == null)
        {
            commandSender.Respond(
                EasyArgs.Build()
                    .Red("Error!")
                    .Space().Orange("ManagementKeycard failed to create.")
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
                + $"{(playerToGiveCard.IsScp ? "<color=green>True</color>" : "<color=red>False</color>")}" + MetalCaseCommandUsage);
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
        
        var grppKeycard = new KeycardHandler.GrppKeycard(holderName, label, KeycardHandler.GetKeycardChecksum(), integers.GetHighestInteger(), allowedSubLevels);
        KeycardHandler.Instance.GiveCustomKeycard(playerToGiveCard, metalKeycard, grppKeycard, commandSender);
        
        commandSender.Respond(EasyArgs.Build().Green("Success!").Space().Orange($"Created a custom card, and gave it to {playerToGiveCard.Nickname}!").Done());
        return true;
    }
}