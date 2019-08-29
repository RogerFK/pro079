using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.API;

namespace Pro079_Tesla
{
	class TeslaLogic
	{
		private static IEnumerable<float> DisableTeslas(float time)
		{
			TeslaGate[] teslas = UnityEngine.Object.FindObjectsOfType<TeslaGate>();
			int length = teslas.Length;
			float[] distances = new float[teslas.Length];
			int i;

			for (i = 0; i < length; i++)
			{
				distances[i] = teslas[i].sizeOfTrigger;
				teslas[i].sizeOfTrigger = -1f;
			}
			int remTime = plugin.GetConfigInt("p079_tesla_remaining");
			yield return time - remTime;
			foreach (Smod2.API.Player player in PluginManager.Manager.Server.GetPlayers())
			{
				string remainingStr = plugin.GetTranslation("teslarem");
				if (player.TeamRole.Role == Role.SCP_079)
				{
					for (i = remTime; i > 0; i--)
					{
						player.PersonalBroadcast(1, Environment.NewLine + remainingStr.Replace("$sec", "<b>" + i.ToString() + "</b>"), false);
					}
					player.PersonalBroadcast(5, Environment.NewLine + plugin.GetTranslation("teslarenabled"), false);
				}
			}
			yield return remTime;

			for (i = 0; i < length; i++)
			{
				teslas[i].sizeOfTrigger = distances[i];
			}
		}
	}
}
