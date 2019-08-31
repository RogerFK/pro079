using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pro079Core.API;
using Smod2;
using Smod2.API;

namespace ChaosCommand
{
	class ChaosCommand : ICommand079
	{
		private ChaosPlugin plugin;

		public ChaosCommand(ChaosPlugin plugin)
		{
			this.plugin = plugin;
		}

		public bool OverrideDisable = false;
		public bool Disabled
		{
			get
			{
				return OverrideDisable || !plugin.enable;
			}
			set
			{
				OverrideDisable = value;
			}
		}

		public string Command => plugin.chaoscmd;

		public string ExtraArguments => string.Empty;

		public string HelpInfo => plugin.chaoshelp;

		public bool Cassie => true;

		public int Cooldown => plugin.cooldown;

		public int MinLevel => plugin.level;

		public int APCost => plugin.cost;

		public string CommandReady => plugin.ready;

		public int CurrentCooldown { get; set; }

		public string CallCommand(string[] args, Player player)
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage(plugin.msg);
			return Pro079Core.Pro079.Configs.CommandSuccess;
		}
	}
}
