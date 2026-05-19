namespace GRPP.API.Features.GRPPCommands;

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Exiled.Permissions.Extensions;
using LabApi.Features.Wrappers;
using PlayerRoles;

[CommandHandler(typeof(ClientCommandHandler))]
public class Fix : ICommand
{
    public string Command => "fix";
    public string[] Aliases => ["fixme", "audiobugfix"];
    public string Description => "Temporary fix for audio bug.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        // IN TESTING. currently limited to users with grpp.fixme permission.
        var plr = ExPlayer.Get(sender);
        var labapiplr = Player.Get(sender);

        if (plr == null || labapiplr == null)
        {
            response = string.Empty;
            return false;
        }

        if (!plr.CheckPermission("grpp.fixme"))
        {
            response = "Unimplemented.";
            return false;
        }

        if (plr.IsSpeaking)
        {
            response = "Please do not speak during the process.";
            return false;
        }

        var cinfo = plr.CustomInfo;
        var name = plr.CustomName; // displaynickname?? idk what it is, could be able to make name look diff for the user and all other players or smt
        var health = plr.Health;
        var maxHealth = plr.MaxHealth;
        var artificialHealth = plr.ArtificialHealth;
        var maxArtificialHealth = plr.MaxArtificialHealth;
        var inv = plr.Inventory.UserInventory.Items;
        var role = labapiplr.Role;


        labapiplr.SetRole(role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
        plr.Inventory.UserInventory.Items = inv;
        plr.MaxHealth = maxHealth;
        plr.Health = health;
        plr.CustomName = name;
        plr.CustomInfo = cinfo;
        plr.MaxArtificialHealth = maxArtificialHealth;
        plr.ArtificialHealth = artificialHealth;
        // plr.UserGroup.

        response = "tried.";
        return true;
    }
}