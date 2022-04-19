using System;
using Exiled.Events.EventArgs;
using RagnarokCore.Componentes;
using System.Collections.Generic;
using System.Linq;
using Respawning;
using Exiled.API.Features;

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
                Cassie.Message(plugin.Config.ChaosCassie, false, false, true);
            }
        }
        public void OnEating330(EatingScp330EventArgs ev)
        {
            if (plugin.Config.Check330Eating)
            {
                Log.Debug($"{ev.Player.Nickname} se comio un caramelo: {ev.Candy}");
            }
        }
        public void OnBanning(BanningEventArgs ev)
        {
            Map.Broadcast(10, $"<i><color=blue>{ev.Target.Nickname}</color> fue baneado por <color=red>{ev.Issuer.Nickname}</color> | <color=green>{ev.Reason}</color></i>");
        }

        public void OnChangingRole(ChangingRoleEventArgs _)
        {
            // Fix al no poder quitarte el invis una vez activado y el NoClip
            _.Player.IsInvisible = false;
            _.Player.IsGodModeEnabled = false;
            _.Player.NoClipEnabled = false;
            
            // CustomInfo para los que son o han sido tutos
            if (_.Player.Role.Type == RoleType.Tutorial)
                _.Player.CustomInfo = "Soy gay por haber sido tuto o por serlo";
        }
    }
}
