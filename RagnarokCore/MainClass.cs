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
        public override Version Version { get; } = new Version(0, 1, 1);
        public override Version RequiredExiledVersion { get; } = new Version(5, 1, 3);

        public EventosHandlers Ev;
        public Harmony Harmony { get; private set; }

        public override void OnEnabled()
        {
            Singleton = this;
            Ev = new EventosHandlers(this);

            PlayerHandler.ChangingGroup += Ev.OnChangingGroup;
            ServerHandler.RespawningTeam += Ev.OnRespawningChaos;
            Scp330Handler.EatingScp330 += Ev.OnEating330;
            PlayerHandler.Banning += Ev.OnBanning;
            PlayerHandler.ChangingRole += Ev.OnChangingRole;
            PlayerHandler.Hurting += Ev.OnHurting;
            PlayerHandler.UsedItem += Ev.OnUsedItem;
            PlayerHandler.PickingUpItem += Ev.OnPickingUpItem;
            PlayerHandler.PickingUpScp330 += Ev.OnPickingUp330;
            PlayerHandler.PickingUpArmor += Ev.OnPickingUpArmor;
            PlayerHandler.PickingUpAmmo += Ev.OnPickingUpAmmo;
            PlayerHandler.InteractingDoor += Ev.OnInteractingDoor;
            PlayerHandler.InteractingLocker += Ev.OnInteractingLocker;
                
            base.OnEnabled();
            Patch();
        }
        public override void OnDisabled()
        {
            PlayerHandler.ChangingGroup -= Ev.OnChangingGroup;
            ServerHandler.RespawningTeam -= Ev.OnRespawningChaos;
            Scp330Handler.EatingScp330 -= Ev.OnEating330;
            PlayerHandler.Banning -= Ev.OnBanning;
            PlayerHandler.ChangingRole -= Ev.OnChangingRole;
            PlayerHandler.Hurting -= Ev.OnHurting;
            PlayerHandler.UsedItem -= Ev.OnUsedItem;
            PlayerHandler.PickingUpItem -= Ev.OnPickingUpItem;
            PlayerHandler.PickingUpScp330 -= Ev.OnPickingUp330;
            PlayerHandler.PickingUpArmor -= Ev.OnPickingUpArmor;
            PlayerHandler.PickingUpAmmo -= Ev.OnPickingUpAmmo;
            PlayerHandler.InteractingDoor -= Ev.OnInteractingDoor;
            PlayerHandler.InteractingLocker -= Ev.OnInteractingLocker;

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
