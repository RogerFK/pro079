using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pro079_Info
{
	class MTFCommand : ICommand079
	{
		public string CallComand(string[] args, Player player)
		{
			if (!plugin.GetConfigBool("p079_mtf"))
			{
				ev.ReturnMessage = plugin.GetTranslation("disabled");
				return;
			}
			if (ev.Player.Scp079Data.Level < plugin.GetConfigInt("p079_mtf_level") - 1)
			{
				ev.ReturnMessage = plugin.GetTranslation("lowlevel").Replace("$min", plugin.GetConfigInt("p079_mtf_level").ToString());
				return;
			}
			if (PluginManager.Manager.Server.Round.Duration < cooldownCassieGeneral)
			{
				ev.ReturnMessage = plugin.GetTranslation("cooldowncassie").Replace("$cd", (cooldownCassieGeneral - PluginManager.Manager.Server.Round.Duration).ToString());
				return;
			}
			if (args.Count() >= 4)
			{
				if (PluginManager.Manager.Server.Round.Duration < cooldownMTF && !ev.Player.GetBypassMode())
				{
					ev.ReturnMessage = plugin.GetTranslation("cooldown").Replace("$cd", (cooldownMTF - PluginManager.Manager.Server.Round.Duration).ToString());
					return;
				}
				if (ev.Player.Scp079Data.AP >= plugin.GetConfigInt("p079_mtf_cost") || ev.Player.GetBypassMode())
				{
					if (!int.TryParse(args[3], out int scpLeft) || !int.TryParse(args[2], out int mtfNum) || !char.IsLetter(args[1][0]))
					{
						ev.ReturnMessage = plugin.GetTranslation("mtfuse").Replace("$min", plugin.GetConfigInt("p079_mtf_cost").ToString());
						return;
					}
					if (scpLeft > plugin.GetConfigInt("p079_mtf_maxscp"))
					{
						ev.ReturnMessage = ev.ReturnMessage = plugin.GetTranslation("mtfuse").Replace("$min", plugin.GetConfigInt("p079_mtf_cost").ToString()) +
							plugin.GetTranslation("mtfmaxscp").Replace("$max", plugin.GetConfigInt("p079_mtf_maxscp").ToString());
						return;
					}
					if (!ev.Player.GetBypassMode())
					{
						ev.Player.Scp079Data.AP -= plugin.GetConfigInt("p079_mtf_cost");

						cooldownCassieGeneral = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_cassie_cooldown");
						cooldownMTF = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_mtf_cooldown");
						Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
						Timing.Run(CooldownMTF(plugin.GetConfigFloat("p079_mtf_cooldown")));
					}
					PluginManager.Manager.Server.Map.AnnounceNtfEntrance(scpLeft, mtfNum, args[1][0]);
					ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
					ev.Player.Scp079Data.Exp += 2.8f * (ev.Player.Scp079Data.Level + 1);

					ev.ReturnMessage = plugin.GetTranslation("success");
					return;

				}
				else
				{
					ev.ReturnMessage = plugin.GetTranslation("lowmana").Replace("$min", plugin.GetConfigInt("p079_mtf_cost").ToString());
				}
				return;
			}
			else
			{
				ev.ReturnMessage = plugin.GetTranslation("mtfuse").Replace("$min", plugin.GetConfigInt("p079_mtf_cost").ToString());
				return;
			}
		}
	}
}
