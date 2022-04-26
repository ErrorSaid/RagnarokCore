using System;
using Exiled.Events.EventArgs;
using RagnarokCore.Componentes;
using System.Collections.Generic;
using System.Linq;
using Respawning;
using Exiled.API.Features;
using CustomPlayerEffects;
using Exiled.API.Enums;

namespace RagnarokCore
{
    public class EventosHandlers
    {
        public readonly MainClass plugin;
        public EventosHandlers(MainClass plugin)
        {
            this.plugin = plugin;
        }
        private bool TryGetColors(string rank, out string[] availableColors)
        {
            availableColors = plugin.Config.Sequences;
            return !string.IsNullOrEmpty(rank) && plugin.Config.RoleRainbowTags.Contains(rank);
        }
        private bool EqualsTo(UserGroup @this, UserGroup other)
        {
            return @this.BadgeColor == other.BadgeColor && @this.BadgeText == other.BadgeText && @this.Permissions == other.Permissions && @this.Cover == other.Cover && @this.HiddenByDefault == other.HiddenByDefault && @this.Shared == other.Shared && @this.KickPower == other.KickPower && @this.RequiredKickPower == other.RequiredKickPower;
        }
        private string GetGroupKey(UserGroup group)
        {
            if (group == null)
            {
                return string.Empty;
            }
            return ServerStatic.PermissionsHandler._groups.FirstOrDefault((KeyValuePair<string, UserGroup> g) => this.EqualsTo(g.Value, group)).Key ?? string.Empty;
        }

        public void OnChangingGroup(ChangingGroupEventArgs ev)
        {
            if (!ev.IsAllowed)
            {
                return;
            }
            string[] colors;
            bool flag = this.TryGetColors(this.GetGroupKey(ev.NewGroup), out colors);
            if (ev.NewGroup != null && ev.Player.Group == null && flag)
            {
                RtController rtController = ev.Player.GameObject.AddComponent<RtController>();
                rtController.Colors = colors;
                rtController.Interval = plugin.Config.ColorInterval;
                return;
            }
            if (flag)
            {
                ev.Player.GameObject.GetComponent<RtController>().Colors = colors;
                return;
            }
            UnityEngine.Object.Destroy(ev.Player.GameObject.GetComponent<RtController>());
        }
        public void OnRespawningChaos(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
            {
                Cassie.Message(plugin.Config.ChaosCassie, false, true, true);
            }
        }
        public void OnEating330(EatingScp330EventArgs ev)
        {
            if (plugin.Config.Check330Eating)
            {
                Log.Debug($"{ev.Player.Nickname} se comio un caramelo: {ev.Candy.Kind}");
            }
        }
        public void OnBanning(BanningEventArgs ev)
        {
            Map.Broadcast(10, $"<i><color=blue>{ev.Target.Nickname}</color> fue baneado por <color=red>{ev.Issuer.Nickname}</color> | <color=green>{ev.Reason}</color></i>");
        }

        public void OnChangingRole(ChangingRoleEventArgs _)
        {
            // Quita el noclip, invis, god y Bypass al cambiar de clase
            _.Player.IsInvisible = false;
            _.Player.IsGodModeEnabled = false;
            _.Player.NoClipEnabled = false;
            _.Player.IsBypassModeEnabled = false;
            
            // CustomInfo para los que son o han sido tutos
            if (_.Player.Role.Type == RoleType.Tutorial)
                _.Player.CustomInfo = "Soy gay por haber sido tuto o por serlo";
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Target == null && ev.Attacker == null)
                return;

            if (ev.Target.GetEffectActive<Invisible>())
                ev.IsAllowed = false;

            if (ev.Attacker.Role.Type == RoleType.Tutorial)
            {
                ev.IsAllowed = false;
                ev.Attacker.ShowHint("No puedes hacerle daños a los demas siendo tutorial", 5f);
            }

            switch (ev.Handler.Type)
            {
                case DamageType.Explosion:
                {
                    ev.Amount = 400;
                    break;
                }
                case DamageType.Scp0492:
                {
                    ev.Amount = 40;
                    break;
                }
                case DamageType.Scp939:
                {
                    if (MainClass.Singleton.Config.Scp939InstaKill)
                    {
                        ev.Amount = 1000;
                    }
                    break;
                }
            }
        }

        public void OnUsedItem(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.Painkillers)
                ev.Player.ResetStamina();
            if (ev.Item.Type == ItemType.SCP500)
                ev.Player.ArtificialHealth = 100f;
        }
    }
}
