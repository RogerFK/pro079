using Smod2.Commands;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.API;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Threading.Tasks;
using Smod2;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using scp4aiur;
using Smod2.EventSystem.Events;

namespace pro079
{
	class Pro79Handlers : IEventHandlerCallCommand, IEventHandlerSetRole//, IEventHandlerDisconnect, IEventHandlerPlayerDie
	{
		private readonly pro079 plugin;
		public Pro79Handlers(pro079 plugin)
		{
			this.plugin = plugin;
		}
		public static bool cooldownGenerator = false;
		public static bool cooldownCassieGeneral = false;
		private bool alreadySent;

		private static void MandaAyuda(Player player)
		{
			player.SendConsoleMessage("Estos son los comandos que tienes disponibles:\n" +
			".079 - Muestra este mensaje de ayuda\n" +
			".079 te - Desactiva la tesla de la habitación en la que estás durante 8 segundos (50 puntos)\n" +
			".079 mtf <letra> <numero> <scp-vivos> - Lanza un mensaje sobre que ha llegado la MTF a la zona con un número falso de SCPs con vida, o el actual si no (100 puntos, nivel 2)\n" +
			".079 gen [1-5] - Manda el mensaje de que X generadores han sido activados, o manda con un 6 para fingir tu muerte\n" +
			"Si mandas .079 gen 5, activarás la secuencia para fingir que estás siendo contenido\n" +
			".079 scp <###> <motivo> - Manda un mensaje de muerte de SCP con el número, el motivo puede ser: unknown, tesla, mtf, decont\n" +
			".079 suicidio - Sobrecarga los generadores para morir cuando quedes tú solo",
			"white");
		}

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			string command = ev.Command.ToLower();

