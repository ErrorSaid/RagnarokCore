using System;
using Exiled.API.Features;
using HarmonyLib;
using PlayerHandler = Exiled.Events.Handlers.Player;
using ServerHandler = Exiled.Events.Handlers.Server;
using Scp330Handler = Exiled.Events.Handlers.Scp330;

namespace RagnarokCore
{
    public class MainClass : Plugin<Config>
    {
        public static MainClass Singleton;
        public override string Author { get; } = "xNexus-ACS";
        public override string Name { get; } = "RagnarokCore";
        public override string Prefix { get; } = "ragnarok_core";
        public override Version Version { get; } = new Version(0, 1, 0);
        public override Version RequiredExiledVersion { get; } = new Version(5, 0, 0);

        public EventosHandlers Ev;
        public Harmony Harmony { get; private set; }

        public override void OnEnabled()
        {
            Singleton = this;
            Ev = new EventosHandlers(this);

            PlayerHandler.ChangingGroup += Ev.OnChangingGroup;
            ServerHandler.RespawningTeam += Ev.OnRespawningChaos;
            Scp330Handler.EatingScp330 += Ev.OnEating330;

            base.OnEnabled();
            Patch();
        }
        public override void OnDisabled()
        {
            PlayerHandler.ChangingGroup -= Ev.OnChangingGroup;
            ServerHandler.RespawningTeam -= Ev.OnRespawningChaos;
            Scp330Handler.EatingScp330 -= Ev.OnEating330;

            Singleton = null;
            Ev = null;
            base.OnDisabled();
            Unpatch();
        }
        public void Patch()
        {
            try
            {
                Harmony = new Harmony("xNexus.RagnarokCore");
                Log.Info("Patch Completado sin errores");
                Harmony.PatchAll();
            }
            catch (Exception e)
            {
                Log.Error($"Ha ocurrido un error durante el Patch : {e}");
            }
        }
        public void Unpatch()
        {
            try
            {
                Log.Info("Unpatch completado sin errores");
                Harmony.UnpatchAll(this.Harmony.Id);
            }
            catch (Exception a)
            {
                Log.Error($"Ha ocurrido un error al unpatchear : {a}");
            }
        }
    }
}
