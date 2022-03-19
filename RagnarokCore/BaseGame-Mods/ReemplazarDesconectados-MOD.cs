using GameCore;
using InventorySystem;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace RagnarokCore.BaseGame_Mods
{
	public class ReemplazarDesconectados_MOD : MonoBehaviour
    {
        private void OnDestroy()
        {
			ReferenceHub.Hubs.Remove(base.gameObject);
			RoleType curClass = base.gameObject.GetComponent<CharacterClassManager>().CurClass;
			if (curClass == RoleType.Spectator || curClass == RoleType.Tutorial || curClass == RoleType.None)
			{
				return;
			}
			if (!ConfigFile.ServerConfig.GetBool("ragnarok_reemplazar_desconectados", true))
			{
				return;
			}
			string @string = ConfigFile.ServerConfig.GetString("ragnarok_reemplazar_desconectados_mensaje", "You've replaced a player that disconnected.");
			Inventory component = base.GetComponent<Inventory>();
			Vector3 realModelPosition = base.GetComponent<PlayerMovementSync>().RealModelPosition;
			foreach (ReferenceHub referenceHub in ReferenceHub.Hubs.Values)
			{
				if (!(PlayerManager.localPlayer == referenceHub.gameObject) && referenceHub.characterClassManager.CurClass == RoleType.Spectator)
				{
					referenceHub.characterClassManager.SetPlayersClass(curClass, referenceHub.gameObject, CharacterClassManager.SpawnReason.Revived, false);
					if (curClass == RoleType.Scp079)
					{
						this.Copy079(referenceHub.scp079PlayerScript, base.GetComponent<Scp079PlayerScript>());
					}
					Timing.RunCoroutine(this.ReallyFunCoroutine(realModelPosition, referenceHub.gameObject), 1);
					PlayerManager.localPlayer.GetComponent<Broadcast>().TargetAddElement(referenceHub.characterClassManager.connectionToClient, @string, 7, Broadcast.BroadcastFlags.Normal);
					break;
				}
			}
		}
		private IEnumerator<float> ReallyFunCoroutine(Vector3 pos, GameObject go)
		{
			yield return Timing.WaitForSeconds(0.3f);
			go.GetComponent<PlayerMovementSync>().OverridePosition(pos, 0f, false);
			yield break;
		}
		private void Copy079(Scp079PlayerScript dst, Scp079PlayerScript src)
		{
			dst.currentCamera = src.currentCamera;
			dst.CurrentRoom = src.CurrentRoom;
			dst.lockedDoors = src.lockedDoors;
			dst.maxMana = src.maxMana;
			dst.nearbyInteractables = src.nearbyInteractables;
			dst.nearestCameras = src.nearestCameras;
			dst._curExp = src._curExp;
			dst._curLvl = src._curLvl;
			dst.Network_curMana = src.Network_curMana;
			dst.NetworkcurSpeaker = src.NetworkcurSpeaker;
			dst.NetworkmaxMana = src.NetworkmaxMana;
		}
	}
}
