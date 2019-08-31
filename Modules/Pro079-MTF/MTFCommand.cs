using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pro079Core.API;
using Smod2.API;
using Pro079Core;
using Smod2;

namespace Pro079MTF
{
	class MTFCommand : ICommand079
	{
		private MTFPlugin plugin;
		public MTFCommand(MTFPlugin plugin) => this.plugin = plugin;
		public bool OverrideDisable = false;
		public bool Disabled
		{
			get
			{
				return OverrideDisable ? OverrideDisable : !plugin.enabled;
			}
			set
			{
				OverrideDisable = value;
			}
		}

		public string Command => plugin.mtfcmd;

		public string HelpInfo => plugin.extendedHelp;

		public bool Cassie => true;

		public int Cooldown => plugin.cooldown;

		public int MinLevel => plugin.level;

		public int APCost => plugin.cost;

		public string CommandReady => plugin.mtfready;

		public int CurrentCooldown { get; set; }

		public string ExtraUsage => plugin.usage;

		public string CallCommand(string[] args, Player player)
		{
			if (Disabled)
			{
				return Pro079.Configs.CommandDisabled;
			}
			if (args.Length >= 3)
			{
				if (!int.TryParse(args[2], out int scpLeft) || !int.TryParse(args[1], out int mtfNum) || !char.IsLetter(args[0][0]))
				{
					return plugin.mtfuse.Replace("$min", plugin.cost.ToString());
				}
				if (scpLeft > plugin.maxscp)
				{
					return plugin.mtfuse.Replace("$min", plugin.cost.ToString()) +
						plugin.mtfmaxscp.Replace("$max", plugin.maxscp.ToString());
				}
				PluginManager.Manager.Server.Map.AnnounceNtfEntrance(scpLeft, mtfNum, args[1][0]);
				Pro079.Manager.GiveExp(player, 5f, ExperienceType.CHEAT);
				return Pro079.Configs.CommandSuccess;
			}
			else return plugin.mtfuse.Replace("$min", plugin.cost.ToString());
		}
	}
}
