# General Roleplay Plugin (GRPP)

## Quick-Start guide:
WIP, DM @z5tm for now

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
