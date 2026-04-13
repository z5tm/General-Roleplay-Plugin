# General Roleplay Plugin (GRPP)

### Join the[ discord ](https://discord.gg/Hw5UTGAJcn)for further information and support.

## Quick-Start guide:

### Installation
- Install ProjectMER into your `LabAPI/plugins/[port]` (or `global`). You can find the latest release [here](https://github.com/Michal78900/ProjectMER/releases/latest). [^1]
- Install EXILED. There is a VERY quick installation guide [here](https://exmod-team.github.io/EXILED/articles/installation/manual.html)! [^2]
### Permissions Overview
| Permission               | Related Commands                                              | Description                                                                                                                                                                              |
|--------------------------|---------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| grpp.bypassrestrict      | Many, check the discord.                                      | This permission allows you to bypass "restrictive mode" which is optionally enabled on RP start.<br/>(Note: Restrictive mode can be permitted in the configuration.)                     |
| grpp.lobby               | `uselobby`/`endlobby`,<br/>`startroleplay`/`endroleplay`<br/> | This permission permits the usage of roleplay-related commands. <br/>For more information, please join the discord.                                                                      |
| grpp.taser               | `tasermod`                                                    | This permission permits the usage of the tasermod command. <br/>More information in the discord.                                                                                         |
| grpp.au                  | `au`<br/>`scombataudio`                                       | This permission permits the usage of the audio player-related commands. <br/>More information in the discord.                                                                            |
| grpp.restrictpermissions | `startroleplay [sitenum] [1=yes]`<br/>`rp1 [sitenum] [1=yes]` | Permits the usage of restrictive mode, which limits other staff's permissions to run certain commands. <br/>Shouldn't affect the main hoster, but could due to **early implementation**. |

[//]: # (| grpp.fixme               |                                                               |                                                                                                                                                                                          |)

## Quick-Build guide: 
---
Requirements: 

###### not sure if we can distribute these so i'll just link to them

[AudioPlayer](https://github.com/Antoniofo/AudioPlayer/releases/latest) (put in ./lib/)

[ProjectMER](https://github.com/Michal78900/ProjectMER/releases/latest) (put in ./lib/)

[Mirror-Publicized and every other SL dep](https://github.com/Jesus-QC/SecretLabDependenciesBuilder/releases/latest)  (copy Mirror-Publicized.dll into ./lib/ - i think, also more information on SL references in the wiki)

It is HEAVILY recommended to use a set seed, but weirdly not a requirement.

# Permissions:
- grpp.lobby (Enable/Disable/Reuse lobby)
- grpp.restrictpermissions (Permits staff to restrict permissions of other hosters during a roleplay. A full list of what is restricted will be provided as soon as this feature is complete.)
- grpp.bypassrestrict (Bypass set restrictions.)
- grpp.taser (Enables modification to the tasers, for now this only permits modification of the chances for the taser to give cardiac arrest, which by default is 1/100.)

# NOTES:
- We are attempting to use Log.Debug("") as much as possible now - so if you need assistance, feel free to enable the EXILED debug mode beforehand and our plugin will log as much as we've set up.

---
**Attribution Requirements for server owners/developers using Site-12 Development Team's code or their pre-built plugin:**
- You are required to put the attribution name "Site-12 Development Team" in your **server info**
- You cannot attempt to hide the attribution in your **server info**

[^1]: The installation guide for ProjectMER is [here](https://github.com/Michal78900/ProjectMER/wiki/Installation)

[^2]: If you need assistance with installing EXILED, join EXILED's discord [on their repo](https://github.com/ExMod-Team/EXILED/)!