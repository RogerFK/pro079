﻿using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace Pro079
{
	[PluginDetails(
        author = "RogerFK",
        name = "Pro 079 Core",
        description = "Core plugin for Pro-079.",
        id = "rogerfk.pro079",
        version = "2.0",
        SmodMajor = 3,
        SmodMinor = 5,
        SmodRevision = 0,
		langFile = "pro079-core"
        )]

    public class Pro079 : Plugin
    {
		/// <summary>
		/// Manager that contains all commands and useful functions
		/// </summary>
		public static Manager Manager { private set; get; }
		/// <summary>
		/// User defined configurations and language options
		/// </summary>
		public static Config079 Configs { private set; get; }
		// Config options
		// Public options
		[ConfigOption]
		public readonly bool enable = true;
		[ConfigOption]
		public readonly bool suicide = true;
		[ConfigOption]
		public readonly bool ult = true;
		[ConfigOption]
		public readonly bool tips = true;

		// Core-only options
		[ConfigOption]
		public readonly float cassieCooldown = 30f;
		[ConfigOption]
		public readonly bool spawnBroadcast = true;
		[ConfigOption]
		public readonly int ultLevel = 4;

		// Language options
		// Public translations
		[LangOption]
		public readonly string disabled = "This command is disabled.";
		[LangOption]
		public readonly string level = "level $lvl";
		[LangOption]
		public readonly string energy = "$ap AP";
		[LangOption]
		public readonly string lowlevel = "Your level is too low (you need $min)";
		[LangOption]
		public readonly string lowmana = "Not enough AP (you need $min)";
		[LangOption]
		public readonly string success = "Command successfully launched";
		[LangOption]
		public readonly string ultlaunched = "Ultimate successfully used.";

		// Core-only
		[LangOption]
		public readonly string basicHelp = "<b>.079</b> - Displays this info message.";
		[LangOption]
		public readonly string suicidehelp = "Overcharges the generators to die when you're alone";
		[LangOption]
		public readonly string ulthelp = "Displays info about ultimates";
		[LangOption]
		public readonly string tipshelp = "Tips about SCP-079 and stuff to take into account";
		[LangOption]
		public readonly string ultdown = "You must wait $cds before using ultimates again.";
		[LangOption]
		public readonly string cooldown = "You have to wait $cds before using this command again";
		[LangOption]
		public readonly string cassieOnCooldown = "Wait $cds before using a command that requires CASSIE (the announcer)";
		[LangOption]
		public readonly string notscp079 = "You aren't SCP-079!";
		[LangOption]
		public readonly string broadcastMsg = "<color=#85ff4c>Press ` to open up the console and use additional commands.</color>";
		[LangOption("tips")] // sry :(
		public readonly string tipsMsg = @"TAB (above Caps Lock): opens up the map.\nSpacebar: switches the camera view from the normal mode to the FPS one (with the white dot).\nWASD: move to the camera the plugin says\nTo get out of the Heavy Containment Zone, go to the elevetor (with TAB) and click the floor's white rectangle, or to the checkpoint and press WASD to get out\nAdditionally, this plugins provide extra commands by typing .079 in the console";
		[LangOption]
		public readonly string unknowncmd = "Unknown command. Type \".079\" for help.";
		[LangOption]
		public readonly string cassieready = "<color=#85ff4c>Announcer (CASSIE) commands ready</color>";
		[LangOption]
		public readonly string ultready = "<color=#85ff4c>Ultimates ready</color>";
		[LangOption]
		public readonly string cantsuicide = "You can't suicide when there's other SCP's remaining";
		[LangOption]
		public readonly string ultlocked = "To use an ultimate, you need level $lvl";
		[LangOption]
		public readonly string kys = "<color=#AA1515>Press ` and write \".079 suicide\" to kill yourself.</color>";
		[LangOption]
		public readonly string ultusageFirstline = "Usage: .079 ultimate <name>";
		[LangOption]
		public readonly string minidisabled = "disabled";
		// Core cmds translations
		[LangOption]
		public readonly string tipscmd = "tips";
		[LangOption]
		public readonly string suicidecmd = "suicide";
		[LangOption]
		public readonly string ultcmd = "ultimate";
		public override void OnDisable()
        {
            this.Info("Pro079 Core disabled.");
        }

        public override void OnEnable()
        {
            this.Info("Pro079 Core enabled");
        }
		
        public override void Register()
        {
			this.Info("Loading Pro-079 Core configs and registering default commands...");
			AddEventHandlers(new Pro79Handlers(this));
			#region Specific configs
            #endregion

            // Translations
            string lang = "pro079-core";

			#region Cmds and Help strings
			AddTranslation(new LangSetting("newteslacmd", "tesla", lang));
			AddTranslation(new LangSetting(    "mtfcmd", "mtf", lang));
			AddTranslation(new LangSetting(    "gencmd", "gen", lang));
			AddTranslation(new LangSetting(    "scpcmd", "scp", lang));
			AddTranslation(new LangSetting(   "infocmd", "info", lang));
			AddTranslation(new LangSetting(  "chaoscmd", "chaos", lang));
			#endregion

			//"\\n1. Lights out: shuts the HCZ down for 1 minute (cooldown: 180 seconds)\\n2. Lockdown: makes humans unable to open big doors, but SCPs can open any (duration: 30 seconds, cooldown: 300 seconds)", lang));

			Manager = new Manager(this);
			Configs = new Config079(this);
		}
    }
}
