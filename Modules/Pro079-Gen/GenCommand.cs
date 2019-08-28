using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pro079_Info
{
	class GenCommand : ICommand079
	{
		public string CallComand(string[] args, Player player)
		{
			if (!plugin.GetConfigBool("p079_gen"))
			{
				ev.ReturnMessage = plugin.GetTranslation("disabled");
				return;
			}
			if (args.Count() == 1)
			{
				ev.ReturnMessage = plugin.GetTranslation("genuse").Replace("$min", plugin.GetConfigInt("p079_gen_cost").ToString());
				return;
			}
			// No need for a double check. The program already knows there are two arguments.
			if (ev.Player.Scp079Data.Level < plugin.GetConfigInt("p079_gen_level") - 1 && !ev.Player.GetBypassMode())
			{
				ev.ReturnMessage = plugin.GetTranslation("lowlevel").Replace("$min", plugin.GetConfigInt("p079_gen_level").ToString());
				return;
			}
			if (ev.Player.Scp079Data.AP < plugin.GetConfigInt("p079_gen_cost") && !ev.Player.GetBypassMode())
			{
				ev.ReturnMessage = plugin.GetTranslation("lowmana").Replace("$min", plugin.GetConfigInt("p079_gen_cost").ToString());
				return;
			}
			if (PluginManager.Manager.Server.Round.Duration < cooldownGenerator && !ev.Player.GetBypassMode())
			{
				ev.ReturnMessage = plugin.GetTranslation("cooldown").Replace("$cd", cooldownGenerator.ToString());
				return;
			}
			int blackcost = plugin.GetConfigInt("p079_gen_cost") + plugin.GetConfigInt("p079_gen_cost_blackout");
			switch (args[1])
			{
				case "1":
				case "2":
				case "3":
				case "4":
					PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon" + args[1]);
					ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
					ev.Player.Scp079Data.Exp += 20f;
					ev.Player.Scp079Data.AP -= plugin.GetConfigInt("p079_gen_cost");
					cooldownCassieGeneral = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_cassie_cooldown");
					cooldownGenerator = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_gen_cooldown");
					Timing.Run(CooldownGen(plugin.GetConfigFloat("p079_gen_cooldown")));
					Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
					ev.ReturnMessage = plugin.GetTranslation("success");
					return;
				case "5":
					if (!ev.Player.GetBypassMode())
					{
						if (ev.Player.Scp079Data.Level < plugin.GetConfigInt("p079_gen_level_blackout") - 1)
						{
							ev.ReturnMessage = plugin.GetTranslation("lowlevel").Replace("$min", plugin.GetConfigInt("p079_gen_level_blackout").ToString());
							return;
						}
						if (ev.Player.Scp079Data.AP < blackcost)
						{
							ev.ReturnMessage = plugin.GetTranslation("lowmana").Replace("$min", blackcost.ToString());
							return;
						}
					}
					ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
					Timing.Run(Fake5Gens());
					ev.Player.Scp079Data.Exp += 80f;
					ev.Player.Scp079Data.AP -= blackcost;
					cooldownCassieGeneral = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_cassie_cooldown");
					cooldownGenerator = 70.3f + PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_gen_penalty") + plugin.GetConfigFloat("p079_gen_cooldown");
					Timing.Run(CooldownGen(70.3f + plugin.GetConfigFloat("p079_gen_penalty") + plugin.GetConfigFloat("p079_gen_cooldown")));
					Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
					ev.ReturnMessage = plugin.GetTranslation("gen5msg");
					return;
				case "6":
					if (!ev.Player.GetBypassMode())
					{
						if (ev.Player.Scp079Data.Level < plugin.GetConfigInt("p079_gen_level_blackout") - 1)
						{
							ev.ReturnMessage = plugin.GetTranslation("lowlevel").Replace("$min", plugin.GetConfigInt("p079_gen_level_blackout").ToString());
							return;
						}
						if (ev.Player.Scp079Data.AP < blackcost)
						{
							ev.ReturnMessage = plugin.GetTranslation("lowmana").Replace("$min", blackcost.ToString());
							return;
						}
					}
					PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
					ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
					ev.Player.Scp079Data.Exp += 50f;
					ev.Player.Scp079Data.AP -= blackcost;
					cooldownCassieGeneral = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_cassie_cooldown");
					cooldownGenerator = PluginManager.Manager.Server.Round.Duration + plugin.GetConfigFloat("p079_gen_penalty") + plugin.GetConfigFloat("p079_gen_cooldown");
					Timing.Run(CooldownGen(plugin.GetConfigFloat("p079_gen_penalty") + plugin.GetConfigFloat("p079_gen_cooldown")));
					Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
					Timing.Run(FakeKillPC());
					ev.ReturnMessage = plugin.GetTranslation("gen6msg");
					return;
				default:
					ev.ReturnMessage = plugin.GetTranslation("genuse");
					return;
			}
		}
	}
}
