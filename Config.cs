namespace GRPP;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

public class Config : IConfig
{
    [Description("Set this to true after configuration has been looked through.")]
    public bool ConfigurationComplete { get; set; } = false;

    [Description("true = Plugin Enabled, false = Plugin Disabled.")]
    public bool IsEnabled { get; set; } = true;

    [Description("Should debug logs be enabled? (USED!!!)")]
    public bool Debug { get; set; } = false;
    [Description("Where players spawn when lobby is enabled. `xf`, `0f`/`2921f` format. Request data in-game through the RA to grab this. The `f` is appended on.")]
    public float LobbySpawnLocationX { get; set; } = 0f;
    public float LobbySpawnLocationY { get; set; } = 0f;
    public float LobbySpawnLocationZ { get; set; } = 0f;

    [Description("The name of the lobby schematic. Use nothing to just load into lobby!")]
    public string LobbySchematic { get; set; } = "";

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

    [Description("Scom Word blocklist")]
    public List<string> Blocklist { get; set; } = [];

    [Description("Whether the WeightSystem should be on or off.")]
    public bool WeightSystem { get; set; } = false;
    
    // [Description("Experiemntal features toggle. (UNIMPLEMENTED)")]
    // public bool Experimental { get; set; } = false;

    [Description("Discord Webhook link for Department Logs")]
    public string URL { get; set; } = "";

    // [Description("Leave empty to allow all IPs on the webserver.")]
    // public List<string> AllowedIPs { get; set; } = [];
    [Description("Site number/name. Shows up on keycards as `Site-number`/`Site-Name`! Can be overriden by hoster, by using `rp1 sitenumber`. Can be negative!")] public string SiteName { get; set; } = "22";
    // Remind me to make this modular, where it detects a number and if there's more than a number it removes the Site, but if there's just numbers it appends Site-
    [Description("Should the main hoster be allowed to restrict permissions of other hosters? (BETA, currently being implemented!)")] public bool RestrictiveMode { get; set; } = false;
    [Description("This is optional. This is for a discord webhook URL, and it sends RP information into the channel, such as start and end time.")] public string RPWebHook { get; set; } = "";

    [Description("Whether to enable showing TPS on the RP webhook.")]
    public bool WebhookTpsEnabled { get; set; } = true;
    [Description("Whether to enable showing player count on the RP webhook.")]
    public bool WebhookPlayerCountEnabled { get; set; } = true;

    [Description("The color to use for the lobby (RP) webhook.")]
    public UInt32 WebhookLobbyColor { get; set; } = 1752220;
    [Description("The color to use for the RP started (RP) webhook.")]
    public UInt32 WebhookRPColor { get; set; } = 7419530;
    // remind me to make this applicable for other things eventually, if that's even required
    [Description("Allow anyone with RA to modify portions of their rank.")]
    public bool RankModEnabled { get; set; } = false;
    [Description("Allow anyone with RA to modify the color of their rank.")]
    public bool RankColorEnabled { get; set; } = false;
    [Description("Allow anyone with RA to modify the words before the name of their rank.")]
    public bool RankNameEnabled { get; set; } = false;
}
