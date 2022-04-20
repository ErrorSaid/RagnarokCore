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
            if (!Round.IsStarted)
            {
                response = "No puedes usar comandos si la ronda no ha empezado";
                return false;
            }
            Player send = Player.Get((CommandSender) sender);
            //Si el sender no tiene permisos
            if (!sender.CheckPermission("ragnarok.donadores"))
            {
                response = "No tienes permisos para ejecutar este comando";
                return false;
            }

            if (send.Role.Type == RoleType.Tutorial)
            {
                send.Role.Type = RoleType.Spectator;
                response = "Ahora eres espectador";
                return true; 
            }

            if (send.Role.Type == RoleType.Spectator)
            {
                send.Role.Type = RoleType.Tutorial;
                response = "Ahora eres tutorial";
                return true;
            }

            response = "Tienes que ser espectador o tutorial para usar este comando";
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
            if (!Round.IsStarted)
            {
                response = "No puedes usar comandos si la ronda no ha empezado";
                return false;
            }
            
            Player send = Player.Get((CommandSender) sender);
            //Si el sender no tiene permisos
            if (!sender.CheckPermission("ragnarok.donadores"))
            {
                response = "No tienes permisos para ejecutar este comando";
                return false;
            }
            //Si el sender no es tutorial
            if (send.Role != RoleType.Tutorial)
            {
                response = "Tienes que estar en tutorial para poder usar este comando";
                return false;
            }

            if (send.IsInvisible)
            {
                send.IsInvisible = false;
                response = "Ya no eres invisible";
                return true;
            }
            
           
            send.IsInvisible = true;
            response = "Ahora eres invisible";
            return true;
        }
    }

    public class NoClip : ICommand
    {
        public string Command => "nc";
        public string[] Aliases => null;
        public string Description => "Activate el poder volar como dios";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Round.IsStarted)
            {
                response = "No puedes usar comandos si la ronda no ha empezado";
                return false;
            }
            
            Player send = Player.Get((CommandSender) sender);
            //Si el sender no tiene permisos
            if (!sender.CheckPermission("ragnarok.donadores"))
            {
                response = "No tienes permisos para ejecutar este comando";
                return false;
            }
            //Si el sender no es tutorial
            if (send.Role != RoleType.Tutorial)
            {
                response = "Tienes que estar en tutorial para poder usar este comando";
                return false;
            }
            //Si ya tiene el noclip activado, desactivarlo
            if (send.NoClipEnabled)
            {
                send.NoClipEnabled = false;
                response = "Noclip desactivado";
                return true;
            }
            //Activar el noclip si no lo tenia activado
            send.NoClipEnabled = true;
            response = "Noclip Activado";
            return true;
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