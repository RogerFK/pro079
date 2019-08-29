using MEC;
using System.Collections.Generic;
using Pro079.API;
using Smod2.API;
using Smod2;

namespace Pro079
{
	public class Manager
	{
		private readonly Pro079 plugin;
		public Manager(Pro079 plugin) => this.plugin = plugin;

		/// <summary>
		/// Dictionary with all the Commands and their respective handlers
		/// </summary>
		public readonly Dictionary<string, ICommand079> Commands = new Dictionary<string, ICommand079>();
		/// <summary>
		/// Function used to register the current command. Doesn't register EventHandlers, so be aware of that.
		/// </summary>
		/// <param name="CommandHandler">The class that implements ICommand079</param>
		/// <returns></returns>
		public string RegisterCommand(ICommand079 CommandHandler)
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
		public readonly Dictionary<string, IUltimate079> Ultimates = new Dictionary<string, IUltimate079>();
		/// <summary>
		/// Function used to register the current command. Doesn't register EventHandlers, so be aware of that.
		/// </summary>
		/// <param name="UltimateHandler">The class that implements ICommand079</param>
		/// <returns></returns>
		public string RegisterUltimate(IUltimate079 UltimateHandler)
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
		public void SetOnCooldown(ICommand079 Command)
		{
			Command.CurrentCooldown = PluginManager.Manager.Server.Round.Duration + Command.Cooldown;
			if (!string.IsNullOrEmpty(Command.CommandReady))
			{
				int p = (int) System.Environment.OSVersion.Platform;
				if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(DelayMessage(Command.CommandReady, Command.Cooldown), MEC.Segment.Update);
				else MEC.Timing.RunCoroutine(DelayMessage(Command.CommandReady, Command.Cooldown), 1);
			}
		}
		private IEnumerator<float> DelayMessage(string message, int delay)
		{
			yield return MEC.Timing.WaitForSeconds(delay);
			var pcs = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
			foreach (Player pc in pcs) pc.PersonalBroadcast(6, message, false);
		}
	}
}
