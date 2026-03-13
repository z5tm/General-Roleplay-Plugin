namespace Site12.API.Features.Other;

using System;
using CommandSystem;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Extensions;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class Warp : ICommand
{
    public string Command => "warp";

    public string[] Aliases => ["wrp"];

    public string Description => "Toggles invisibility, and explodes a ghostbulb that has no effect.";


    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckRemoteAdmin(out response))
            return false;

        var player = ExPlayer.Get(sender);

        if (player.IsEffectActive<Invisible>())
            player.DisableEffect<Invisible>();
        else
            player.EnableEffect<Invisible>();
        //player.EnableEffect<Fade>(); ( i wanna do smt with this later )

        player.ExplodeEffect(ProjectileType.Scp2176); // doesn't actually affect the room! how cool!

        response = "<color=green>Manipulating reality..</color>";
        return true;
    }
}