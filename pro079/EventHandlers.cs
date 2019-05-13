using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using scp4aiur; // Future update will have this removed
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;

namespace pro079
{
	internal class Pro79Handlers : IEventHandlerCallCommand, IEventHandlerSetRole, IEventHandlerSetConfig, IEventHandlerPlayerDie, IEventHandlerTeamRespawn, IEventHandlerDoorAccess, IEventHandlerWaitingForPlayers
	{
		private readonly Pro079 plugin;
		private List<int> broadcasted = new List<int>(); // to only clear broadcasts for these players, good practice if you have playerprefs
		private string helpFormatted;
		public Pro79Handlers(Pro079 plugin)
		{
			this.plugin = plugin;
		}
		private static bool cooldownGenerator = false;
		private static bool cooldownCassieGeneral = false;
		private static bool cooldownMTF = false;
		private static bool cooldownChaos = false;
		private static bool ultDown = false;
		private static IEnumerable<Room> rooms;
		private static bool DeconBool { get; set; }
		private static float DeconTime { get; set; }
		private static int MinMTF { get; set; }
		private static int MaxMTF { get; set; }
		private static float LastMtfSpawn { get; set; }
		private bool UltDoors = false;
		private bool cooldownScp;

		private string FormatEnergyLevel(int energy, int level, string energStr, string lvlStr)
		{
			string str;
			if (energy > 0 || level > 1)
			{
				str = " (" + (energy > 0 ? energStr.Replace("$ap", energy.ToString()) : "")
					+ (level > 1 ? ", " + lvlStr.Replace("$lvl", level.ToString()) : "") + ')';
				return str;
			}
			return "";
		}