			if (command.StartsWith("079"))
			{
				MatchCollection collection = new Regex("[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'").Matches(command);
				string[] args = new string[collection.Count - 1];

				for (int i = 1; i < collection.Count; i++)
				{
					// If it's wrapped in quotes, 
					if (collection[i].Value[0] == '\"' && collection[i].Value[collection[i].Value.Length - 1] == '\"')
					{
						args[i - 1] = collection[i].Value.Substring(1, collection[i].Value.Length - 2);
					}
					else
					{
						args[i - 1] = collection[i].Value;
					}
				}
				if (ev.Player.TeamRole.Role == Role.SCP_079)
				{
					if (args.Length == 0)
					{
						MandaAyuda(ev.Player);
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
												ev.Player.Scp079Data.Exp += (10.0f/ev.Player.Scp079Data.Level);
												Timing.Run(DisableTesla(tesla, tesla.TriggerDistance));
												noTesla = false;
												break;
											}
											else ev.Player.SendConsoleMessage("Esta Tesla está desactivada.", "white");
										}
									}
									if (noTesla == true) ev.Player.SendConsoleMessage("No estás cerca de una Tesla.", "white");
								}
								else ev.Player.SendConsoleMessage("No tienes suficiente energía (necesitas 50).", "white");
								return;
							case "suicidio":
								if ((PluginManager.Manager.Server.Round.Stats.Zombies + PluginManager.Manager.Server.Round.Stats.SCPAlive) != 1)
								{
									ev.Player.SendConsoleMessage("No puedes suicidarte cuando hay más SCP vivos", "white");
									return;
								}
								PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
								FakeKillPC(false);
								ev.Player.Kill(DamageType.NUKE);
								return;
							case "mtf":
								ev.Player.SendConsoleMessage("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
								return;
							case "scp":
								ev.Player.SendConsoleMessage("Uso: .079 scp (173) (unknown)");
								return;
							case "gen":
								ev.Player.SendConsoleMessage("Uso: .079 gen (1) - Sonará que hay 1 generador activado");
								return;
							default:
								ev.Player.SendConsoleMessage("Comando no reconocido. Usa .079 para ayuda", "white");
								return;
						}
					}
					else if (args.Length > 1)
					{
						if (cooldownCassieGeneral)
						{
							ev.Player.SendConsoleMessage("Tienes que esperar antes de volver a usar un comando que requiera a cassie", "white");
							return;
						}
						switch (args[0])
						{
							case "mtf":
								if (ev.Player.Scp079Data.AP >= 80)
								{
									if (ev.Player.Scp079Data.Level >= 1)
									{
										if (args.Length == 4)
										{
											try
											{
												int.Parse(args[3]);
												int.Parse(args[2]);
												if (!char.IsLetter(args[1][0]))
												{
													ev.Player.SendConsoleMessage("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
													return;
												}
											}
											catch
											{
												ev.Player.SendConsoleMessage("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
												return;
											}
											ev.Player.Scp079Data.AP -= 80;
											PluginManager.Manager.Server.Map.AnnounceNtfEntrance(int.Parse(args[3]), int.Parse(args[2]), args[1][0]);
											ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
											ev.Player.Scp079Data.Exp += (5.0f / ev.Player.Scp079Data.Level);
											Timing.Run(CooldownCassie(25.0f));
										}
										else
										{
											ev.Player.SendConsoleMessage("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
										}
									}
									else ev.Player.SendConsoleMessage("No tienes suficiente nivel", "white");
								}
								else ev.Player.SendConsoleMessage("No tienes suficiente energía.", "white");

								return;

							case "scp":
								if (args.Length >= 3)
								{

									if (args[1].Length > 3)
									{
										ev.Player.SendConsoleMessage("No puedes poner más de 3 caracteres - Uso: .079 scp (173) (unknown)");
										return;
									}
									string scp;
									switch (args[1].Length)
									{
										case 1:
											scp = "00" + args[1];
											break;
										case 2:
											scp = "0" + args[1];
											break;
										default:
											scp = args[1];
											break;
									}
									if (args[2] == "mtf")
									{
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
										if (dummy == null) ev.Player.SendConsoleMessage("No hay MTFs vivos. Mandando como \"unknown\"", "white");
										PluginManager.Manager.Server.Map.AnnounceScpKill(scp, dummy);
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += (5.0f / ev.Player.Scp079Data.Level);
										Timing.Run(CooldownCassie(15.0f));
									}
									else if (args[2] == "unknown")
									{
										PluginManager.Manager.Server.Map.AnnounceScpKill(scp, null);
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += (5.0f / ev.Player.Scp079Data.Level);
										Timing.Run(CooldownCassie(15.0f));
									}
									else if (args[2] == "tesla")
									{
										string[] cutre = new string[20];
										for (int i = 0; i < scp.Length; i++)
										{
											cutre[i] = scp[i].ToString();
										}
										string scpNum = String.Join(" ", cutre);
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Successfully Terminated by automatic security systems");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += (5.0f / ev.Player.Scp079Data.Level);
										Timing.Run(CooldownCassie(15.0f));
									}
									else if (args[2] == "decont")
									{
										string[] cutre = new string[20];
										for (int i = 0; i < scp.Length; i++)
										{
											cutre[i] = scp[i].ToString();
										}
										string scpNum = String.Join(" ", cutre);
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Lost in Decontamination Sequence");
										ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
										ev.Player.Scp079Data.Exp += (5.0f / ev.Player.Scp079Data.Level);
										Timing.Run(CooldownCassie(15.0f));
									}
								}
								else ev.Player.SendConsoleMessage("Uso: .079 scp (173) (unknown)");
								return;
							case "gen":
								if (cooldownGenerator)
								{
									ev.Player.SendConsoleMessage("Tienes que esperar antes de volver a usar el comando", "white");
									return;
								}
								if (ev.Player.Scp079Data.Level < 2)
								{
									ev.Player.SendConsoleMessage("No tienes suficiente nivel", "white");
									return;
								}
								switch (args[1])
								{
									case "1":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon1");
										Timing.Run(CooldownGen(20.0f));
										Timing.Run(CooldownCassie(10.5f));
										return;
									case "2":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon2");
										Timing.Run(CooldownGen(20.0f));
										Timing.Run(CooldownCassie(10.5f));
										return;
									case "3":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon3");
										Timing.Run(CooldownGen(20.0f));
										Timing.Run(CooldownCassie(10.5f));
										return;
									case "4":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon4");
										Timing.Run(CooldownGen(20.0f));
										Timing.Run(CooldownCassie(10.5f));
										return;
									case "5":
										Timing.Run(Fingir5Gens());
										Timing.Run(CooldownGen(180.0f));
										Timing.Run(CooldownCassie(10.5f));
										return;
									case "6":
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
										Timing.Run(CooldownGen(120.0f));
										Timing.Run(CooldownCassie(10.5f));
										Timing.Run(FakeKillPC(true));
										return;
									default:
										ev.Player.SendConsoleMessage("Uso: .079 gen 1 - Sonará que hay 1 generador activado, del 1-5, pon 6 para mandar que has muerto (nivel 3)");
										return;
								}
							default:
								MandaAyuda(ev.Player);
								return;
						}
					}
				}
				else
				{
					ev.Player.SendConsoleMessage("¡No eres SCP-079!", "red");
				}
			}
		}

		public static IEnumerable<float> DisableTesla(Smod2.API.TeslaGate tesla, float current)
		{
			tesla.TriggerDistance = -1.0f;
			yield return 8.0f;
			tesla.TriggerDistance = current;
		}
		public static IEnumerable<float> FakeKillPC(bool cosilla)
		{
			yield return 7.3f;

			foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
			{
				try
				{
					room.FlickerLights();
				}
				catch { }
			}
			if (cosilla) PluginManager.Manager.Server.Map.AnnounceScpKill("079", null);
		}

		public static IEnumerable<float> Fingir5Gens()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon5");
			yield return 70.3f;
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
			Timing.Run(FakeKillPC(true));
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
		public static IEnumerable<float> DelayBroadcastPC(Player pc)
		{
			yield return 0.5f;

			if (pc.TeamRole.Role == Role.SCP_079)
			{
				pc.PersonalBroadcast(20, "Presiona Ñ para abrir la consola y usar comandos adicionales como desactivar teslas, fingir la muerte de un SCP...", true);
				MandaAyuda(pc);
			}
		}
		/*
		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			//esto seguramente esté mal
			if ((PluginManager.Manager.Server.Round.Stats.Zombies + PluginManager.Manager.Server.Round.Stats.SCPAlive) == 1 && !alreadySent)
			{
				foreach(Player player in PluginManager.Manager.Server.GetPlayers(Role.SCP_079))
				{
					player.PersonalBroadcast(20, "Pulsa la Ñ y pon \".079 suicidio\" si quieres suicidarte.", false);
					alreadySent = true;
				}
			}
		}

		public void OnDisconnect(DisconnectEvent ev)
		{
			if ((PluginManager.Manager.Server.Round.Stats.Zombies + PluginManager.Manager.Server.Round.Stats.SCPAlive) == 1 && !alreadySent)
			{
				foreach (Player player in PluginManager.Manager.Server.GetPlayers(Role.SCP_079))
				{
					player.PersonalBroadcast(20, "Pulsa la Ñ y pon \".079 suicidio\" si quieres suicidarte.", false);
					alreadySent = true;
				}
			}
		}*/
	}
}
