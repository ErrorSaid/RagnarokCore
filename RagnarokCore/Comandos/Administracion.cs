using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Mirror;
using NorthwoodLib.Pools;
using UnityEngine;

namespace RagnarokCore.Comandos
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Administracion : ParentCommand
    {
        public Administracion() => LoadGeneratedCommands();

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new TrollCommand());
            RegisterCommand(new ClientDestroyerCommand());
        }
        public override string Command { get; } = "admin";
        public override string[] Aliases { get; } = new string[] { "a" };
        public override string Description { get; } = "[RagnarokCore] Comandos de Admin para el staff";

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ragnarok.admin"))
            {
                response = "No tienes permisos";
                return false;
            }
            response = "SubComandos disponibles: trollcommand, clientcommand";
            return true;
        }
    }
    public class TrollCommand : ICommand
    {
        public string Command { get; } = "trollcommand";
        public string[] Aliases { get; } = null;
        public string Description { get; } = "[RagnarokCore] Comando troll, descubre los efectos por ti mismo";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ragnarok.admin"))
            {
                response = "No tienes permisos";
                return false;
            }
            var builder = StringBuilderPool.Shared.Rent();
            foreach (var identity in UnityEngine.Object.FindObjectsOfType<NetworkIdentity>())
            {
                builder.AppendLine($"{identity.transform.name} (layer{identity.transform.gameObject.layer})");
                builder.AppendLine($"  Components:");
                foreach (var i in identity.transform.gameObject.GetComponents<Component>())
                    builder.AppendLine($"    {i?.name}:{i?.GetType()}");
                builder.AppendLine($"  ComponentsInChildren:");
                foreach (var j in identity.transform.gameObject.GetComponentsInChildren<Component>())
                    builder.AppendLine($"    {j?.name}:{j?.GetType()}");
                builder.AppendLine($"  ComponentsInParent:");
                foreach (var k in identity.transform.gameObject.GetComponentsInParent<Component>())
                    builder.AppendLine($"    {k?.name}:{k?.GetType()}");
            }
            builder.AppendLine("---------END OF LIST----------");

            response = builder.ToString();
            StringBuilderPool.Shared.Return(builder);
            return true;
        }
    }

    public class ClientDestroyerCommand : ICommand
    {
        public string Command { get; } = "clientcommand";
        public string[] Aliases { get; } = null;
        public string Description { get; } = "[RagnarokCore] Comando troll, descubre los efectos por ti mismo";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ragnarok.admin"))
            {
                response = "No tienes permisos";
                return false;
            }
            Application.Quit();
            response = "A Chuparla";
            return true;
        }
    }
}
