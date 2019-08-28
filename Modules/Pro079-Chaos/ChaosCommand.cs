using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pro079_Info
{
	class ChaosCommand : ICommand079
	{
		public string CallComand(string[] args, Player player)
		{
			if (!plugin.GetConfigBool("p079_chaos"))
			{
				ev.ReturnMessage = plugin.GetTranslation("disabled");
				return;
			}
			if (PluginManager.Manager.Server.Round.Duration < cooldownCassieGeneral && !ev.Player.GetBypassMode())
			{
				ev.ReturnMessage = plugin.GetTranslation("cooldowncassie").Replace("$cd", (cooldownCassieGeneral - PluginManager.Manager.Server.Round.Duration).ToString());
				return;
			}
			if (PluginManager.Manager.Server.Round.Duration < cooldownChaos && !ev.Player.GetBypassMode())
			{
				ev.ReturnMessage = plugin.GetTranslation("cooldown").Replace("$cd", (cooldownChaos - PluginManager.Manager.Server.Round.Duration).ToString());
			}
			if (ev.Player.Scp079Data.Level + 1 < plugin.GetConfigInt("p079_chaos_level") && !ev.Player.GetBypassMode())
			{
				ev.ReturnMessage = plugin.GetTranslation("lowlevel").Replace("$min", plugin.GetConfigInt("p079_chaos_level").ToString());
				return;
			}
			if (ev.Player.Scp079Data.AP < plugin.GetConfigInt("p079_chaos_cost") && !ev.Player.GetBypassMode())
			{
				ev.ReturnMessage = plugin.GetTranslation("lowmana").Replace("$min", plugin.GetConfigInt("p079_chaos_cost").ToString());
				return;
			}
			if (!ev.Player.GetBypassMode())
			{
				ev.Player.Scp079Data.AP -= plugin.GetConfigInt("p079_chaos_cost");
				cooldownChaos = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigInt("p079_chaos_cooldown");
			}
			cooldownCassieGeneral = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_cassie_cooldown");
			cooldownChaos = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_chaos_cooldown");
			PluginManager.Manager.Server.Map.AnnounceCustomMessage(plugin.GetConfigString("p079_chaos_msg"));
			ev.ReturnMessage = plugin.GetTranslation("success");
			return;
		}
	}
}
