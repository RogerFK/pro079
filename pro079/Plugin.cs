using scp4aiur;
using Smod2;
using Smod2.Attributes;
using Smod2.Config;

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
        SmodRevision = 0
        )]

    public class pro079 : Plugin
    {
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
            AddEventHandlers(new Pro79Handlers(this));
            AddConfig(new ConfigSetting("p079_broadcast_enable", true, true, ""));
            AddConfig(new ConfigSetting("p079_broadcast_msg", "<color=#85ff4c>Presiona ñ para abrir la consola y usar comandos adicionales</color>", true, ""));
            Timing.Init(this);
        }
    }
}
