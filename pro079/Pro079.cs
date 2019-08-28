using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace pro079
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
		langFile = "pro079"
        )]

    public class Pro079 : Plugin
    {
		// Language options
		[LangOption]
		public static readonly string basicHelp = "<b>.079</b> - Displays this info.";
		[LangOption]
		public static readonly string disabled = "This command is disabled.";
		[LangOption]
		public static readonly string cooldown = "You have to wait $cds before using this command again";
		/*AddTranslation(new LangSetting("cooldown", "You have to wait $cds before using this command again", lang));
			AddTranslation(new LangSetting("cooldowncassie", "Wait $cds before using a command that requires CASSIE (the announcer)", lang));
			*/
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

			AddConfig(new ConfigSetting("p079_enable", true, true, "Enables/disables Pro-079"));
			AddConfig(new ConfigSetting("p079_broadcast_enable", true, true, "Enables a broadcast at the start of the round."));

			#region Specific configs

            // Ultimates not configurable yet. Will use a Dictionary for Ult IDs in the future with it's own Ultimate079 class
            // If you want to help with this, at least point me towards the best way to read other .dlls for custom made Ultimates or to use piping
            AddConfig(new ConfigSetting("p079_ult", true, true, "Enables/disables Pro-079's fake Chaos messages"));
			
			AddConfig(new ConfigSetting("p079_suicide", true, true, "Enables/disables 079 to suicide when he is alone. Gives a shit about lone-079, feel free to use it"));

            // From now on, they are just about C.A.S.S.I.E. announcements
            AddConfig(new ConfigSetting("p079_cassie_cooldown", 30f, true, "How many seconds will C.A.S.S.I.E commands be generally disabled for after each command"));

			AddConfig(new ConfigSetting("p079_tips", true, true, "Enables/disables Pro-079's fake SCP death messages"));
            #endregion

            // Translations
            string lang = "pro079";

			AddTranslation(new LangSetting("broadcast_msg", "<color=#85ff4c>Press ` to open up the console and use additional commands.</color>", lang));
			AddTranslation(new LangSetting("help", "<b>.079</b> - Displays this info", lang));

            AddTranslation(new LangSetting("tips", @"TAB (above Caps Lock): opens up the map.\nSpacebar: switches the camera view from the normal mode to the FPS one (with the white dot).\nWASD: move to the camera the plugin says\nTo get out of the Heavy Containment Zone, go to the elevetor (with TAB) and click the floor's white rectangle, or to the checkpoint and press WASD to get out\nAdditionally, this plugins provides extra commands by typing .079 in the console", lang));

            AddTranslation(new LangSetting("level", "level $lvl", lang));
			AddTranslation(new LangSetting("energy", "$ap AP", lang));

			AddTranslation(new LangSetting("unknowncmd", "Unknown command. Type \".079\" for help.", lang));
			AddTranslation(new LangSetting("bugwarn", "If you find any bug, tell RogerFK#3679 on Discord", lang));

			#region Cmds and Help strings
			AddTranslation(new LangSetting("newteslacmd", "tesla", lang));
			AddTranslation(new LangSetting(    "mtfcmd", "mtf", lang));
			AddTranslation(new LangSetting(    "gencmd", "gen", lang));
			AddTranslation(new LangSetting(    "scpcmd", "scp", lang));
			AddTranslation(new LangSetting(   "infocmd", "info", lang));
			AddTranslation(new LangSetting("suicidecmd", "suicide", lang));
			AddTranslation(new LangSetting(    "ultcmd", "ultimate", lang));
			AddTranslation(new LangSetting(  "chaoscmd", "chaos", lang));
			AddTranslation(new LangSetting(   "tipscmd", "tips", lang));

			AddTranslation(new LangSetting("suicidehelp", "<b>.079 suicide</b> - Overcharges the generators to die when you're alone", lang));
			AddTranslation(new LangSetting(    "ulthelp", "<b>.079 ultimate</b> - Displays info about ultimates", lang));
			AddTranslation(new LangSetting(   "tipshelp", "<b>.079 tips</b> - Tips about SCP-079 and stuff to take into account", lang));

			#endregion

			AddTranslation(new LangSetting("notscp079", "You aren't SCP-079!", lang));

			AddTranslation(new LangSetting("cassieready", "<color=#85ff4c>Announcer (CASSIE) commands ready</color>", lang));
			AddTranslation(new LangSetting("ultready", "<color=#85ff4c>Ultimates ready</color>", lang));
			AddTranslation(new LangSetting("success", "Command successfully launched", lang));

			AddTranslation(new LangSetting("lowlevel", "Your level is too low (you need $min)", lang));
			AddTranslation(new LangSetting("lowmana", "Not enough AP (you need  $min)", lang));

			AddTranslation(new LangSetting("disabled", "This command is disabled.", lang));

			AddTranslation(new LangSetting("cooldown", "You have to wait $cds before using this command again", lang));
			AddTranslation(new LangSetting("cooldowncassie", "Wait $cds before using a command that requires CASSIE (the announcer)", lang));

			AddTranslation(new LangSetting("cantsuicide", "You can't suicide when there's other SCP's remaining", lang));
            
            AddTranslation(new LangSetting("ultlocked", "To use an ultimate, you need level 4", lang));
            AddTranslation(new LangSetting("ultdown", "You must wait $cds before using ultimates again.", lang));
            AddTranslation(new LangSetting("ultlaunched", "Ultimate successfully used.", lang));

            AddTranslation(new LangSetting("ultusage", "Usage: .079 ultimate <number>\\n1. Lights out: shuts the HCZ down for 1 minute (cooldown: 180 seconds)\\n2. Lockdown: makes humans unable to open big doors, but SCPs can open any (duration: 30 seconds, cooldown: 300 seconds)", lang));
            
            AddTranslation(new LangSetting("kys", "<color=#AA1515>Press ` and write \".079 suicide\" to kill yourself.</color>", lang));
		}
    }
}
