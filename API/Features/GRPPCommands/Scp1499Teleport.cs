namespace GRPP.API.Features.GRPPCommands;

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using EasyTmp;
using Items.Scp1499;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class Scp1499Teleport : ICommand
{
    public string Command => "scp1499teleport";
    public string[] Aliases => ["scp1499tp", "scp1499", "1499", "1499tp"];
    public string Description => "Teleport to SCP-1499's dimension.";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var plr = ExPlayer.Get(sender);
        if (plr == null || !plr.RemoteAdminAccess)
        {
            response = "Error! No permission!";
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(Plugin.Singleton.Config.Scp1499Map) || !Scp1499Manager.Scp1499DimensionLoaded)
        {
            response = EasyArgs.Build().Red("Error!").Space().Orange("Scp1499Map is empty!").Done();
            return false;
        }
        
        var oldPos = plr.Position;
        plr.Teleport(Plugin.Singleton.Config.Scp1499PlayerSpawnPoint);
        response = EasyArgs.Build().Green("Success!").Space().Orange($"Teleported to SCP-1499's dimension.\nPrevious Position: \"{oldPos}\"").Done();
        return true;
    }
}