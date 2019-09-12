# pro079
A plugin to enhance playing 079 by giving it some fancy commands to play with, becuase the whole spanish community agreed that playing SCP-079 is boring, this plugin gives him more stuff to not be bored to literal death.

# Discord
## I now have a Discord where you can get pinged and all of that each time I do something cool, or where you can publish your own commands to: https://discord.gg/cBWuSrR

# Configs

*Wiki with **all** the configurations (you should check this one):* https://github.com/RogerFK/pro079/wiki
**The only feature disabled by default is `p079_chaos`. If you want to use it, you have to enable it.**

This plugin take many configs. The basic ones, that work between features, are as follows:

| Config Option | Value Type | Default Value | Description |
|:-----------------------:|:----------:|:-------------------------:|:------------------------------:|
| pro079_enable | boolean | True | Enables/Disables this plugin |
| pro079_feature_enable | boolean | True | Enables X feature |
| pro079_feature_cooldown | float | (Depends on each command) | Cooldown for the command |
| pro079_feature_level | float | (Depends on each command) | Minimum level for that feature |
| pro079_feature_cost | float | (Depends on each command) | Cost for that feature |

# Translations
Just place one of the translations into your sm_translations folder and you're good to go! Defaults to english, this plugin will autogenerate the default translation if you're missing any

# API Guide

This plugin features an API with which you can add new commands/ultimates via plugins (similar to gamemodes/itemmanager)
To do so, make sure to reference 0Pro079.dll beforehand, then take a reference from the commands that have already been made inside the `Modules` folder and guide yourself from there. I recommend looking at the Pro079.Chaos command, since it's the most simple one (hint: replace Chaos with Serpent's Hand in case you wanted to fake the Serpent's Hand coming but you couldn't before, but for starters, make sure to change the properties of the project and the id of the plugin and all of that beforehand or else you will not be able to run it)
