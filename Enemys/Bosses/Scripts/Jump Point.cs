using UnityEngine;
using UnityEngine.Events;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class JumpPoint : StateController, IConnector
	{
		private UnityAction<ushort> _getTouch;
		private ushort _touchIndex;
		private bool _stopJump = false;
		[Header("Extern Interaction")]
		[SerializeField, Tooltip("If this point will trigger with other object.")] private bool _hasTarget;
		[SerializeField, Tooltip("If this boss has a toggle atribute to switch.")] private bool _hasToggle;
		public PathConnection PathConnection => PathConnection.Boss;
		private new void Awake()
		{
			base.Awake();
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		internal void GetTouch(ushort touchIndex, UnityAction<ushort> getTouch)
		{
			this._getTouch = getTouch;
			this._touchIndex = touchIndex;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._stopJump)
				return;
			if (this._hasTarget)
			{
				if (GuwbaAstral<VisualGuwba>.EqualObject(other.gameObject))
					this._getTouch.Invoke(this._touchIndex);
				return;
			}
			if (other.TryGetComponent<JumperBoss>(out _))
				this._getTouch.Invoke(this._touchIndex);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			BossType bossType = (BossType)additionalData;
			if (bossType.HasFlag(BossType.Jumper) || bossType.HasFlag(BossType.All))
				if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue && this._hasToggle)
					this._stopJump = !data.ToggleValue.Value;
		}
	};
};
