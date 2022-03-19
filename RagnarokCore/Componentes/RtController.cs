using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace RagnarokCore.Componentes
{
    public class RtController : MonoBehaviour
    {
		public string[] Colors
		{
			get
			{
				return this._colors ?? Array.Empty<string>();
			}
			set
			{
				this._colors = (value ?? Array.Empty<string>());
				this._position = 0;
			}
		}
		public float Interval
		{
			get
			{
				return this._interval;
			}
			set
			{
				this._interval = value;
				this._intervalInFrames = Mathf.CeilToInt(value) * 50;
			}
		}
		private void Awake()
		{
			this._player = Player.Get(base.gameObject);
		}
		private void Start()
		{
			this._coroutineHandle = Timing.RunCoroutine(MECExtensionMethods2.CancelWith<RtController>(MECExtensionMethods2.CancelWith(this.UpdateColor(), this._player.GameObject), this));
		}
		private void OnDestroy()
		{
			Timing.KillCoroutines(new CoroutineHandle[]
			{
				this._coroutineHandle
			});
		}
		private string RollNext()
		{
			int num = this._position + 1;
			this._position = num;
			if (num >= this._colors.Length)
			{
				this._position = 0;
			}
			if (this._colors.Length == 0)
			{
				return string.Empty;
			}
			return this._colors[this._position];
		}
		private IEnumerator<float> UpdateColor()
		{
			for (; ; )
			{
				int num;
				for (int z = 0; z < this._intervalInFrames; z = num + 1)
				{
					yield return 0f;
					num = z;
				}
				string text = this.RollNext();
				if (string.IsNullOrEmpty(text))
				{
					break;
				}
				this._player.RankColor = text;
			}
			UnityEngine.Object.Destroy(this);
			yield break;
		}
		private Player _player;
		private int _position;
		private float _interval;
		private int _intervalInFrames;
		private string[] _colors;
		private CoroutineHandle _coroutineHandle;
	}
}
