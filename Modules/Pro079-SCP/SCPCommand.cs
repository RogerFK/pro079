using System.Collections.Generic;
using System.Linq;
using Pro079Core.API;
using Smod2;
using Smod2.API;

namespace SCPCommand
{
	internal class SCPCommand : ICommand079
	{
		private readonly SCPPlugin plugin;
		public SCPCommand(SCPPlugin plugin)
		{
			this.plugin = plugin;
		}
        Role[] mtf = new Role[]
        {
            Role.FACILITY_GUARD, Role.NTF_CADET, Role.NTF_LIEUTENANT, Role.NTF_SCIENTIST, Role.NTF_COMMANDER, Role.SCIENTIST
        };
        public bool OverrideDisable = false;
		public bool Disabled
		{
			get => OverrideDisable ? OverrideDisable : !plugin.enable;
			set => OverrideDisable = value;
		}

		public string Command => plugin.scpcmd;

		public string ExtraArguments => plugin.scpextrainfo;

		public string HelpInfo => plugin.scpusage;

		public bool Cassie => true;

		public int Cooldown => plugin.cooldown;

		public int MinLevel => plugin.level;

		public int APCost => plugin.cost;

		public string CommandReady => plugin.scpready;

		public int CurrentCooldown { get; set; }
        private string SpaceTheScp(string scp)
        {
            int length = scp.Length * 2 - 1;
            char[] spacedScp = new char[length];
            for(int i = 0; i < length; i++)
            {
                spacedScp[i] = (i % 2 == 0) ? scp[i/2] : ' ';
            }
            return new string(spacedScp);
        }
		public string CallCommand(string[] args, Player player, CommandOutput output)
		{
			if (args.Length < 2)
			{
				output.Success = false;
				return plugin.scpuse.Replace("$min", plugin.cost.ToString());
			}

			if (!plugin.GetConfigList("p079_scp_list").Contains(args[0]))
			{
				output.Success = false;
				return plugin.scpexist + " - " + plugin.scpuse.Replace("$min", plugin.cost.ToString());
			}
			switch (args[1])
			{
				case "mtf":
                    Player dummy = PluginManager.Manager.Server.GetPlayers(mtf).FirstOrDefault();
					if (dummy == null)
					{
						player.SendConsoleMessage(plugin.scpnomtfleft, "red");
					}
					PluginManager.Manager.Server.Map.AnnounceScpKill(args[0], dummy);
					break;
				case "unknown":
					PluginManager.Manager.Server.Map.AnnounceScpKill(args[0], null);
					break;
				case "tesla":
					PluginManager.Manager.Server.Map.AnnounceCustomMessage($"scp {SpaceTheScp(args[0])} Successfully Terminated by automatic security system");
					break;
				case "decont":
					PluginManager.Manager.Server.Map.AnnounceCustomMessage($"scp {SpaceTheScp(args[0])} Lost in Decontamination Sequence");
					break;
				default:
					return plugin.scpway + " .079 scp " + args[0] + " (unknown/tesla/mtf/decont)";
			}
			Pro079Core.Pro079.Manager.GiveExp(player, 5 * (player.Scp079Data.Level + 1), ExperienceType.CHEAT);
			return Pro079Core.Pro079.Configs.CommandSuccess;
		}
	}
}
