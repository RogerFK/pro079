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
			float startingTime = UnityEngine.Time.time;
			AddEventHandlers(new Pro79Handlers(this));

			AddConfig(new ConfigSetting("p079_enable", true, true, "Enables/disables Pro-079"));
			AddConfig(new ConfigSetting("p079_broadcast_enable", true, true, "Enables a broadcast at the start of the round."));

			#region Specific configs
			AddConfig(new ConfigSetting("p079_tesla", true, true, "Enables/disables Pro-079's Teslas functionality"));
			AddConfig(new ConfigSetting("p079_tesla_seconds", 10, true, "How many seconds the teslas will be disabled for"));
			AddConfig(new ConfigSetting("p079_tesla_cost", 15, true, "AP cost for the tesla command"));
			AddConfig(new ConfigSetting("p079_tesla_global_cost", 50, true, "AP cost for the teslas command"));
			AddConfig(new ConfigSetting("p079_tesla_level", 1, true, "Level for the tesla and teslas command"));

			AddConfig(new ConfigSetting("p079_info", false, true, "Enables/disables Pro-079's info functionality"));
			AddConfig(new ConfigSetting("p079_info_cost", 5, true, "AP cost for the info command"));
			AddConfig(new ConfigSetting("p079_info_alive", 1, true, "Minimum level to display the info about how many people are alive"));
			AddConfig(new ConfigSetting("p079_info_decont", 2, true, "Minimum level to display how many Class D and Scientists are alive"));
			AddConfig(new ConfigSetting("p079_info_escape", 2, true, "Minimum level to display the info about how many people escaped"));
			AddConfig(new ConfigSetting("p079_info_plebs", 2, true, "Minimum level to display how many Class D and Scientists are alive"));
			AddConfig(new ConfigSetting("p079_info_mtfci", 3, true, "Minimum level to display how many MTF and Chaos are alive"));
			AddConfig(new ConfigSetting("p079_info_gens", 1, true, "Minimum level to display info about the generators"));

			// Ultimates not configurable yet. Will use a Dictionary for Ult IDs in the future with it's own Ultimate079 class
			// If you want to help with this, at least point me towards the best way to read other .dlls for custom made Ultimates or to use piping
			AddConfig(new ConfigSetting("p079_ult", false, true, "Enables/disables Pro-079's fake Chaos messages"));
			
			AddConfig(new ConfigSetting("p079_suicide", true, true, "Enables/disables 079 to suicide when he is alone. Gives a shit about lone-079, feel free to use it"));

			// From now on, they are just about C.A.S.S.I.E. announcements
			AddConfig(new ConfigSetting("p079_cassie_cooldown", 30f, true, "How many seconds will C.A.S.S.I.E commands be generally disabled for after each command"));

			AddConfig(new ConfigSetting("p079_mtf", true, true, "Enables/disables Pro-079's fake MTF messages"));
			AddConfig(new ConfigSetting("p079_mtf_cooldown", 60f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_mtf_level", 2, true, "Minimum level for 079 to fake a MTF message"));
			AddConfig(new ConfigSetting("p079_mtf_cost", 70, true, "AP cost for 079 to fake a MTF message"));
			AddConfig(new ConfigSetting("p079_mtf_maxscp", 5, true, "Max SCP input so the guy doesn't spam it"));

			AddConfig(new ConfigSetting("p079_chaos", false, true, "Enables/disables Pro-079's fake Chaos messages"));
			AddConfig(new ConfigSetting("p079_chaos_cooldown", 50f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_chaos_level", 2, true, "Minimum level for 079 to fake a Chaos message"));
			AddConfig(new ConfigSetting("p079_chaos_cost", 70, true, "AP cost for 079 to fake a Chaos message"));
			AddConfig(new ConfigSetting("p079_chaos_msg", "warning . chaosinsurgency detected in the surface", true, "Message for the chaos command"));

			AddConfig(new ConfigSetting("p079_scp", true, true, "Enables/disables Pro-079's fake SCP death messages"));
			AddConfig(new ConfigSetting("p079_scp_cooldown", 30f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_scp_cost", 40, true, "AP cost for 079 to fake a SCP death message"));
			AddConfig(new ConfigSetting("p079_scp_level", 2, true, "Minimum level for 079 to fake a SCP death message"));

			AddConfig(new ConfigSetting("p079_gen", true, true, "Enables/disables Pro-079's fake SCP death messages"));
			AddConfig(new ConfigSetting("p079_gen_cooldown", 60f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_gen_cost", 40, true, "Cost for the command"));
			AddConfig(new ConfigSetting("p079_gen_level", 1, true, "Minimum level for 079 to fake a SCP death message"));
			AddConfig(new ConfigSetting("p079_gen_penalty", 60f, true, "For how long there will be a penalty after using gen 5 or gen 6"));

			AddConfig(new ConfigSetting("p079_tips", true, true, "Enables/disables Pro-079's fake SCP death messages"));
			#endregion

			// Translations
			AddConfig(new ConfigSetting("p079_lang", "es", true, "Selects the language"));
			string lang = GetConfigString("p079_lang");

			AddTranslation(new LangSetting("broadcast_msg", "<color=#85ff4c>Presiona ñ para abrir la consola y usar comandos adicionales</color>", "pro079_" + this.GetConfigString("p079_lang")));
			AddTranslation(new LangSetting("help", "<b>.079</b> - Muestra este mensaje de ayuda", "pro079_" + lang));

			AddTranslation(new LangSetting("level", "nivel $lvl", "pro079_" + lang));
			AddTranslation(new LangSetting("energy", "$ap de energía", "pro079_" + lang));

			AddTranslation(new LangSetting("unknowncmd", "Comando desconocido. Escribe .079 para recibir ayuda.", "pro079_" + lang));
			AddTranslation(new LangSetting("bugwarn", "Si encuentras algún fallo, avisa a RogerFK#3679", "pro079_" + lang));

			#region Cmds and Help strings
			AddTranslation(new LangSetting(  "teslacmd", "te", "pro079_" + lang));
			AddTranslation(new LangSetting( "teslascmd", "teslas", "pro079_" + lang));
			AddTranslation(new LangSetting(    "mtfcmd", "mtf", "pro079_" + lang));
			AddTranslation(new LangSetting(    "gencmd", "gen", "pro079_" + lang));
			AddTranslation(new LangSetting(    "scpcmd", "scp", "pro079_" + lang));
			AddTranslation(new LangSetting(   "infocmd", "info", "pro079_" + lang));
			AddTranslation(new LangSetting("suicidecmd", "suicidio", "pro079_" + lang));
			AddTranslation(new LangSetting(    "ultcmd", "ultimate", "pro079_" + lang));
			AddTranslation(new LangSetting(  "chaoscmd", "chaos", "pro079_" + lang));
			AddTranslation(new LangSetting(   "tipscmd", "controles", "pro079_" + lang));

			AddTranslation(new LangSetting(  "teslahelp", "<b>.079 te</b> - Desactiva la tesla de la habitación en la que estás durante $sec segundos", "pro079_" + lang));
			AddTranslation(new LangSetting( "teslashelp", "<b>.079 teslas</b> - Desactiva todas las teslas durante $sec segundos", "pro079_" + lang));
			AddTranslation(new LangSetting(    "mtfhelp", "<b>.079 mtf <letra> <numero> <scp-vivos></b> - Lanza un mensaje sobre que ha llegado la MTF a la zona con un número que elijas de SCPs con vida", "pro079_" + lang));
			AddTranslation(new LangSetting(    "genhelp", "<b>.079 gen [1-5]</b> - Manda el mensaje de que X generadores han sido activados, o manda con un 6 para fingir tu muerte", "pro079_" + lang));
			AddTranslation(new LangSetting(    "scphelp", "<b>.079 scp <###> <motivo></b> - Manda un mensaje de muerte de SCP con el número del SCP (173, 096...), el motivo puede ser: unknown, tesla, mtf, decont", "pro079_" + lang));
			AddTranslation(new LangSetting(   "infohelp", "<b>.079 info</b> - Muestra datos sobre las instalaciones", "pro079_" + lang));
			AddTranslation(new LangSetting("suicidehelp", "<b>.079 suicidio</b> - Sobrecarga los generadores para morir cuando quedes tú solo", "pro079_" + lang));
			AddTranslation(new LangSetting(    "ulthelp", "<b>.079 ultimate</b> - Mira los ultimate que tienes disponibles", "pro079_" + lang));
			AddTranslation(new LangSetting("chaoshelp", "<b>.079 chaos</b> - Anuncia la llegada de Chaos Insurgency", "pro079_" + lang));
			AddTranslation(new LangSetting(   "tipshelp", "<b>.079 controles</b> - Controles de SCP-079 y cosas a tener en cuenta", "pro079_" + lang));

			AddTranslation(new LangSetting("mtfuse", "Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP - $min de energía", "pro079_" + lang));
			AddTranslation(new LangSetting("mtfmaxscp", "Máximo de SCPs: $max", "pro079_" + lang));

			AddTranslation(new LangSetting("scpuse", "Uso: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont) - $min de energía", "pro079_" + lang));
			AddTranslation(new LangSetting("scpexist", "Pon un SCP que exista", "pro079_" + lang));
			AddTranslation(new LangSetting("scpway", "Pon un método de morir que exista - Uso:", "pro079_" + lang));

			#endregion
			// This line down below probably causes many issues when creating the file for the first time. Will be changed (or default completely disabled) in the future
			AddTranslation(new LangSetting("tips", "TAB (encima del Bloq. Mayus): abre el mapa donde estás.\\nEspacio: cambia tu modo de cámara entre el modo normal (ratón libre) y el modo primera persona (con el punto blanco).\\nTeclas de movimiento: muévete a la cámara que indica arriba a la derecha\\nPara salir de la heavy containment zone, ve hacia el elevador y pulsa el recuadro blanco, o hacia el checkpoint y usa la W para moverte entre cámaras\\nAdicionalmente, este plugin te permite usar comandos como podrás haber comprobado usando .079\\n", "pro079_" + this.GetConfigString("p079_lang")));

			AddTranslation(new LangSetting("notscp079", "¡No eres SCP-079!", "pro079_" + lang));

			AddTranslation(new LangSetting("cassieready", "<color=#85ff4c>Comandos de anunciante listos</color>", "pro079_" + lang));
			AddTranslation(new LangSetting("ultready", "<color=#85ff4c>Tus ultimates están listas</color>", "pro079_" + lang));
			AddTranslation(new LangSetting("mtfready", "<color=#85ff4c>Comando MTF listo</color>", "pro079_" + lang));
			AddTranslation(new LangSetting("genready", "<color=#85ff4c>Comando generador listo</color>", "pro079_" + lang));
			AddTranslation(new LangSetting("scpready", "<color=#85ff4c>Comando SCP listo</color>", "pro079_" + lang));
			AddTranslation(new LangSetting("ready", "listo", "pro079_" + lang));
			AddTranslation(new LangSetting("success", "Comando lanzado", "pro079_" + lang));

			AddTranslation(new LangSetting("lowlevel", "No tienes suficiente nivel (necesitas $min)", "pro079_" + lang));
			AddTranslation(new LangSetting("lowmana", "No tienes suficiente AP (necesitas $min)", "pro079_" + lang));
			//AddTranslation(new LangSetting("minimum", "Mínimo: ", "pro079_" + lang));

			AddTranslation(new LangSetting("disabled", "Este comando está deshabilitado.", "pro079_" + lang));

			AddTranslation(new LangSetting("teslaerror",    "Esta Tesla ya está desactivada.", "pro079_" + lang));
			AddTranslation(new LangSetting("teslasuccess",  "Tesla desactivada.", "pro079_" + lang));
			AddTranslation(new LangSetting("globaltesla", "Teslas desactivadas.", "pro079_" + lang));
			AddTranslation(new LangSetting("teslanotclose", "No estás cerca de una Tesla.", "pro079_" + lang));
			AddTranslation(new LangSetting("cooldown", "Tienes que esperar antes de usar este comando", "pro079_" + lang));
			AddTranslation(new LangSetting("cooldowncassie", "Espera antes de volver a usar un comando que requiera a C.A.S.S.I.E (anunciante)", "pro079_" + lang));

			AddTranslation(new LangSetting("cantsuicide", "No puedes suicidarte cuando hay más SCP vivos", "pro079_" + lang));

			AddTranslation(new LangSetting("gen5msg", "Comando lanzado. Se reproducirá el mensaje de tu contención al completo, incluyendo cuando te matan y cuando se apagan/encienden las luces.", "pro079_" + lang));
			AddTranslation(new LangSetting("gen6msg", "Comando de falsear suicidio lanzado.", "pro079_" + lang));
			Timing.Init(this);

			this.Info("Done loading! Took " + (UnityEngine.Time.time - startingTime).ToString("0.00") + " seconds to complete!");
		}
    }
}
