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

		public bool OverrideDisable = false;
		public bool Disabled
		{
			get => OverrideDisable ? OverrideDisable : !plugin.enabled;
			set => OverrideDisable = value;
		}

		public string Command => plugin.scpcmd;

		public string ExtraArguments => plugin.extrainfo;

		public string HelpInfo => plugin.scpusage;

		public bool Cassie => true;

		public int Cooldown => plugin.cooldown;

		public int MinLevel => plugin.level;

		public int APCost => plugin.cost;

		public string CommandReady => plugin.ready;

		public int CurrentCooldown { get; set; }

		public string CallCommand(string[] args, Player player)
		{
			if (args.Length < 3)
			{
				return plugin.scpuse.Replace("$min", plugin.cost.ToString());
			}

			if (!plugin.GetConfigList("p079_scp_list").Contains(args[1]))
			{
				return plugin.scpexist + " - " + plugin.scpuse.Replace("$min", plugin.cost.ToString());
			}
			string scpNum = string.Join(" ", args[1].ToCharArray());
			switch (args[1])
			{
				case "mtf":
					Player dummy = null;
					List<Role> mtf = new List<Role>
										{
											Role.FACILITY_GUARD, Role.NTF_CADET, Role.NTF_LIEUTENANT, Role.NTF_SCIENTIST, Role.NTF_COMMANDER, Role.SCIENTIST
										};
					foreach (Player ply in PluginManager.Manager.Server.GetPlayers())
					{
						if (mtf.Contains(ply.TeamRole.Role))
						{
							dummy = ply;
							break;
						}
					}
					if (dummy == null)
					{
						player.SendConsoleMessage(plugin.nomtfleft, "red");
					}

					PluginManager.Manager.Server.Map.AnnounceScpKill(args[0], dummy);
					break;
				case "unknown":
					PluginManager.Manager.Server.Map.AnnounceScpKill(args[0], null);
					break;
				case "tesla":
					PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Successfully Terminated by automatic security system");
					break;
				case "decont":
					PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Lost in Decontamination Sequence");
					break;
				default:
					return plugin.scpway + " .079 scp " + args[0] + " (unknown/tesla/mtf/decont)";
			}
			Pro079Core.Pro079.Manager.GiveExp(player, 5 * (player.Scp079Data.Level + 1), ExperienceType.CHEAT);
			return Pro079Core.Pro079.Configs.CommandSuccess;
		}
	}
}
