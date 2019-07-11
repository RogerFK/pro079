using scp4aiur;
using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace pro079
{
    [PluginDetails(
        author = "RogerFK",
        name = "Pro 079",
        description = "Now it's funnier isn't it lol",
        id = "rogerfk.pro079",
        version = "1.0",
        SmodMajor = 3,
        SmodMinor = 4,
        SmodRevision = 0
        )]

    public class Pro079 : Plugin
    {
        public override void OnDisable()
        {
            this.Info(this.Details.name + " disabled");
        }

        public override void OnEnable()
        {
            this.Info(this.Details.name + " enabled");
        }

        public override void Register()
        {
			this.Info("Loading Pro-079 configs and commands...");
            Timing.Init(this);
			AddEventHandlers(new Pro79Handlers(this));

			AddConfig(new ConfigSetting("p079_enable", true, true, "Enables/disables Pro-079"));
			AddConfig(new ConfigSetting("p079_broadcast_enable", true, true, "Enables a broadcast at the start of the round."));

			#region Specific configs
			AddConfig(new ConfigSetting("p079_tesla", true, true, "Enables/disables Pro-079's Teslas functionality"));
			AddConfig(new ConfigSetting("p079_tesla_seconds", 10, true, "How many seconds the teslas will be disabled for"));
			AddConfig(new ConfigSetting("p079_tesla_cost", 15, true, "AP cost for the tesla command"));
			AddConfig(new ConfigSetting("p079_tesla_global_cost", 50, true, "AP cost for the teslas command"));
			AddConfig(new ConfigSetting("p079_tesla_level", 1, true, "Level for the tesla and teslas command"));

			AddConfig(new ConfigSetting("p079_info", true, true, "Enables/disables Pro-079's info functionality"));
			AddConfig(new ConfigSetting("p079_info_alive", 1, true, "Minimum level to display the info about how many people are alive"));
			AddConfig(new ConfigSetting("p079_info_decont", 2, true, "Minimum level to display how many Class D and Scientists are alive"));
			AddConfig(new ConfigSetting("p079_info_escaped", 2, true, "Minimum level to display the info about how many people escaped"));
			AddConfig(new ConfigSetting("p079_info_plebs", 2, true, "Minimum level to display how many Class D and Scientists are alive"));
			AddConfig(new ConfigSetting("p079_info_mtfci", 3, true, "Minimum level to display how many MTF and Chaos are alive"));
			AddConfig(new ConfigSetting("p079_info_gens", 1, true, "Minimum level to display info about the generators"));
            AddConfig(new ConfigSetting("p079_info_mtfest", 3, true, "Minimum level to display info about the time that MTF will come"));

            // Ultimates not configurable yet. Will use a Dictionary for Ult IDs in the future with it's own Ultimate079 class
            // If you want to help with this, at least point me towards the best way to read other .dlls for custom made Ultimates or to use piping
            AddConfig(new ConfigSetting("p079_ult", true, true, "Enables/disables Pro-079's fake Chaos messages"));
			
			AddConfig(new ConfigSetting("p079_suicide", true, true, "Enables/disables 079 to suicide when he is alone. Gives a shit about lone-079, feel free to use it"));

            // From now on, they are just about C.A.S.S.I.E. announcements
            AddConfig(new ConfigSetting("p079_cassie_cooldown", 30f, true, "How many seconds will C.A.S.S.I.E commands be generally disabled for after each command"));

			AddConfig(new ConfigSetting("p079_mtf", true, true, "Enables/disables Pro-079's fake MTF messages"));
			AddConfig(new ConfigSetting("p079_mtf_cooldown", 60f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_mtf_level", 2, true, "Minimum level for 079 to fake a MTF message"));
			AddConfig(new ConfigSetting("p079_mtf_cost", 70, true, "AP cost for 079 to fake a MTF message"));
			AddConfig(new ConfigSetting("p079_mtf_maxscp", 5, true, "Max SCP input so the guy doesn't spam it, or spams it"));

			AddConfig(new ConfigSetting("p079_chaos", false, true, "Enables/disables Pro-079's fake Chaos messages"));
			AddConfig(new ConfigSetting("p079_chaos_cooldown", 50f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_chaos_level", 2, true, "Minimum level for 079 to fake a Chaos message"));
			AddConfig(new ConfigSetting("p079_chaos_cost", 70, true, "AP cost for 079 to fake a Chaos message"));
			AddConfig(new ConfigSetting("p079_chaos_msg", "warning . chaosinsurgency detected in the surface", true, "Message for the chaos command"));

			AddConfig(new ConfigSetting("p079_scp", true, true, "Enables/disables Pro-079's fake SCP death messages"));
			AddConfig(new ConfigSetting("p079_scp_cooldown", 30f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_scp_cost", 40, true, "AP cost for 079 to fake a SCP death message"));
			AddConfig(new ConfigSetting("p079_scp_level", 2, true, "Minimum level for 079 to fake a SCP death message"));
            AddConfig(new ConfigSetting("p079_scp_list", new string[] { "173", "096", "106", "049", "939" }, true, "List of allowed SCPs you can type in the .079 scp command"));

            AddConfig(new ConfigSetting("p079_gen", true, true, "Enables/disables Pro-079's fake SCP death messages"));
			AddConfig(new ConfigSetting("p079_gen_cooldown", 60f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_gen_cost", 40, true, "Cost for the command"));
			AddConfig(new ConfigSetting("p079_gen_cost_blackout", 40, true, "Penalty for the Gen5 and Gen6 commands"));
			AddConfig(new ConfigSetting("p079_gen_level", 2, true, "Minimum level for 079 to fake a generator"));
			AddConfig(new ConfigSetting("p079_gen_level_blackout", 3, true, "Minimum level for 079 to fake gen 5 and gen 6"));
			AddConfig(new ConfigSetting("p079_gen_penalty", 60f, true, "For how long there will be a penalty after using gen 5 or gen 6"));

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
			AddTranslation(new LangSetting(  "teslacmd", "te", lang));
			AddTranslation(new LangSetting( "teslascmd", "teslas", lang));
			AddTranslation(new LangSetting(    "mtfcmd", "mtf", lang));
			AddTranslation(new LangSetting(    "gencmd", "gen", lang));
			AddTranslation(new LangSetting(    "scpcmd", "scp", lang));
			AddTranslation(new LangSetting(   "infocmd", "info", lang));
			AddTranslation(new LangSetting("suicidecmd", "suicide", lang));
			AddTranslation(new LangSetting(    "ultcmd", "ultimate", lang));
			AddTranslation(new LangSetting(  "chaoscmd", "chaos", lang));
			AddTranslation(new LangSetting(   "tipscmd", "tips", lang));

			AddTranslation(new LangSetting(  "teslahelp", "<b>.079 te</b> - Disables the tesla of the room you're in for $sec seconds", lang));
			AddTranslation(new LangSetting( "teslashelp", "<b>.079 teslas</b> - Disables all teslas for $sec seconds", lang));
			AddTranslation(new LangSetting(    "mtfhelp", "<b>.079 mtf <character> <number> <alive-scps></b> - Announces that a new MTF squad arrived, with your own custom number of SCPs", lang)); //Lanza un mensaje sobre que ha llegado la MTF a la zona con un número que elijas de SCPs con vida"
            AddTranslation(new LangSetting(    "genhelp", "<b>.079 gen [1-5]</b> - Announces that X generators are enabled, if it's 6 it will fake your suicide", lang));
			AddTranslation(new LangSetting(    "scphelp", "<b>.079 scp <###> <reason></b> - Fakes an SCP (173, 096...) death, the reason can be: unknown, tesla, mtf, decont", lang));
			AddTranslation(new LangSetting(   "infohelp", "<b>.079 info</b> - Shows stuff about the facility", lang));
			AddTranslation(new LangSetting("suicidehelp", "<b>.079 suicide</b> - Overcharges the generators to die when you're alone", lang));
			AddTranslation(new LangSetting(    "ulthelp", "<b>.079 ultimate</b> - Displays info about ultimates", lang));
			AddTranslation(new LangSetting(  "chaoshelp", "<b>.079 chaos</b> - Announces the chaos comming", lang));
			AddTranslation(new LangSetting(   "tipshelp", "<b>.079 tips</b> - Tips about SCP-079 and stuff to take into account", lang));

			AddTranslation(new LangSetting("mtfuse", "Usage: .079 mtf (p) (5) (4), will say Papa-5 is coming and there are 4 SCP remaining - $min ap", lang));
			AddTranslation(new LangSetting("mtfmaxscp", "Maximum SCPs: $max", lang));

			AddTranslation(new LangSetting("scpuse", "Usage: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont) - $min AP", lang));
			AddTranslation(new LangSetting("scpexist", "Type a SCP that exists", lang));
			AddTranslation(new LangSetting("scpway", "Type a method that exists", lang));

			#endregion

			AddTranslation(new LangSetting("notscp079", "You aren't SCP-079!", lang));

			AddTranslation(new LangSetting("cassieready", "<color=#85ff4c>Announcer (CASSIE) commands ready</color>", lang));
			AddTranslation(new LangSetting("ultready", "<color=#85ff4c>Ultimates ready</color>", lang));
			AddTranslation(new LangSetting("mtfready", "<color=#85ff4c>MTF command ready</color>", lang));
			AddTranslation(new LangSetting("genready", "<color=#85ff4c>Generator command ready</color>", lang));
			AddTranslation(new LangSetting("scpready", "<color=#85ff4c>SCP command ready</color>", lang));
			AddTranslation(new LangSetting("ready", "ready", lang));
			AddTranslation(new LangSetting("success", "Command successfully handled", lang));

			AddTranslation(new LangSetting("lowlevel", "Your level is too low (you need $min)", lang));
			AddTranslation(new LangSetting("lowmana", "Not enough AP (you need  $min)", lang));

			AddTranslation(new LangSetting("disabled", "This command is disabled.", lang));

			AddTranslation(new LangSetting("teslaerror",    "This tesla is already disabled.", lang));
			AddTranslation(new LangSetting("teslasuccess",  "Tesla disabled.", lang));
			AddTranslation(new LangSetting("globaltesla", "All teslas disabled.", lang));
			AddTranslation(new LangSetting("teslanotclose", "You're not close to a tesla.", lang));
			AddTranslation(new LangSetting("cooldown", "You have to wait $cds before using this command again", lang));
			AddTranslation(new LangSetting("cooldowncassie", "Wait $cds before using a command that requires CASSIE (the announcer)", lang));

			AddTranslation(new LangSetting("cantsuicide", "You can't suicide when there's other SCP's remaining", lang));

            AddTranslation(new LangSetting("genuse", "Use: .079 gen (1-6) - Will announce there are X generator activated, or will fake your death if you ttype 6. 5 generators will fake your whole recontainment process. - $min AP", lang));
            AddTranslation(new LangSetting("gen5msg", "Success. Your recontainment procedure, including when lights are turned off and a message telling you died, will be played.", lang));
			AddTranslation(new LangSetting("gen6msg", "Fake death command launched.", lang));

            AddTranslation(new LangSetting("nomtfleft", "No MTF's alive. Sending as \"unknown\"", lang));

            // Info translations
            AddTranslation(new LangSetting("decontdisabled", "Decontamination is disabled", lang));
			AddTranslation(new LangSetting("deconthappened", "LCZ is decontaminated", lang));
            // This happens when the nuke goes off before decont, but I don't know how it works and how many minutes it adds, because I saw at one time -3 mins.
            AddTranslation(new LangSetting("decontbug", "should have happened", lang));
            AddTranslation(new LangSetting("mtfest0", "between $(min)s and $(max)s", lang));
            AddTranslation(new LangSetting("mtfest1", "less than $(max)", lang));
            AddTranslation(new LangSetting("mtfest2", "are respawning / should have already respawned", lang));
                                                     //SCP alive: $scpalive\\nHumans alive: $humans | N N N N N N N : $estMTF\\nTiempo hasta la descontami: $decont\\nClase D escapados: $cdesc | Científicos escapados: $sciesc\\nClase D vivos:     $cdalive | Chaos vivos:           $cialive\\nCientíficos vivos: $scialive | MTF vivos:             $mtfalive
            AddTranslation(new LangSetting("infomsg", "SCP alive: $scpalive\\nHumans alive: $humans | Next MTF/Chaos: $estMTF\\nTime until decontamination: $decont\\nEscaped Class Ds:  $cdesc | Escaped scientists:    $sciesc\\nAlive Class-Ds:    $cdalive | Alive chaos:           $cialive\\nAlive scientists:  $scialive | Alive MTFs:            $mtfalive", lang));
            
            AddTranslation(new LangSetting("lockeduntil", "Locked until level $lvl", lang));
			AddTranslation(new LangSetting("generators", "Generators:", lang));

            AddTranslation(new LangSetting("generatorin", "$room's generator", lang));
            AddTranslation(new LangSetting("activated", "is activated.", lang));
			AddTranslation(new LangSetting("hastablet", "has a tablet", lang));
			AddTranslation(new LangSetting("notablet", "doesn't have a tablet", lang));

			AddTranslation(new LangSetting("timeleft", "and has $secs remaining", lang));

            AddTranslation(new LangSetting("ultlocked", "To use an ultimate, you need level 4", lang));
            AddTranslation(new LangSetting("ultdown", "You must wait $cds before using ultimates again.", lang));
            AddTranslation(new LangSetting("ultlaunched", "Ultimate successfully used.", lang));

            AddTranslation(new LangSetting("ultusage", "Usage: .079 ultimate <number>\\n1. Lights out: shuts the HCZ down for 1 minute (cooldown: 180 seconds)\\n2. Lockdown: makes humans unable to open big doors, but SCPs can open any (duration: 30 seconds, cooldown: 300 seconds)", lang));
            
            AddTranslation(new LangSetting("kys", "<color=#AA1515>Press ` and write \".079 suicide\" to kill yourself.</color>", lang));
		}
    }
}
