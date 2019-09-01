using System;
using System.Collections.Generic;
using Pro079Core.API;
using Smod2;
using Smod2.API;

namespace Pro079Core
{
	public static class Pro079Logic
	{
		////////////////////////////////
		//			  HELP			  //
		////////////////////////////////
		private static List<string> Help;
		private static string FormatEnergyLevel(int energy, int level, string energStr, string lvlStr)
		{
			string str;
			if (energy > 0)
			{
				str = "(" + energStr.Replace("$ap", energy.ToString())
					+ (level > 1 ? ", " + lvlStr.Replace("$lvl", level.ToString()) : "") + ')';
				return str;
			}
			if (energy <= 0 && level > 1)
			{
				str = "(" + FirstCharToUpper(lvlStr.Replace("$lvl", level.ToString())) + ')';
				return str;
			}
			return string.Empty;
		}
		private static void FetchExternalHelp()
		{
			Help = new List<string>(Pro079.Manager.Commands.Keys.Count);
			foreach (KeyValuePair<string, ICommand079> kvp in Pro079.Manager.Commands)
			{
				if (!kvp.Value.Disabled) Help.Add($"<b>.079 {kvp.Key + (!string.IsNullOrEmpty(kvp.Value.ExtraArguments) ? " " + kvp.Value.ExtraArguments : string.Empty)}</b> - {kvp.Value.HelpInfo} {FormatEnergyLevel(kvp.Value.APCost, kvp.Value.MinLevel, Pro079.Instance.energy, Pro079.Instance.level)}");
			}
		}
		internal static string GetHelp()
		{
			string help = Pro079.Instance.basicHelp;
			if (Help == null || Help.Count != Pro079.Manager.Commands.Keys.Count) FetchExternalHelp();
			foreach (string line in Help)
			{
				help += Environment.NewLine + line;
			}
			if (Pro079.Instance.suicide) help += System.Environment.NewLine + $"<b>.079 {Pro079.Instance.suicidecmd}</b> - " + Pro079.Instance.suicidehelp;
			if (Pro079.Instance.ult) help += System.Environment.NewLine + $"<b>.079 {Pro079.Instance.ultcmd}</b> - " + Pro079.Instance.ulthelp;
			if (Pro079.Instance.tips) help += System.Environment.NewLine + $"<b>.079 {Pro079.Instance.tipscmd}</b> - " + Pro079.Instance.tipshelp;
			return help;
		}
		private static List<string> UltimateHelp;
		private static void FetchUltimates()
		{
			UltimateHelp = new List<string>(Pro079.Manager.Ultimates.Keys.Count);
			foreach (KeyValuePair<string, IUltimate079> kvp in Pro079.Manager.Ultimates)
			{
				UltimateHelp.Add($"<b>.079 {kvp.Key}</b> - {kvp.Value.Info} {Pro079.Instance.ultdata.Replace("$cd", kvp.Value.Cooldown.ToString()).Replace("$cost", kvp.Value.Cost.ToString())}");
			}
		}
		internal static string GetUltimates()
		{
			string help = Pro079.Instance.ultusageFirstline;
			if (UltimateHelp == null || UltimateHelp.Count != Pro079.Manager.Ultimates.Keys.Count) FetchUltimates();
			foreach (string line in UltimateHelp)
			{
				help += Environment.NewLine + " - " + line;
			}
			return help;
		}

		// This thing below was pasted from here: https://www.c-sharpcorner.com/blogs/first-letter-in-uppercase-in-c-sharp1
		internal static string FirstCharToUpper(string s)
		{
			// Check for empty string.
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			// Return char and concat substring.
			return char.ToUpper(s[0]) + s.Substring(1);
		}

		///////////////////////////////////////
		//	 		LOGIC FUNCTIONS			 //
		///////////////////////////////////////
		internal static IEnumerator<float> DelaySpawnMsg(Player player)
		{
			yield return 0.1f; // This value produces completely random outputs, but it's good enough for delaying the message a tiny bit so it doesn't overlap
			if (player.TeamRole.Role == Role.SCP_079)
			{
				player.PersonalClearBroadcasts();
				player.PersonalBroadcast(20, Pro079.Instance.broadcastMsg, true);
				player.SendConsoleMessage(GetHelp(), "white");
			}
		}
		/// <summary>
		/// Does the fake 6th gen
		/// </summary>
		public static IEnumerator<float> FakeDeath(Player player = null)
		{
			yield return MEC.Timing.WaitForSeconds(7.3f);
			foreach (FlickerableLight flickerableLight in EventHandlers.FlickerableLightsArray)
			{
				Scp079Interactable component = flickerableLight.GetComponent<Scp079Interactable>();
				if (component == null || component.currentZonesAndRooms[0].currentZone == "HeavyRooms")
				{
					flickerableLight.EnableFlickering(10f);
				}
			}
			foreach (Door door in EventHandlers.DoorArray)
			{
				Scp079Interactable component = door.GetComponent<Scp079Interactable>();
				if (component.currentZonesAndRooms[0].currentZone == "HeavyRooms" && door.isOpen && !door.locked)
				{
					door.ChangeState(true);
				}
			}
			yield return MEC.Timing.WaitForSeconds(11f);
			if (player != null) player.ChangeRole(Role.SPECTATOR);
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("SCP 0 7 9 ContainedSuccessfully"); // thanks to "El n*z* jud*o" (uh...) for helping me with this
		}
		/// <summary>
		/// Does the whole recontainment process
		/// </summary>
		public static IEnumerable<float> Fake5Gens()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon5");
			yield return MEC.Timing.WaitForSeconds(79.89f);
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
			int p = (int)System.Environment.OSVersion.Platform;
			if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(FakeDeath(null), MEC.Segment.Update);
			else MEC.Timing.RunCoroutine(FakeDeath(null), 1);
		}

		internal static IEnumerator<float> CooldownCassie(float time)
		{
			if (time > 5)
			{
				yield return MEC.Timing.WaitForSeconds(time);

				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(3, Pro079.Instance.cassieready, false);
				}
			}
		}
		internal static IEnumerator<float> DelayKysMessage(List<Player> PCplayers)
		{
			if (string.IsNullOrEmpty(Pro079.Instance.kys)) yield break;
			yield return 0.3f;
			if (PluginManager.Manager.Server.Round.Stats.SCPAlive + PluginManager.Manager.Server.Round.Stats.Zombies - PCplayers.Count == 0)
			{
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(20, Pro079.Instance.kys, false);
				}
			}
		}
	}
}
