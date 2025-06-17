using UnityEngine;
using UnityEngine.Events;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class JumpPoint : StateController
	{
		private UnityAction<ushort> _getTouch;
		private ushort _touchIndex;
		[Header("Extern Interaction")]
		[SerializeField, Tooltip("If this point will trigger with other object.")] private bool _hasTarget;
		internal void GetTouch(ushort touchIndex, UnityAction<ushort> getTouch)
		{
			this._getTouch = getTouch;
			this._touchIndex = touchIndex;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._hasTarget)
			{
				if (GuwbaAstral<VisualGuwba>.EqualObject(other.gameObject))
					this._getTouch.Invoke(this._touchIndex);
				return;
			}
			if (other.TryGetComponent<JumperBoss>(out _))
				this._getTouch.Invoke(this._touchIndex);
		}
	};
};