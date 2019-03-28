﻿using System.Collections.Generic;
using scp4aiur;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.EventHandlers;

namespace pro079
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079",
		description = "Ahora es más divertido el jueguito eh",
		id = "rogerfk.pro079",
		version = "1.0",
		SmodMajor = 3,
		SmodMinor = 3,
		SmodRevision = 1
		)]

	public class pro079 : Plugin
	{
		public bool probando = false;
		public override void OnDisable()
		{
			this.Info(this.Details.name + " se fue a la puta :(");
		}

		public override void OnEnable()
		{
			this.Info(this.Details.name + " ha cargado. Verás el lag");
		}

		public override void Register()
		{
			// Register Command(s)
			this.AddEventHandler(typeof(IEventHandlerCallCommand), new Pro79Handlers(this), Smod2.Events.Priority.Lowest);
			AddEventHandler(typeof(IEventHandlerSetRole), new Pro79Handlers(this));
			//AddEventHandler(typeof(IEventHandlerPlayerDie), new Pro79Handlers(this)); //no sé por qué no funciona :pepeshrug:
			//AddEventHandler(typeof(IEventHandlerDisconnect), new Pro79Handlers(this));
			Timing.Init(this);
		}
	}
}
