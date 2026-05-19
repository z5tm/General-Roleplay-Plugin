namespace GRPP.API.Features.GRPPCommands;

using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using GRPP.API.Attributes;

public class InfiniteRadio
{
    [OnPluginEnabled]
    private static void Init()
    {
        PlayerHandlers.UsingRadioBattery += Radio;
        Item.UsingRadioPickupBattery += PickupRadio;
    }
    private static void PickupRadio(UsingRadioPickupBatteryEventArgs ev) => ev.IsAllowed = false;
    private static void Radio(UsingRadioBatteryEventArgs ev) => ev.IsAllowed = false;
}