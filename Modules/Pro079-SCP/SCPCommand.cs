using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pro079Core.API;
using Smod2.API;

namespace SCPCommand
{
	class SCPCommand : ICommand079
	{
		private readonly SCPPlugin plugin;
		public SCPCommand(SCPPlugin plugin) => this.plugin = plugin;

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

		public string Command => plugin.scpcmd;

		public string ExtraArguments => plugin.extrainfo;

		public string HelpInfo => plugin.scpusage;

		public bool Cassie => true;

		public int Cooldown => throw new NotImplementedException();

		public int MinLevel => throw new NotImplementedException();

		public int APCost => throw new NotImplementedException();

		public string CommandReady => throw new NotImplementedException();

		public int CurrentCooldown { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public string CallCommand(string[] args, Player player)
		{
			if (!plugin.GetConfigBool("p079_scp"))
			{
				ev.ReturnMessage = plugin.GetTranslation("disabled");
				return;
			}
			if (!ev.Player.GetBypassMode())
			{
				if (ev.Player.Scp079Data.Level < plugin.GetConfigInt("p079_scp_level") - 1)
				{
					ev.ReturnMessage = plugin.GetTranslation("lowlevel").Replace("$min", plugin.GetConfigInt("p079_scp_level").ToString());
					return;
				}
				if (PluginManager.Manager.Server.Round.Duration < cooldownScp)
				{
					ev.ReturnMessage = plugin.GetTranslation("cooldown").Replace("$cd", (cooldownScp - PluginManager.Manager.Server.Round.Duration).ToString());
					return;
				}
				if (ev.Player.Scp079Data.AP < plugin.GetConfigInt("p079_scp_cost"))
				{
					ev.ReturnMessage = plugin.GetTranslation("lowmana").Replace("$min", plugin.GetConfigInt("p079_scp_cost").ToString());
					return;
				}
			}
			if (args.Length >= 3)
			{
				if (!plugin.GetConfigList("p079_scp_list").Contains(args[1]))
				{
					ev.ReturnMessage = plugin.GetTranslation("scpexist") + " - " + plugin.GetTranslation("scpuse").Replace("$min", plugin.GetConfigInt("p079_scp_cost").ToString());
					return;
				}
				string scpNum = string.Join(" ", args[1].ToCharArray());
				switch (args[2])
				{
					case "mtf":
						Player dummy = null;
						List<Role> mtf = new List<Role>
										{
											Role.FACILITY_GUARD, Role.NTF_CADET, Role.NTF_LIEUTENANT, Role.NTF_SCIENTIST, Role.NTF_COMMANDER, Role.SCIENTIST
										};
						foreach (Player player in PluginManager.Manager.Server.GetPlayers())
						{
							if (mtf.Contains(player.TeamRole.Role))
							{
								dummy = player;
								break;
							}
						}
						if (dummy == null)
						{
							ev.Player.SendConsoleMessage(plugin.GetTranslation("nomtfleft"), "red");
						}

						PluginManager.Manager.Server.Map.AnnounceScpKill(args[1], dummy);
						ev.ReturnMessage = plugin.GetTranslation("success");
						break;
					case "unknown":
						PluginManager.Manager.Server.Map.AnnounceScpKill(args[1], null);
						ev.ReturnMessage = plugin.GetTranslation("success");
						break;
					case "tesla":
						PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Successfully Terminated by automatic security system");
						ev.ReturnMessage = plugin.GetTranslation("success");
						break;
					case "decont":
						PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Lost in Decontamination Sequence");
						ev.ReturnMessage = plugin.GetTranslation("success");
						break;
					default:
						ev.ReturnMessage = plugin.GetTranslation("scpway") + " .079 scp " + args[1] + " (unknown/tesla/mtf/decont)";
						return;
				}
			}
			else
			{
				ev.ReturnMessage = plugin.GetTranslation("scpuse").Replace("$min", plugin.GetConfigInt("p079_scp_cost").ToString());
				return;
			}
			ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
			ev.Player.Scp079Data.AP -= plugin.GetConfigInt("p079_scp_cost");
			ev.Player.Scp079Data.Exp += 5.0f * (ev.Player.Scp079Data.Level + 1);
			cooldownCassieGeneral = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_cassie_cooldown");
			cooldownScp = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_scp_cooldown");
			Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
			Timing.Run(CooldownScp(plugin.GetConfigFloat("p079_scp_cooldown")));
			return;
		}
	}
}
