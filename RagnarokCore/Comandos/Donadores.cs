using System;
using Exiled.API.Features;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.Permissions.Extensions;
using MEC;
using System.Collections.Generic;
using System.Linq;

namespace RagnarokCore.Comandos
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DonadorParent : ParentCommand
    {
        public DonadorParent() => LoadGeneratedCommands();

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new SetTutorial());
            RegisterCommand(new GiveSpecialItem());
            RegisterCommand(new Invisible());
            RegisterCommand(new NoClip());
            RegisterCommand(new DonadorChat());
        }

        public string SubCommands => "\ntutorial: Transformate en Tutorial/Espectador \nspecialitem: Date un objeto especial mientras eres tutorial \ninvisible: Vuelvete invisible \nnc: Activate el poder volar como dios \ndchat: chat de donadores";
        public override string Command => "donador";
        public override string[] Aliases => new[] {"d"};
        public override string Description => "Comandos de donadores";

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"SubComandos disponibles: {SubCommands}";
            return true;
        }
    }

    public class SetTutorial : ICommand
    {
        public string Command => "tutorial";
        public string[] Aliases => null;
        public string Description => "Cambia tu role a tutorial siendo espectador";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ragnarok.donadores"))
            {
                response = "No tienes permisos";
                return false;
            }
            Player ply = Player.Get((CommandSender)sender);

            if (ply.Role.Type != RoleType.Spectator)
            {
                response = "No eres espectador";
                return false;
            }

            if (ply.Role.Type == RoleType.Spectator)
            {
                ply.SetRole(RoleType.Tutorial, SpawnReason.ForceClass);
                response = "Ahora eres tutorial";
                return true;
            }

            if (ply.Role.Type == RoleType.Tutorial)
            {
                ply.SetRole(RoleType.Spectator, SpawnReason.ForceClass);
                response = "Ahora eres espectador";
                return true;
            }

            response = "Stacktrace: Fallo en el comando, reporta esto con una captura al Dev del servidor";
            return false;
        }
    }

    public class GiveSpecialItem : ICommand
    {
        public string Command => "specialitem";
        public string[] Aliases => null;
        public string Description => "Date un objeto especial mientras eres tutorial";

        public bool _item = true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ragnarok.donadores"))
            {
                response = "No tienes permisos";
                return false;
            }
            Player ply = Player.Get((CommandSender)sender);
            
            if (ply.Role.Type != RoleType.Tutorial)
            {
                response = "No eres espectador";
                return false;
            }

            if (ply.Role.Type == RoleType.Tutorial && _item)
            {
                ply.AddItem(ItemType.SCP244b, 2);
                _item = false;

                Timing.CallDelayed(120f, () =>
                {
                    _item = true;
                });
                
                response = "Item Giveado";
                return true;
            }
            else
            {
                response = "No puedes hacerlo todavia o No eres Tutorial";
                return false;
            }
        }
    }
    public class Invisible : ICommand
    {
        public string Command { get; } = "invis";
        public string[] Aliases { get; } = null;
        public string Description { get; } = "Vuelvete invisible";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ragnarok.donadores"))
            {
                response = "No tienes permisos";
                return false;
            }
            Player p = Player.Get((CommandSender)sender);

            if (p.Role.Type != RoleType.Tutorial)
            {
                response = "No eres tutorial para ejecutar el comando";
                return false;
            }
            if (p.Role.Type == RoleType.Tutorial)
            {
                p.IsInvisible = true;
                response = "Ahora eres invisible";
                return true;
            }

            // Fix al no poder quitarte el invis una vez activado
            if (p.IsInvisible = true)
            {
                p.IsInvisible = false;
                response = "Ya no eres invisible";
                return true;
            }

            response = "Stacktrace: Fallo en el comando, reporta esto con una captura al Dev del servidor";
            return false;
        }
    }

    public class NoClip : ICommand
    {
        public string Command => "nc";
        public string[] Aliases => null;
        public string Description => "Activate el poder volar como dios";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ragnarok.donadores"))
            {
                response = "No tienes permisos";
                return false;
            }
            Player _ = Player.Get((CommandSender)sender);

            if (_.Role.Type != RoleType.Tutorial)
            {
                response = "No eres tutorial";
                return false;
            }
            if (_.Role.Type == RoleType.Tutorial)
            {
                _.NoClipEnabled = true;
                response = "Hecho, usa ALT para empezar a volar";
                return true;
            }
            // Fix al no poder desactivarle el Noclip
            if (_.NoClipEnabled = true)
            {
                _.NoClipEnabled = false;
                response = "Ya no puedes volar";
                return true;
            }

            response = "Stacktrace: Fallo en el comando, reporta esto con una captura al Dev del servidor";
            return false;
        }
    }

    public class DonadorChat : ICommand
    {
        public string Command => "dchat";
        public string[] Aliases => null;
        public string Description => "Chat de donadores";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ragnarok.donadores"))
            {
                response = "No tienes permisos para ejecutar este comando";
                return false;
            }
            Player send = Player.Get((CommandSender)sender);
            IEnumerable<string> thing2 = arguments.Skip(0);
            string msg = "";
            foreach (string s in thing2)
                msg += $"{s} ";

            foreach (Player ply in Player.List)
            {
                if (ply.CheckPermission("ragnarok.donadores"))
                {
                    ply.ClearBroadcasts();
                    ply.Broadcast(10, $"<i><color=orange>{send.Nickname}:</color> {msg}</i>", Broadcast.BroadcastFlags.Normal, true);
                }
            }

            response = "Broadcast enviado a todos los staffs y donadores del servidor";
            return true;
        }
    }
}