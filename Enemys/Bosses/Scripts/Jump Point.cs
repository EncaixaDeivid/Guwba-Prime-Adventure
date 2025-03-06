using UnityEngine;
using UnityEngine.Events;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class JumpPoint : StateController
	{
		private UnityAction<ushort> _getTouch;
		private ushort _touchIndex;
		internal void GetTouch(ushort touchIndex, UnityAction<ushort> getTouch)
		{
			this._getTouch = getTouch;
			this._touchIndex = touchIndex;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<JumperBoss>(out _))
				this._getTouch(this._touchIndex);
		}
	};
};