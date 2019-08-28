using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace pro079tesla
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

	public class Pro079 : Plugin, I079Command
	{
		public override void OnDisable()
		{
			this.Info("Pro079 Tesla disabled.");
		}

		public override void OnEnable()
		{
			this.Info("Pro079 Tesla enabled");
		}
		private string lang = "p079tesla";
		public override void Register()
		{
			this.Info("Loading Pro-079 Tesla configs and registering the command...");

			// Command configs
			AddConfig(new ConfigSetting("p079_tesla", true, true, "Enables/disables Pro-079's Teslas functionality"));
			AddConfig(new ConfigSetting("p079_tesla_cost", 40, true, "AP cost for the tesla command"));
			AddConfig(new ConfigSetting("p079_tesla_level", 1, true, "Level for the tesla and teslas command"));
			AddConfig(new ConfigSetting("p079_tesla_remaining", 5, true, "When should the countdown start to tell the user when teslas are going to be reactivated"));
			// Cmds and Help strings
			AddTranslation(new LangSetting("newteslacmd", "tesla", lang));

			AddTranslation(new LangSetting("newteslahelp", "<b>.079 tesla <time></b> - Disables all teslas for the amount of seconds you want", lang));

			AddTranslation(new LangSetting("globaltesla", "All teslas disabled.", lang));
			AddTranslation(new LangSetting("teslausage", "Usage: .079 tesla <time>", lang));
			AddTranslation(new LangSetting("teslarem", "Teslas re-enabled in $sec seconds", lang));
			AddTranslation(new LangSetting("teslarenabled", "<color=#66F>Teslas re-enabled</color>", lang));
		}

		public string CallComand(string[] args, Player player)
		{
			if (!this.GetConfigBool("p079_tesla"))
			{
				return ""; // Reference the disabled static translation from Pro079 Core
			}
			if (args.Length < 2)
			{
				ev.ReturnMessage = this.GetTranslation("teslausage");
				return;
			}
			float time;
			if (!float.TryParse(args[1], out time))
			{
				ev.ReturnMessage = this.GetTranslation("teslausage");
				return;
			}
			if (!ev.Player.GetBypassMode())
			{
				if (ev.Player.Scp079Data.Level < this.GetConfigInt("p079_tesla_level") - 1 && !ev.Player.GetBypassMode())
				{
					ev.ReturnMessage = this.GetTranslation("lowlevel").Replace("$min", @this.GetConfigInt("p079_tesla_level").ToString());
					return;
				}
				if (ev.Player.Scp079Data.AP < this.GetConfigInt("p079_tesla_cost") && !ev.Player.GetBypassMode())
				{
					ev.ReturnMessage = this.GetTranslation("lowmana").Replace("$min", @this.GetConfigInt("p079_tesla_cost").ToString());
					return;
				}
				ev.Player.Scp079Data.AP -= this.GetConfigInt("p079_tesla_cost");
			}
			Timing.Run(DisableTeslas(time));
			ev.Player.Scp079Data.Exp += 5.0f / (ev.Player.Scp079Data.Level + 1); //ignore these
			ev.ReturnMessage = this.GetTranslation("globaltesla");
			return;
		}
	}
}