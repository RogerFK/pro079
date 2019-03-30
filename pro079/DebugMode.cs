using Smod2.Commands;
using Smod2.API;

namespace pro079
{
	class DebugMode : ICommandHandler
	{
		private readonly pro079 plugin;

		public DebugMode(pro079 plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			throw new System.NotImplementedException();
		}

		public string GetUsage()
		{
			throw new System.NotImplementedException();
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender is Player p)
			{
				p.Scp079Data.APPerSecond = 99999f;
				p.Scp079Data.Level = 4;
				p.Scp079Data.APPerSecond = 99999f;
			}
			return new string[] { "mi poya literal" };
		}
	}
}
