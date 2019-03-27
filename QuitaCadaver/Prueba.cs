using Smod2.API;
using Smod2.Commands;

namespace pro079
{
	internal class Prueba : ICommandHandler
	{
		private pro079 pro079;

		public Prueba(pro079 pro079)
		{
			this.pro079 = pro079;
		}

		public string GetCommandDescription()
		{
			return "tonto";
		}

		public string GetUsage()
		{
			return "poya";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if(sender is Player player)
			{
				player.Scp079Data.AP = 999999.0f;
				player.Scp079Data.Level = 4;
			}
			return new string[] { "Vuamos" };
		}
	}
}