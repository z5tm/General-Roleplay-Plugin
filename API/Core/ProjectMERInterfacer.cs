namespace GRPP.API.Core;

using Attributes;
using LabApi.Events.Handlers;
using Exiled.API.Features.Doors;
using System.Collections.Generic;

public class ProjectMERInterfacer
{
    public static List<Door> MerDoors = [];
    
    [OnPluginEnabled]
    public static void OnPluginEnabled()
    {
        ServerEvents.WaitingForPlayers += Initialize;
    }
    [OnPluginDisabled]
    public static void OnPluginDisabled()
    {
        Deinitialize();
        ServerEvents.WaitingForPlayers -= Initialize;
    }
    
    public static void Initialize()
    {
        // if (!Plugin.Singleton.Config.Debug)
            // return;
        
        
        // var iterator = 0;
        // foreach (var door in Door.List.Where(door => door != null)/*.Where(door => door.Name.Contains("MER"))*/)
        // {
            // iterator++;
            // Log.Info($"Door caught! {door.Nametag?._nametag ?? "{{{Null!}}}"} <-- NameTag");
            // Log.Info($"Door caught! {door.Name} <-- Name");
            
            // door?.PlaySound(DoorBeepType.InteractionAllowed);
            // door?.PlaySound(DoorBeepType.InteractionDenied);
            // door?.PlaySound(DoorBeepType.LockBypassDenied);
            // door?.PlaySound(DoorBeepType.PermissionDenied);
        // }
        // Log.Info($"Door count: {iterator}");
        // iterator = 0;
        // foreach (var door in Door.List.Where(door => door == null))
        // {
            // iterator++;
            // Log.Error("Null door in Door.List.");
        // }
        // Log.Info($"Null door count: {iterator}");
    }
    public static void Deinitialize()
    {
        
    }
}