using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace RagnarokCore.Patches
{
    class ProcessStaminaOverride
    {
		private static readonly MethodInfo _processStamina = typeof(Stamina).GetMethod("ProcessStamina",
																	   BindingFlags.Public |
																	   BindingFlags.NonPublic |
																	   BindingFlags.Instance |
																	   BindingFlags.IgnoreCase);
		private static readonly MethodInfo _customStamina = typeof(ProcessStaminaOverride).GetMethod("CustomProcessStamina",
																					BindingFlags.Public |
																					BindingFlags.NonPublic |
																					BindingFlags.Static |
																					BindingFlags.IgnoreCase);
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
			var found = false;
			if (_processStamina == null)
			{
				Log.Warn("Can't find ProcessStamina in the Stamina class");
				foreach (var instruction in instructions) yield return instruction;
				yield break;
			}
			Log.Debug("Checking if any function calls " + _processStamina);
			foreach (var instruction in instructions)
			{
				if (instruction.Calls(_processStamina))
				{
					// If a call to ProcessStamina is found,
					// add a 'CustomProcessStamina(this);' line.

					// yield return new CodeInstruction(OpCodes.Ldarg_0); - ldarg.0 is already called, just replace the "call"
					yield return new CodeInstruction(OpCodes.Call, _customStamina);
					found = true;
				}
				else
				{
					yield return instruction;
				}
			}
			if (!found) Log.Error($"Can't find instruction ProcessStamina in method ServersideUpdate");
			else Log.Debug("Method succesfully patched");
		}
		public static void CustomProcessStamina(Stamina _this)
        {
			if (_this._hub.characterClassManager.CurRole.team == global::Team.SCP
			|| _this._hub.characterClassManager.AliveTime < _this.StaminaImmunityUponRespawn)
			{
				// *************************** //
				// We patched this, so reset it here (they'll probably respawn without
				// stamina otherwise
				_this._prevPosition = _this._hub.playerMovementSync.RealModelPosition;
				// *************************** //
				return;
			}
			if (_this._asphyxiated != null && _this._asphyxiated.enabled)
			{
				_this._regenerationTimer = 0f;
			}
			if (_this._isSprinting)
			{
				float num = _this.StaminaUse;
				if (_this._exhausted.enabled)
				{
					num *= _this._exhausted.maxStaminaMultiplier;
				}
				_this._regenerationTimer = 0f;
				if (!_this._invigorated.enabled)
				{
					// *************************** //
					// Everything above is code from Northwood.
					// Calculate the movement percentage based on his maxSpeed
					_this._hub.animationController.fpc.GetSpeed(out float maxSpeed, true);
					float distance = Vector3.Distance(_this._prevPosition, _this._hub.playerMovementSync.RealModelPosition);
					float distPercent = distance / (maxSpeed * _this._regenerationTimer * Time.deltaTime);
					num *= distPercent;
					// Everything below is code from Northwood.
					// *************************** //
					_this.RemainingStamina -= Time.deltaTime * num;
				}
			}
			else if (_this.RemainingStamina < 1f)
			{
				_this.RemainingStamina += Time.deltaTime * _this.RegenerationOverTime.Evaluate(_this._regenerationTimer) * (_this._exhausted.enabled ? 0.5f : 1f);
				if (_this._regenerationTimer < _this.RegenerationOverTime.keys[_this.RegenerationOverTime.keys.Length - 1].time)
				{
					_this._regenerationTimer += Time.deltaTime;
				}
			}
			if (_this._exhausted.enabled)
			{
				_this.RemainingStamina = Mathf.Min(_this.RemainingStamina, 1f * _this._exhausted.maxStaminaMultiplier);
			}
			_this.RemainingStamina = Mathf.Clamp01(_this.RemainingStamina);
			// *************************** //
			// We patched this, so reset it here
			_this._prevPosition = _this._hub.playerMovementSync.RealModelPosition;
			// *************************** //
		}
	}
}