		// This thing below was pasted from here: https://www.c-sharpcorner.com/blogs/first-letter-in-uppercase-in-c-sharp1
		public static string FirstCharToUpper(string s)
		{
			// Check for empty string.
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			// Return char and concat substring.  
			return char.ToUpper(s[0]) + s.Substring(1);
		}
		private string FormatHelp()
		{
			// Auxiliary stuff so you don't have to get the translations multiple times
			string help = plugin.GetTranslation("help");
			string lvlaux = plugin.GetTranslation("level"), energyaux = plugin.GetTranslation("energy");
			if (plugin.GetConfigBool("p079_tesla"))
			{
				string aux = plugin.GetTranslation("teslahelp");
				help += '\n' + aux.Replace("$sec", plugin.GetConfigInt("p079_tesla_seconds").ToString()) +
					" (" + energyaux.Replace("$energy", plugin.GetConfigInt("p079_tesla_cost").ToString()) + (plugin.GetConfigInt("p079_tesla_level") > 1 ? ", " + lvlaux.Replace("$lvl", plugin.GetConfigInt("p079_tesla_level").ToString()) : "") + ')';
				aux = plugin.GetTranslation("teslashelp");
				help += '\n' + aux.Replace("$sec", plugin.GetConfigInt("p079_tesla_seconds").ToString())
				+ FormatEnergyLevel(plugin.GetConfigInt("p079_tesla_global_cost"), plugin.GetConfigInt("p079_tesla_level"), energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_mtf"))
			{
				help += '\n' + plugin.GetTranslation("mtfhelp")
				+ FormatEnergyLevel(plugin.GetConfigInt("p079_mtf_cost"), plugin.GetConfigInt("p079_mtf_level"), energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_chaos"))
			{
				help += '\n' + plugin.GetTranslation("chaoshelp")
				+ FormatEnergyLevel(plugin.GetConfigInt("p079_chaos_cost"), plugin.GetConfigInt("p079_chaos_level"), energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_scp"))
			{
				help += '\n' + plugin.GetTranslation("scphelp")
				+ FormatEnergyLevel(plugin.GetConfigInt("p079_scp_cost"), plugin.GetConfigInt("p079_scp_level"), energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_suicide"))
			{
				help += '\n' + plugin.GetTranslation("suicidehelp");
			}
			if (plugin.GetConfigBool("p079_ult"))
			{
				help += '\n' + plugin.GetTranslation("ulthelp")
				+ FormatEnergyLevel(0, 4, energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_info"))
			{
				help += '\n' + plugin.GetTranslation("infohelp") + FormatEnergyLevel(plugin.GetConfigInt("p079_info_cost"), 0, energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_tips"))
			{
				help += '\n' + plugin.GetTranslation("tipshelp") + FormatEnergyLevel(0, 0, energyaux, lvlaux);
			}
			return help;
		}

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			string command = ev.Command.ToLower();
			if (command.StartsWith("079"))
			{
				ev.ReturnMessage = plugin.GetTranslation("unknowncmd");
				//this is pasted from PlayerPrefs https://github.com/probe4aiur/PlayerPreferences/
				MatchCollection collection = new Regex("[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'").Matches(command);
				string[] args = new string[collection.Count - 1];

				for (int i = 1; i < collection.Count; i++)
				{
					// If the first char (0) and the last one (its length - 1) is ", that is the char (defined by ' ')
					// that is \" (the \ is to define it's not a quote for a string), then take the substring that
					// starts from the second character to the second last one. Pretty fucking clever.
					if (collection[i].Value[0] == '\"' && collection[i].Value[collection[i].Value.Length - 1] == '\"')
					{
						args[i - 1] = collection[i].Value.Substring(1, collection[i].Value.Length - 2);
					}
					else
					{
						args[i - 1] = collection[i].Value;
					}
				}
				// end of the paste thx
				// everything below this is completely hardcoded in Spanish
				if (ev.Player.TeamRole.Role == Role.SCP_079)
				{
					if (args.Length == 0)
					{
						ev.Player.SendConsoleMessage(helpFormatted, "white");
						ev.ReturnMessage = plugin.GetTranslation("bugwarn") + " <Made by RogerFK#3679>";
					}
					else if (args.Length == 1)
					{
						// Most unclear way to do the switch statement, but anyways it's the most optimized way to do it.
						switch (SwitchParser.ParseArg(args[0], plugin))
						{
							case 10: // tipscmd
								if (!plugin.GetConfigBool("p079_tips"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								ev.Player.SendConsoleMessage(plugin.GetTranslation("controls").Replace("\\n", "\n"), "white");
								ev.ReturnMessage = "<Made by RogerFK#3679>";
								return;
							case 1: // teslacmd
								if (!plugin.GetConfigBool("p079_tesla"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								if (ev.Player.Scp079Data.Level < plugin.GetConfigInt("p079_tesla_level") - 1)
								{
									ev.ReturnMessage = plugin.GetTranslation("lowlevel").Replace("$min", plugin.GetConfigInt("p079_tesla_level").ToString());
									return;
								}
								if (ev.Player.Scp079Data.AP >= plugin.GetConfigInt("p079_tesla_cost"))
								{
									bool noTesla = true;
									foreach (Smod2.API.TeslaGate tesla in PluginManager.Manager.Server.Map.GetTeslaGates())
									{
										if (Vector.Distance(ev.Player.Scp079Data.Camera, tesla.Position) < 8.0f)
										{
											if (tesla.TriggerDistance > 0)
											{
												ev.Player.Scp079Data.AP -= plugin.GetConfigInt("p079_tesla_cost");
												ev.Player.Scp079Data.ShowGainExp(ExperienceType.USE_TESLAGATE);
												ev.Player.Scp079Data.Exp += 10.0f / (ev.Player.Scp079Data.Level + 1); //ignore these
												Timing.Run(DisableTesla(tesla, tesla.TriggerDistance));
												noTesla = false;
												ev.ReturnMessage = plugin.GetTranslation("teslasuccess");
												break;
											}
											else
											{
												ev.ReturnMessage = plugin.GetTranslation("teslaerror");
											}
										}
									}
									if (noTesla == true)
									{
										ev.ReturnMessage = plugin.GetTranslation("teslanotclose");
									}
								}
								else
								{
									ev.ReturnMessage = plugin.GetTranslation("lowmana").Replace("$min", plugin.GetConfigInt("p079_tesla_cost").ToString());
								}
								return;
							case 2: // teslascmd
								if (!plugin.GetConfigBool("p079_tesla"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								if (ev.Player.Scp079Data.Level < plugin.GetConfigInt("p079_tesla_level") - 1 && !ev.Player.GetBypassMode())
								{
									ev.ReturnMessage = plugin.GetTranslation("lowlevel").Replace("$min", plugin.GetConfigInt("p079_tesla_level").ToString());
									return;
								}
								if (ev.Player.Scp079Data.AP < plugin.GetConfigInt("p079_tesla_global_cost") && !ev.Player.GetBypassMode())
								{
									ev.ReturnMessage = plugin.GetTranslation("lowmana").Replace("$min", plugin.GetConfigInt("p079_tesla_global_cost").ToString());
									return;
								}
								if (!ev.Player.GetBypassMode())
								{
									ev.Player.Scp079Data.AP -= plugin.GetConfigInt("p079_tesla_global_cost");
								}

								foreach (Smod2.API.TeslaGate tesla in PluginManager.Manager.Server.Map.GetTeslaGates())
								{
									if (tesla.TriggerDistance > 0)
									{
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.USE_TESLAGATE);
										ev.Player.Scp079Data.Exp += 5.0f / (ev.Player.Scp079Data.Level + 1); //ignore these
										Timing.Run(DisableTesla(tesla, tesla.TriggerDistance));
										ev.Player.Scp079Data.Exp += 4.0f / (ev.Player.Scp079Data.Level + 1); //ignore these
									}
								}
								ev.ReturnMessage = plugin.GetTranslation("globaltesla");
								return;
							case 7: // suicidecmd
								if (!plugin.GetConfigBool("p079_suicide"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
								int pcs = PCplayers.Count;
								if (PluginManager.Manager.Server.Round.Stats.SCPAlive + PluginManager.Manager.Server.Round.Stats.Zombies - pcs != 0)
								{
									ev.ReturnMessage = plugin.GetTranslation("cantsuicide");
									return;
								}
								PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
								Timing.Run(FakeKillPC());
								ev.Player.Kill(DamageType.NUKE);
								return;
							case 3: // mtfcmd
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
								if (cooldownCassieGeneral)
								{
									ev.ReturnMessage = plugin.GetTranslation("cooldowncassie");
									return;
								}
								if (args.Count() >= 4)
								{
									if (cooldownMTF && !ev.Player.GetBypassMode())
									{
										ev.ReturnMessage = this.plugin.GetTranslation("cooldown");
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
									ev.ReturnMessage = plugin.GetTranslation("mtfuse").Replace("$min", this.plugin.GetConfigInt("p079_mtf_cost").ToString());
									return;
								}
							case 5: // scpcmd
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
									if (cooldownScp)
									{
										ev.ReturnMessage = plugin.GetTranslation("cooldown");
										return;
									}
									if (ev.Player.Scp079Data.AP < plugin.GetConfigInt("p079_scp_cost") )
									{
										ev.ReturnMessage = plugin.GetTranslation("lowmana").Replace("$min", plugin.GetConfigInt("p079_scp_cost").ToString());
										return;
									}
								}
								if (args.Length >= 3)
								{
									string[] scpList = new string[5]
									{
										"173", "096", "106", "049", "939"
									};
									if (!scpList.Contains(args[1]))
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
											string matador = "la MTF";
											if (dummy == null)
											{
												ev.Player.SendConsoleMessage("No hay MTFs vivos. Mandando como \"unknown\"", "red");
												matador = "\"Unknown\" (Chaos o Clase D)";
											}

											PluginManager.Manager.Server.Map.AnnounceScpKill(args[1], dummy);
											ev.ReturnMessage = "Comando lanzado (SCP " + args[1] + " matado por " + matador + ".";
											break;
										case "unknown":
											PluginManager.Manager.Server.Map.AnnounceScpKill(args[1], null);
											ev.ReturnMessage = "Comando lanzado (SCP " + args[1] + " matado por \"Unknown\".";
											break;
										case "tesla":
											PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Successfully Terminated by automatic security systems");
											ev.ReturnMessage = "Comando lanzado (SCP " + args[1] + " matado por Tesla.";
											break;
										case "decont":
											PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Lost in Decontamination Sequence");
											ev.ReturnMessage = "Comando lanzado (SCP " + args[1] + " matado por descontaminación.";
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
								Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
								Timing.Run(CooldownScp(plugin.GetConfigFloat("p079_scp_cooldown")));
								return;
							case 4: // gencmd
								if (!plugin.GetConfigBool("p079_gen"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								if (args.Count() == 1)
								{
									ev.ReturnMessage = "Uso: .079 gen (1-5) - Sonará que hay 1 generador activado - 50 de energía";
									return;
								}
								// No need for a double check. The program already knows there are two arguments.
								if (ev.Player.Scp079Data.Level < plugin.GetConfigInt("p079_gen_level") - 1 && !ev.Player.GetBypassMode())
								{
									ev.ReturnMessage = plugin.GetTranslation("lowlevel").Replace("$min", plugin.GetConfigInt("p079_gen_level").ToString());
									return;
								}
								if (ev.Player.Scp079Data.AP < plugin.GetConfigInt("p079_gen_cost") && !ev.Player.GetBypassMode())
								{//"p079_gen_cost"
									ev.ReturnMessage = ev.ReturnMessage = plugin.GetTranslation("lowmana").Replace("$min", plugin.GetConfigInt("p079_gen_cost").ToString());
									return;
								}
								if (cooldownGenerator && !ev.Player.GetBypassMode())
								{
									ev.ReturnMessage = "Tienes que esperar antes de volver a usar este comando";
									return;
								}
								switch (args[1])
								{
									case "1":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon1");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += 20f;
										Timing.Run(CooldownGen(plugin.GetConfigFloat("p079_gen_cooldown")));
										Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
										ev.ReturnMessage = plugin.GetTranslation("success");
										return;
									case "2":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon2");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += 20f;
										Timing.Run(CooldownGen(plugin.GetConfigFloat("p079_gen_cooldown")));
										Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
										ev.ReturnMessage = plugin.GetTranslation("success");
										return;
									case "3":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon3");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += 20f;
										Timing.Run(CooldownGen(plugin.GetConfigFloat("p079_gen_cooldown")));
										Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
										ev.ReturnMessage = plugin.GetTranslation("success");
										return;
									case "4":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon4");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += 20f;
										Timing.Run(CooldownGen(plugin.GetConfigFloat("p079_gen_cooldown")));
										Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
										ev.ReturnMessage = plugin.GetTranslation("success");
										return;
									case "5":
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										Timing.Run(Fake5Gens());
										ev.Player.Scp079Data.Exp += 80f;
										Timing.Run(CooldownGen(70.3f + plugin.GetConfigFloat("p079_gen_penalty") + plugin.GetConfigFloat("p079_gen_cooldown")));
										Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
										ev.ReturnMessage = plugin.GetTranslation("gen5msg");
										return;
									case "6":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += 50f;
										Timing.Run(CooldownGen(plugin.GetConfigFloat("p079_gen_penalty") + plugin.GetConfigFloat("p079_gen_cooldown")));
										Timing.Run(CooldownCassie(plugin.GetConfigFloat("p079_cassie_cooldown")));
										Timing.Run(FakeKillPC());
										ev.ReturnMessage = plugin.GetTranslation("gen6msg");
										return;
									default:
										ev.ReturnMessage = "Uso: .079 gen 1 - Sonará que hay 1 generador activado, del 1-5, pon 6 para mandar que has muerto (nivel 3)\nEl 5 falseará la sequencia completa de contención";
										return;
								}
							case 6: // infocmd
								if (!plugin.GetConfigBool("p079_info"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								if (ev.Player.Scp079Data.AP < 5 && !ev.Player.GetBypassMode())
								{
									ev.ReturnMessage = "No tienes suficiente energía (necesitas 5).";
									return;
								}
								string humansAlive = "[Nivel 2]";
								string tiempoDecont = "[Nivel 2]";
								string ScientistsEscaped = "[Nivel 3]";
								string ClassDEscaped = "[Nivel 3]";
								string ClassDAlive = "[Nivel 2]";
								string ScientistsAlive = "[Nivel 2]";
								string MTFAlive = "[Nivel 3]";
								string CiAlive = "[Nivel 3]";
								string MTFtiempo = "[Nivel 3]";

								if (ev.Player.Scp079Data.Level > 0 || ev.Player.GetBypassMode())
								{
									humansAlive = (PluginManager.Manager.Server.Round.Stats.ClassDAlive + PluginManager.Manager.Server.Round.Stats.ScientistsAlive + PluginManager.Manager.Server.Round.Stats.CiAlive + PluginManager.Manager.Server.Round.Stats.NTFAlive).ToString();

									ClassDAlive = PluginManager.Manager.Server.Round.Stats.ClassDAlive.ToString("00");
									ScientistsAlive = PluginManager.Manager.Server.Round.Stats.ScientistsAlive.ToString("00");

									if (DeconBool == true)
									{
										tiempoDecont = "La descontaminación está desactivada";
									}
									else if (PluginManager.Manager.Server.Map.LCZDecontaminated == true)
									{
										tiempoDecont = "LCZ está descontaminada";
									}
									else
									{
										tiempoDecont = (DeconTime - float.Parse(PluginManager.Manager.Server.Round.Duration.ToString()) / 60.0f).ToString("0.00");
									}
								}
								if (ev.Player.Scp079Data.Level > 1 || ev.Player.GetBypassMode())
								{
									ClassDEscaped = PluginManager.Manager.Server.Round.Stats.ClassDEscaped.ToString("00");
									ScientistsEscaped = PluginManager.Manager.Server.Round.Stats.ScientistsEscaped.ToString("00");

									MTFAlive = PluginManager.Manager.Server.Round.Stats.NTFAlive.ToString("00");
									CiAlive = PluginManager.Manager.Server.Round.Stats.CiAlive.ToString("00");

									if (PluginManager.Manager.Server.Round.Duration - LastMtfSpawn < MinMTF)
									{
										MTFtiempo = "entre " + (MinMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0") + " segundos y " + (MaxMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0") + " segundos.";
									}
									else if (PluginManager.Manager.Server.Round.Duration - LastMtfSpawn < MaxMTF)
									{
										MTFtiempo = "menos de " + (MaxMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0");
									}
									else
									{
										MTFtiempo = "están reapareciendo / van a reaparecer pronto.";
									}
								}
								ev.Player.SendConsoleMessage(
								"\nSCP vivos: " + PluginManager.Manager.Server.Round.Stats.SCPAlive +
								"\nHumanos vivos: " + humansAlive + " | Siguientes MTF/Chaos: " + MTFtiempo +
								"\nTiempo hasta la descontaminación: " + tiempoDecont +
								"\nClase D escapados: " + ClassDEscaped + " | Científicos escapados: " + ScientistsEscaped +
								"\nClase D vivos:     " + ClassDAlive + " | Chaos vivos:           " + CiAlive +
								"\nCientíficos vivos: " + ScientistsAlive + " | MTF vivos:             " + MTFAlive
								, "white");
								ev.ReturnMessage = "Generadores:\n";
								foreach (Generator generator in PluginManager.Manager.Server.Map.GetGenerators())
								{
									ev.ReturnMessage = ev.ReturnMessage + "Generador de " + generator.Room.RoomType.ToString();
									if (generator.Engaged)
									{
										ev.ReturnMessage += " está activado.\n";
									}
									else
									{
										ev.ReturnMessage += (generator.HasTablet ? " tiene una tablet y le quedan " : " no tiene tablet y le quedan ") + generator.TimeLeft.ToString("0") + " segundos.\n";
									}
								}
								ev.Player.Scp079Data.AP -= 5;
								ev.Player.Scp079Data.Exp += 5;
								return;
							case 8: // ultcmd
								if (!plugin.GetConfigBool("p079_ult"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								if (args.Count() == 1)
								{
									if (ev.Player.Scp079Data.Level < 3 && !ev.Player.GetBypassMode())
									{
										ev.ReturnMessage = "Para lanzar un ultimate necesitas tier 4.";
									}
									else
									{
										ev.ReturnMessage = "Uso: .079 ultimate <número>\n" +
											"1. Luces fuera: apaga durante 1 minuto la HCZ (cooldown: 180 segundos)\n" +
											"2. Lockdown: impide a los humanos abrir puertas, permite a los SCP abrir cualquiera (duración: 30 segundos, cooldown: 300 segundos)\n" +
											"3. ... ¡Añade tu propia aquí! Tan solo tienes que ponerlo en #sugerencias-debates o en #sugerencias (ve a #bots y pon ,suggest \"Tu idea\" en el Discord de World in Chaos.\n"
											+ "Adicionalmente, si estás baneado, muteado o cualquier cosa, puedes contactar directamente con RogerFK#3679";
									}
								}
								else
								{
									if (ev.Player.Scp079Data.Level < 3 && !ev.Player.GetBypassMode())
									{
										ev.ReturnMessage = "Para lanzar un ultimate necesitas tier 4";
										return;
									}
									if (ultDown)
									{
										ev.ReturnMessage = "Debes esperar antes de volver a usar un ultimate.";
										return;
									}
									if (!int.TryParse(args[1], out int ult))
									{
										ev.ReturnMessage = "Uso: .079 ultimate <número>\n" +
										"1. Luces fuera: apaga durante 1 minuto la HCZ (cooldown: 180 segundos)\n" +
										"2. Lockdown: impide a los humanos abrir puertas, permite a los SCP abrir cualquiera (duración: 30 segundos, cooldown: 300 segundos)\n" +
										"3. ... ¡Añade tu propia aquí! Tan solo tienes que ponerlo en #sugerencias-debates en el Discord, si nos gusta, ¡la añadiremos!.";
										return;
									}
									ev.ReturnMessage = "Ultimate lanzada.";
									switch (ult)
									{
										case 1:
											PluginManager.Manager.Server.Map.AnnounceCustomMessage("warning . malfunction detected on heavy containment zone . Scp079Recon6 . . . light systems Disengaged");
											Timing.Run(ShamelessTimingRunLights());
											Timing.Run(CooldownUlt(180f));
											return;
										case 2:
											PluginManager.Manager.Server.Map.AnnounceCustomMessage("warning facility control lost . starting security lockdown");
											Timing.Run(Ult2Toggle(30f));
											Timing.Run(CooldownUlt(300f));
											return;
										default:
											ev.ReturnMessage = "Uso: .079 ultimate <número>\n" +
											"1. Luces fuera: apaga durante 1 minuto la HCZ (cooldown: 180 segundos)\n" +
											"2. Lockdown: impide a los humanos abrir puertas, permite a los SCP abrir cualquiera (duración: 30 segundos, cooldown: 300 segundos)\n" +
											"3. ... ¡Añade tu propia aquí! Tan solo tienes que ponerlo en #sugerencias-debates en el Discord de World in Chaos.";
											return;
									}
								}

								return;
							case 9: // chaoscmd
								if (plugin.GetConfigBool("p079_chaos"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								if (cooldownCassieGeneral)
								{
									ev.ReturnMessage = plugin.GetTranslation("cooldowncassie");
									return;
								}
								if (cooldownChaos)
								{
									if (ev.Player.Scp079Data.Level + 1 < plugin.GetConfigInt("p079_chaos_level"))
									{
										ev.ReturnMessage = plugin.GetConfigString("lowlevel").Replace("$min", plugin.GetConfigInt("p079_chaos_level").ToString());
										return;
									}
								}

								if (ev.Player.Scp079Data.AP < plugin.GetConfigInt("p079_chaos_cost"))
								{
									ev.ReturnMessage = plugin.GetConfigString("lowmana").Replace("$min", plugin.GetConfigInt("p079_chaos_cost").ToString());
									return;
								}
								ev.Player.Scp079Data.AP -= plugin.GetConfigInt("p079_chaos_cost");
								PluginManager.Manager.Server.Map.AnnounceCustomMessage(plugin.GetConfigString("p079_chaos_msg"));
								ev.ReturnMessage = plugin.GetTranslation("success");
								return;
							default:
								ev.ReturnMessage = plugin.GetTranslation("unknowncmd");
								return;
						}
					}
				}
				else
				{
					ev.ReturnMessage = "¡No eres SCP-079!";
				}
			}
		}
		
		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (!plugin.GetConfigBool("p079_broadcast_enable"))
			{
				return;
			}

			if (ev.Role != Role.SCP_079)
			{
				// Not done in the same IF as it would have to read the List if it's SCP 079 two times
				if (broadcasted.Contains(ev.Player.PlayerId))
				{
					ev.Player.PersonalClearBroadcasts();
				}
			}
			else //if (ev.Role == Role.SCP_079)
			{
				ev.Player.PersonalBroadcast(20, plugin.GetTranslation("broadcast_msg"), true);
				ev.Player.SendConsoleMessage(helpFormatted, "white");
				if (!broadcasted.Contains(ev.Player.PlayerId))
				{
					broadcasted.Add(ev.Player.PlayerId);
				}
			}
		}

		public static IEnumerable<float> DisableTesla(Smod2.API.TeslaGate tesla, float current)
		{
			tesla.TriggerDistance = -1f;
			yield return 10f;
			tesla.TriggerDistance = current;
		}
		/* // this is fucking op btw
		public static IEnumerable<float> OverCharge()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Warning All Security Personnel Unauthorized Use Of Error Error Error Error pitch_2 error error error error pitch_1 error");

			yield return 7.3f;

			foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
			{
				try { if (room.ZoneType == ZoneType.HCZ) room.FlickerLights(); }
				catch { }
			}

			foreach (Smod2.API.TeslaGate tesla in PluginManager.Manager.Server.Map.GetTeslaGates())
			{
				Timing.Run(DisableTesla(tesla, tesla.TriggerDistance));
			}
		}
		*/
		public static IEnumerable<float> FakeKillPC()
		{
			// falta cerrar puertas
			yield return 7.3f;

			foreach (Room room in rooms)
			{
				room.FlickerLights();
			}
			yield return 8f;
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("SCP 0 7 9 ContainedSuccessfully"); // thanks to "El n*z* jud*o" (uh...) for helping me with this
		}

		public static IEnumerable<float> Fake5Gens()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon5");
			yield return 19.89f + 60f; // this value is fucking shit actually
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
			Timing.Run(FakeKillPC());
		}

		/* Cooldowns could be substituted with a coroutine,
		 * but it wouldn't matter anyways as there will always be
		 * a coroutine for the broadcast 
		 
		 * Also before you ask, no, you can't pass a bool as a reference in C#
		 * or else I don't know the proper way to do it*/
		private IEnumerable<float> CooldownScp(float v)
		{
			if (v > 0)
			{
				cooldownScp = true;
				yield return v;
				List<Player> pcs = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player pc in pcs)
				{
					pc.PersonalBroadcast(6, plugin.GetTranslation("scpready"), false);
				}
				cooldownScp = false;
			}
		}
		private IEnumerable<float> CooldownUlt(float time)
		{
			if (time > 0)
			{
				ultDown = true;
				yield return time;
				ultDown = false;
				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(6, plugin.GetTranslation("ultready"), false);
				} 
			}
		}

		private IEnumerable<float> Ult2Toggle(float v)
		{
			if (v > 0)
			{
				UltDoors = true;
				yield return v;
				UltDoors = false;
				PluginManager.Manager.Server.Map.AnnounceCustomMessage("attention all Personnel . doors lockdown finished");
			}
		}

		private IEnumerable<float> CooldownMTF(float time)
		{
			if (time > 0)
			{
				cooldownMTF = true;
				yield return time;
				cooldownMTF = false;
				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(5, plugin.GetTranslation("mtfready"), false);
				} 
			}
		}

		private IEnumerable<float> CooldownGen(float time)
		{
			if (time > 0)
			{
				cooldownGenerator = true;
				yield return time;
				cooldownGenerator = false;

				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(5, plugin.GetTranslation("genready"), false);
				} 
			}
		}

		private IEnumerable<float> CooldownCassie(float time)
		{
			if (time > 0)
			{
				cooldownCassieGeneral = true;
				yield return time;
				cooldownCassieGeneral = false;

				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(5, plugin.GetTranslation("cassieready"), false);
				} 
			}
		}
		
		/* pasted and slightly modified from here btw https://github.com/probe4aiur/Blackout
		 * tbh that's what I was programming myself but then I had to check how many seconds
		 * the lights were going to be turned off for, so I just actually copied that yield
		 * thingy I swear to god I'm not just a copy paster (and if I were I would still give proper credit)
		 */
		private IEnumerable<float> ShamelessTimingRunLights()
		{
			yield return 12.1f;
			float start = PluginManager.Manager.Server.Round.Duration;
			while (start + 60f > PluginManager.Manager.Server.Round.Duration)
			{
				foreach (Room room in rooms)
				{
					room.FlickerLights();
				}
				yield return 8f;
			}
		}

		public void OnSetConfig(SetConfigEvent ev)
		{
			switch (ev.Key)
			{
				case "disable_decontamination":
					DeconBool = (bool)ev.Value;
					return;
				case "decontamination_time":
					DeconTime = (float)ev.Value;
					return;
				case "minimum_MTF_time_to_spawn":
					MinMTF = (int)ev.Value;
					return;
				case "maximum_MTF_time_to_spawn":
					MaxMTF = (int)ev.Value;
					return;
				default:
					return;
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (ev.Player.TeamRole.Team == Smod2.API.Team.SCP && ev.Player.TeamRole.Role != Role.SCP_079)
			{
				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				int pcs = PCplayers.Count;
				if (PluginManager.Manager.Server.Round.Stats.SCPAlive + PluginManager.Manager.Server.Round.Stats.Zombies - pcs == 0)
				{
					foreach (Player player in PCplayers)
					{
						player.PersonalBroadcast(10, "<color=#AA1515>Pulsa ñ y escribe \".079 suicidio\" para suicidarte.</color>", false);
					}
				}
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			LastMtfSpawn = PluginManager.Manager.Server.Round.Duration;
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			if (UltDoors == false || string.IsNullOrWhiteSpace(ev.Door.Permission))
			{
				return;
			}
			else
			{
				if (ev.Player.TeamRole.Team == Smod2.API.Team.SCP)
				{
					ev.Allow = true;
				}
				else
				{
					ev.Allow = false;
				}
			}
		}
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			rooms = PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(x => x.ZoneType == ZoneType.HCZ);
			helpFormatted = FormatHelp();
			cooldownGenerator = false;
			cooldownCassieGeneral = false;
			cooldownMTF = false;
			ultDown = false;
			DeconBool = false;
			UltDoors = false;
			cooldownChaos = false;
		}
	}
}
