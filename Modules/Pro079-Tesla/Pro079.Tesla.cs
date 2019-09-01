using Pro079Core;
using Pro079Core.API;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace TeslaCommand
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079 Tesla",
		description = "Tesla command for Pro-079.",
		id = "rogerfk.pro079.tesla",
		version = "2.0",
		langFile = "p079tesla",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]

	public class TeslaPlugin : Plugin
	{
		public override void OnDisable()
		{
			Info("Pro079 Tesla disabled.");
		}

		public override void OnEnable()
		{
			Info("Pro079 Tesla enabled");
		}
		[ConfigOption("p079_tesla")]
		public readonly bool enabled = true;
		[ConfigOption]
		public readonly int cost = 40;
		[ConfigOption]
		public readonly int level = 1;
		[ConfigOption]
		public readonly int remaining = 5;
		[LangOption]
		public readonly string teslacmd = "te";
		[LangOption]
		public readonly string teslaExtra = "<time>";
		[LangOption]
		public readonly string teslause = "Disables all teslas for the amount of seconds you want";
		[LangOption]
		public readonly string globaltesla = "All teslas disabled.";
		[LangOption]
		public readonly string teslausage = "Usage: .079 $cmd <time>";
		[LangOption]
		public readonly string teslarem = "Teslas re-enabled in $sec seconds";
		[LangOption]
		public readonly string teslarenabled = "<color=#66F>Teslas re-enabled</color>";
		public override void Register()
		{
			Info("Loading Pro-079 Tesla configs and registering the command...");

			Pro079.Manager.RegisterCommand(new TeslaCommand(this));
		}
	}
	public class TeslaCommand : ICommand079
	{
		private readonly TeslaPlugin plugin;
		public TeslaCommand(TeslaPlugin teslaPlugin)
		{
			plugin = teslaPlugin;
		}

		public bool OverrideDisable = false;
		public bool Disabled
		{
			get => OverrideDisable || !plugin.enabled;
			set => OverrideDisable = value;
		}

		public string Command => plugin.teslacmd;

		public string ExtraArguments => plugin.teslaExtra;

		public string HelpInfo => plugin.teslause;

		public bool Cassie => false;

		public int Cooldown => 0;

		public int MinLevel => plugin.level;

		public int APCost => plugin.cost;

		public string CommandReady => string.Empty;

		public int CurrentCooldown { get => 0; set => CurrentCooldown = 0; }

		public string CallCommand(string[] args, Player player)
		{
			if (args.Length < 2)
			{
				return plugin.teslausage;
			}
			float time;
			if (!float.TryParse(args[1], out time))
			{
				return plugin.teslausage;
			}
			int p = (int)System.Environment.OSVersion.Platform;
			if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(TeslaLogic.DisableTeslas(time, plugin), MEC.Segment.Update);
			else MEC.Timing.RunCoroutine(TeslaLogic.DisableTeslas(time, plugin), 1);
			Pro079.Manager.GiveExp(player, time);
			return plugin.globaltesla;
		}
	}
}