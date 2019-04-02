using scp4aiur;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace pro079
{
	internal class Pro79Handlers : IEventHandlerCallCommand, IEventHandlerSetRole
	{
		
		private readonly pro079 plugin;
		public Pro79Handlers(pro079 plugin)
		{
			this.plugin = plugin;
		}
		public static bool cooldownGenerator = false;
		public static bool cooldownCassieGeneral = false;
		public static bool infoCooldown = false;
		public static bool cooldownMTF = false;

		private static void MandaAyuda(Player player)
		{
			player.SendConsoleMessage("Estos son los comandos que tienes disponibles:\n" +
			".079 - Muestra este mensaje de ayuda\n" +
			".079 te - Desactiva la tesla de la habitación en la que estás durante 8 segundos (50 puntos)\n" +
			".079 mtf <letra> <numero> <scp-vivos> - Lanza un mensaje sobre que ha llegado la MTF a la zona con un número falso de SCPs con vida, o el actual si no (100 puntos, nivel 2)\n" +
			".079 gen [1-5] - Manda el mensaje de que X generadores han sido activados, o manda con un 6 para fingir tu muerte\n" +
			"Si mandas .079 gen 5, activarás la secuencia para fingir que estás siendo contenido\n" +
			".079 scp <###> <motivo> - Manda un mensaje de muerte de SCP con el número, el motivo puede ser: unknown, tesla, mtf, decont\n" +
			".079 suicidio - Sobrecarga los generadores para morir cuando quedes tú solo" +
			".079 info - Manda la gente que queda viva, junto a los SCP y los clase D y científicos" 
			//+ ".079 cont106 - Manda el audio de recontención de SCP 106" //no implementado porque seguramente no funcione bien, habría que invocar el audio y luego hacer el callrpc
			, "white");
		}

		
		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			string command = ev.Command.ToLower();
			//this is pasted from PlayerPrefs
			if (command.StartsWith("079"))
			{
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
						MandaAyuda(ev.Player);
						ev.ReturnMessage = "";
					}
					else if (args.Length == 1)
					{
						switch (args[0])
						{
							case "te":
								if (ev.Player.Scp079Data.AP >= 50)
								{
									bool noTesla = true;
									foreach (Smod2.API.TeslaGate tesla in PluginManager.Manager.Server.Map.GetTeslaGates())
									{
										if (Vector.Distance(ev.Player.Scp079Data.Camera, tesla.Position) < 8.0f)
										{
											if (tesla.TriggerDistance > 0)
											{
												ev.Player.Scp079Data.AP -= 50;
												ev.Player.Scp079Data.ShowGainExp(ExperienceType.USE_TESLAGATE);
												ev.Player.Scp079Data.Exp += (10.0f / (ev.Player.Scp079Data.Level + 1)); //ignore these
												Timing.Run(DisableTesla(tesla, tesla.TriggerDistance));
												noTesla = false;
												ev.ReturnMessage = "Tesla desactivada.";
												break;
											}
											else
											{
												ev.ReturnMessage = ("Esta Tesla está desactivada.");
											}
										}
									}
									if (noTesla == true)
									{
										ev.ReturnMessage = ("No estás cerca de una Tesla.");
									}
								}
								else
								{
									ev.ReturnMessage = ("No tienes suficiente energía (necesitas 50).");
								}

								return;
							case "suicidio":
								if ((PluginManager.Manager.Server.Round.Stats.Zombies + PluginManager.Manager.Server.Round.Stats.SCPAlive) != 1)
								{
									ev.ReturnMessage = ("No puedes suicidarte cuando hay más SCP vivos");
									return;
								}
								PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
								FakeKillPC(false);
								ev.Player.Kill(DamageType.NUKE);
								return;
							case "mtf":
								ev.ReturnMessage = ("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
								return;
							case "scp":
								ev.ReturnMessage = ("Uso: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont)");
								return;
							case "gen":
								ev.ReturnMessage = ("Uso: .079 gen (1-5) - Sonará que hay 1 generador activado");
								return;
							default:
								ev.ReturnMessage = ("Comando no reconocido. Usa .079 para ayuda");
								return;
						}
					}
					else if (args.Length > 1)
					{
						if (cooldownCassieGeneral)
						{
							ev.ReturnMessage = ("Tienes que esperar antes de volver a usar un comando que requiera a cassie");
							return;
						}
						switch (args[0])
						{
							case "mtf":
								if (cooldownMTF)
								{
									ev.ReturnMessage = ("Tienes que esperar antes de volver a usar el comando MTF");
									return;
								}
								if (ev.Player.Scp079Data.Level >= 1)
								{
									if (ev.Player.Scp079Data.AP >= 80)
									{
										if (args.Length == 4)
										{
											try
											{
												int.Parse(args[3]);
												int.Parse(args[2]);
												if (!char.IsLetter(args[1][0]))
												{
													ev.ReturnMessage = ("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
													return;
												}
											}
											catch
											{
												ev.ReturnMessage = ("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
												return;
											}
											ev.Player.Scp079Data.AP -= 80;
											PluginManager.Manager.Server.Map.AnnounceNtfEntrance(int.Parse(args[3]), int.Parse(args[2]), args[1][0]);
											ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
											ev.Player.Scp079Data.Exp += (2.8f * (ev.Player.Scp079Data.Level + 1));
											Timing.Run(CooldownCassie(20.0f));
											Timing.Run(CooldownMTF(60.0f));
											ev.ReturnMessage = "Comando lanzado.";
											return;
										}
										else
										{
											ev.ReturnMessage = ("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
										}

									}
									else
									{
										ev.ReturnMessage = ("No tienes suficiente energía.");
									}
								}
								else
								{
									ev.ReturnMessage = ("No tienes suficiente nivel");
								}

								return;
							case "scp":
								if (args.Length >= 3)
								{
									string[] scpList = new string[5]
									{
										"173", "096", "106", "049", "939"
									};
									if (!scpList.Contains(args[1]))
									{
										ev.ReturnMessage = ("Pon un SCP que exista - Uso: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont)");
										return;
									}
									string scpNum = string.Join(" ", args[1].ToCharArray());
									switch (args[2])
									{
										case "mtf":
											Player dummy = null;
											List<Role> mtf = new List<Role>
										{
											Role.FACILITY_GUARD, Role.NTF_CADET, Role.NTF_LIEUTENANT, Role.NTF_SCIENTIST, Role.NTF_COMMANDER
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
												ev.Player.SendConsoleMessage("No hay MTFs vivos. Mandando como \"unknown\"", "red");
											}

											PluginManager.Manager.Server.Map.AnnounceScpKill(args[1], dummy);
											ev.ReturnMessage = "Comando lanzado.";
											break;
										case "unknown":
											PluginManager.Manager.Server.Map.AnnounceScpKill(args[1], null);
											ev.ReturnMessage = "Comando lanzado.";
											break;
										case "tesla":
											PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Successfully Terminated by automatic security systems");
											ev.ReturnMessage = "Comando lanzado.";
											break;
										case "decont":
											PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Lost in Decontamination Sequence");
											ev.ReturnMessage = "Comando lanzado.";
											break;
										default:
											ev.ReturnMessage = "Pon un SCP que exista - Uso: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont)";
											return;
									}
								}
								else
								{
									ev.ReturnMessage = ("Uso: .079 scp (173) (unknown)");
									return;
								}
								ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
								ev.Player.Scp079Data.Exp += (5.0f * (ev.Player.Scp079Data.Level + 1));
								Timing.Run(CooldownCassie(20.0f));
								ev.ReturnMessage = "Comando lanzado.";
								return;
							case "gen":
								if (cooldownGenerator || cooldownCassieGeneral)
								{
									ev.ReturnMessage = ("Tienes que esperar antes de volver a usar el comando");
									return;
								}
								if (ev.Player.Scp079Data.Level < 2)
								{
									ev.ReturnMessage = ("No tienes suficiente nivel");
									return;
								}
								switch (args[1])
								{
									case "1":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon1");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += 20f;
										Timing.Run(CooldownGen(20.0f));
										Timing.Run(CooldownCassie(10.5f));
										ev.ReturnMessage = "Comando lanzado.";
										return;
									case "2":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon2");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += 20f;
										Timing.Run(CooldownGen(20.0f));
										Timing.Run(CooldownCassie(10.5f));
										ev.ReturnMessage = "Comando lanzado.";
										return;
									case "3":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon3");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += 20f;
										Timing.Run(CooldownGen(20.0f));
										Timing.Run(CooldownCassie(10.5f));
										ev.ReturnMessage = "Comando lanzado.";
										return;
									case "4":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon4");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += 20f;
										Timing.Run(CooldownGen(20.0f));
										Timing.Run(CooldownCassie(10.5f));
										ev.ReturnMessage = "Comando lanzado.";
										return;
									case "5":
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										Timing.Run(Fingir5Gens());
										ev.Player.Scp079Data.Exp += 80f;
										Timing.Run(CooldownGen(180.0f));
										Timing.Run(CooldownCassie(10.5f));
										ev.ReturnMessage = ("Comando lanzado. Se reproducirá el mensaje de tu contención al completo, incluyendo cuando te matan y cuando se apagan/encienden las luces.");
										return;
									case "6":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += 50f;
										Timing.Run(CooldownGen(120.0f));
										Timing.Run(CooldownCassie(10.5f));
										Timing.Run(FakeKillPC(true));
										return;
									default:
										ev.ReturnMessage = ("Uso: .079 gen 1 - Sonará que hay 1 generador activado, del 1-5, pon 6 para mandar que has muerto (nivel 3)\nEl 5 falseará la sequencia completa de contención");
										return;
								}
							case "info":
								ev.Player.Scp079Data.AP = 0;
								Timing.Run(CooldownInfo(30f));
								if (infoCooldown)
								{
									ev.ReturnMessage = "Tienes que esperar antes de volver a usar el comando info";
									return;
								}
								string humansAlive = "[Bloqueado hasta nivel 2]";
								string tiempoDecont = "[Bloqueado hasta nivel 2]";
								string ScientistsEscaped = "[Bloqueado hasta nivel 3]";
								string ClassDEscaped = "[Bloqueado hasta nivel 3]";
								string ClassDAlive = "[Bloqueado hasta nivel 4]";
								string ScientistsAlive = "[Bloqueado hasta nivel 4]";
								string MTFAlive = "[Bloqueado hasta nivel 4]";
								string CiAlive = "[Bloqueado hasta nivel 4]";
								;
								if (ev.Player.Scp079Data.Level > 0)
								{
									humansAlive = (PluginManager.Manager.Server.Round.Stats.ClassDAlive + PluginManager.Manager.Server.Round.Stats.ScientistsAlive + PluginManager.Manager.Server.Round.Stats.CiAlive + PluginManager.Manager.Server.Round.Stats.NTFAlive).ToString();
									if (PluginManager.Manager.Server.Map.LCZDecontaminated == true) tiempoDecont = "Ya ha sido descontaminada";
									else tiempoDecont = (plugin.GetConfigFloat("decontamination_time") -
										float.Parse(PluginManager.Manager.Server.Round.Duration.ToString())/60.0f).ToString();
								}
								if (ev.Player.Scp079Data.Level > 1)
								{
									ClassDEscaped = PluginManager.Manager.Server.Round.Stats.ClassDEscaped.ToString();
									ScientistsEscaped = PluginManager.Manager.Server.Round.Stats.ScientistsEscaped.ToString();
								}
								if(ev.Player.Scp079Data.Level > 2)
								{
									ClassDAlive = PluginManager.Manager.Server.Round.Stats.ClassDAlive.ToString();
									ScientistsAlive = PluginManager.Manager.Server.Round.Stats.ScientistsAlive.ToString();
									MTFAlive = PluginManager.Manager.Server.Round.Stats.NTFAlive.ToString();
									CiAlive = PluginManager.Manager.Server.Round.Stats.CiAlive.ToString();
								}
								ev.Player.SendConsoleMessage("\nClase D escapados: "+ ClassDEscaped +
								"\nCientíficos escapados: " + ScientistsEscaped +
								"\nSCP vivos: " + PluginManager.Manager.Server.Round.Stats.SCPAlive +
								"\nHumanos vivos: " + humansAlive +
								"\nTiempo hasta la descontaminación: " + tiempoDecont +
								"\n\nClase D vivos: " + ClassDAlive +
								"\nCientíficos vivos: " + ScientistsAlive +
								"\nMTF vivos: " + MTFAlive +
								"\nChaos vivos: " + CiAlive 
								, "white");
								ev.ReturnMessage = "";
								return;
								
							default:
								MandaAyuda(ev.Player);
								return;
						}
					}
				}
				else
				{
					ev.ReturnMessage = ("¡No eres SCP-079!");
				}
			}
		}

		public static IEnumerable<float> DisableTesla(Smod2.API.TeslaGate tesla, float current)
		{
			tesla.TriggerDistance = -1.0f;
			yield return 8.0f;
			tesla.TriggerDistance = current;
		}
		/* // no usado por estar OP pero podría ser usado uwu
		public static IEnumerable<float> OverCharge()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Warning All Security Personnel Unauthorized Use Of Error Error Error Error pitch_2 error error error error pitch_0.5 error error");

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
		public static IEnumerable<float> FakeKillPC(bool cosilla)
		{
			// falta cerrar puertas
			yield return 7.3f;

			foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
			{
				try{ if (room.ZoneType == ZoneType.HCZ) room.FlickerLights(); }
				catch { }
			}
			foreach(Smod2.API.Door door in PluginManager.Manager.Server.Map.GetDoors()){
				
			}
			if (cosilla)
			{
				PluginManager.Manager.Server.Map.AnnounceCustomMessage("SCP 0 7 9 Contained Successfully");
			}
		}

		public static IEnumerable<float> Fingir5Gens()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon5");
			yield return 70.3f;
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
			Timing.Run(FakeKillPC(true));
		}

		public static IEnumerable<float> CooldownMTF(float time)
		{
			cooldownMTF = true;
			yield return time;
			cooldownMTF = false;
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (ev.Player.TeamRole.Role == Role.SCP_079)
			{
				Timing.Run(DelayBroadcastPC(ev.Player));
			}
		}

		public static IEnumerable<float> CooldownGen(float time)
		{
			cooldownGenerator = true;
			yield return time;
			cooldownGenerator = false;
		}

		public static IEnumerable<float> CooldownCassie(float time)
		{
			cooldownCassieGeneral = true;
			yield return time;
			cooldownCassieGeneral = false;
		}

		public static IEnumerable<float> CooldownInfo(float time)
		{
			cooldownCassieGeneral = true;
			yield return time;
			cooldownCassieGeneral = false;

		}

		public static IEnumerable<float> DelayBroadcastPC(Player pc)
		{
			yield return 0.5f;

			if (pc.TeamRole.Role == Role.SCP_079)
			{
				pc.PersonalBroadcast(20, "Presiona ñ para abrir la consola y usar comandos adicionales como desactivar teslas, fingir la muerte de un SCP...", true);
				MandaAyuda(pc);
			}
		}
	}
}
