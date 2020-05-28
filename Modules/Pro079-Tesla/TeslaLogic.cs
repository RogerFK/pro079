using System;
using System.Collections.Generic;
using Smod2;
using Smod2.API;
using SMRole = Smod2.API.RoleType;

namespace TeslaCommand
{
	internal static class TeslaLogic
	{
		internal static IEnumerator<float> DisableTeslas(float time, TeslaPlugin plugin)
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
			int remTime = plugin.remaining;
			yield return MEC.Timing.WaitForSeconds(time - remTime);
			foreach (Smod2.API.Player player in PluginManager.Manager.Server.GetPlayers())
			{
				string remainingStr = plugin.teslarem;
				if (player.TeamRole.Role == SMRole.SCP_079)
				{
					for (i = remTime; i > 0; i--)
					{
						player.PersonalBroadcast(1, Environment.NewLine + remainingStr.Replace("$sec", "<b>" + i.ToString() + "</b>"), false);
					}
					player.PersonalBroadcast(5, Environment.NewLine + plugin.GetTranslation("teslarenabled"), false);
				}
			}
			yield return MEC.Timing.WaitForSeconds(remTime);

			for (i = 0; i < length; i++)
			{
				teslas[i].sizeOfTrigger = distances[i];
			}
		}
	}
}
