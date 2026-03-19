namespace GRPP;

using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

public class Config : IConfig
{
    [Description("Set this to true after configuration has been looked through.")]
    public bool ConfigurationComplete { get; set; } = false;

    [Description("true = Plugin Enabled, false = Plugin Disabled.")]
    public bool IsEnabled { get; set; } = true;

    [Description("Should debug logs be enabled? (Unused. EXILED debug mode works for now, but this will be added alongside it soon.")]
    public bool Debug { get; set; } = false;
    [Description("Where players spawn when lobby is enabled. `xf`, `0f`/`2921f` format. Request data in-game through the RA to grab this. The `f` is appended on.")]
    public float LobbySpawnLocationX { get; set; } = 0f;
    public float LobbySpawnLocationY { get; set; } = 0f;
    public float LobbySpawnLocationZ { get; set; } = 0f;

    [Description("Lobby Schematic Name")]
    public string LobbySchematic { get; set; } = "exampleSchematic";

    [Description("Spawn location when picking a role other than Class-D. Can be `x,y,z` i.e. `0,0,0`")] 
    // remind me to add the option to change spawn positions per-role and for classd-specific
    public float PlayerSpawnLocationX { get; set; } = 0f;
    public float PlayerSpawnLocationY { get; set; } = 0f;
    public float PlayerSpawnLocationZ { get; set; } = 0f;

    [Description("List of Departments, i.e. Security, Research")]
    public List<string> Departments { get; set; } = [
    "SetMeUp1",
    "SetMeUpNow"
    ]; // need to make this shit more user-friendly. I shall soon.

    [Description("Scom Word Blacklist")]
    public List<string> BlackList { get; set; } = [];

    [Description("Whether the WeightSystem should be on or off..")]
    public bool WeightSystem { get; set; } = false;
    
    [Description("Experiemntal features toggle. (UNIMPLEMENTED)")]
    public bool Experimental { get; set; } = false;

    [Description("Discord Webhook link for Department Logs")]
    public string URL { get; set; } = "Example URL";

    [Description("Leave empty to allow all IPs on the webserver.")]
    public List<string> AllowedIPs { get; set; } = [];
    [Description("Site number/name. Shows up on keycards as `Site-number`/`Site-Name`! Can be overriden by hoster, by using `rp1 sitenumber`. Can be negative!")] public string SiteName { get; set; } = "22";
    // Remind me to make this modular, where it detects a number and if there's more than a number it removes the Site, but if there's just numbers it appends Site-
    [Description("Should the main hoster be allowed to restrict permissions of other hosters? (BETA, currently unimplemented)")] public bool RestrictiveMode { get; set; } = false;
}
