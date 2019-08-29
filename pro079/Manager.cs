using MEC;
using System.Collections.Generic;
using pro079.API;
using Smod2.API;
using Smod2;

namespace pro079
{
	class Manager
	{
		/// <summary>
		/// Dictionary with all the Commands and their respective handlers
		/// </summary>
		public readonly static Dictionary<string, ICommand079> Commands = new Dictionary<string, ICommand079>();
		/// <summary>
		/// Function used to register the current command. Doesn't register EventHandlers, so be aware of that.
		/// </summary>
		/// <param name="CommandHandler">The class that implements ICommand079</param>
		/// <returns></returns>
		public static string RegisterCommand(ICommand079 CommandHandler)
		{
			if(CommandHandler == null || string.IsNullOrEmpty(CommandHandler.Command))
			{
				return "Trying to register a \"null\" or an empty Command is not allowed!";
			}
			if (Commands.ContainsKey(CommandHandler.Command))
			{
				return "You can't register the same command twice!";
			}
			Commands.Add(CommandHandler.Command, CommandHandler);
			return "Command succesfully added";
		}
		/// <summary>
		/// Dictionary with all the Ultimates and their respective handlers
		/// </summary>
		public readonly static Dictionary<string, IUltimate079> Ultimates = new Dictionary<string, IUltimate079>();
		/// <summary>
		/// Function used to register the current command. Doesn't register EventHandlers, so be aware of that.
		/// </summary>
		/// <param name="UltimateHandler">The class that implements ICommand079</param>
		/// <returns></returns>
		public static string RegisterUltimate(IUltimate079 UltimateHandler)
		{
			if (UltimateHandler == null || string.IsNullOrEmpty(UltimateHandler.Name))
			{
				return "Trying to register a \"null\" or an empty name is not allowed!";
			}
			if (Ultimates.ContainsKey(UltimateHandler.Name))
			{
				return "An ultimate called " + UltimateHandler.Name + " was already added!";
			}
			Ultimates.Add(UltimateHandler.Name, UltimateHandler);
			return "Ultimate succesfully added";
		}

		/// <summary>
		/// Properly sets the cooldown for the given command
		/// </summary>
		/// <param name="Command"></param>
		public static void SetOnCooldown(ICommand079 Command)
		{
			Command.CurrentCooldown = PluginManager.Manager.Server.Round.Duration + Command.Cooldown;
			if (!string.IsNullOrEmpty(Command.CommandReady))
			{
				int p = (int) System.Environment.OSVersion.Platform;
				if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(DelayMessage(Command.CommandReady, Command.Cooldown), MEC.Segment.Update);
				else MEC.Timing.RunCoroutine(DelayMessage(Command.CommandReady, Command.Cooldown), 1);
			}
		}
		private static IEnumerator<float> DelayMessage(string message, int delay)
		{
			yield return MEC.Timing.WaitForSeconds(delay);
			var pcs = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
			foreach (Player pc in pcs) pc.PersonalBroadcast(6, message, false);
		}
		private static List<string> Help;
		private static void FormatHelp()
		{
			Help = new List<string>(Commands.Keys.Count);
			foreach (KeyValuePair<string, ICommand079> kvp in Commands)
			{
				if(!kvp.Value.Disabled) Help.Add($"<b>.079 {kvp.Key} - {kvp.Value.HelpInfo}");
			}
		}
		public static string GetHelp()
		{
			string help = Pro079.singleton.basicHelp;
			if (Help == null || Help.Count != Commands.Keys.Count) FormatHelp();
			foreach(string line in Help)
			{
				help += System.Environment.NewLine + line;
			}
			// Faltan suicidio, ultimates y tips
			if(Pro079.singleton.suicide)
			help += System.Environment.NewLine + $"<b>.079 {/*suicide*/0} - ";
			return help;
		}
	}
}
