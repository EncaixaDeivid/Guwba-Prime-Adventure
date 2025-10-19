using UnityEngine;
using UnityEngine.Events;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class JumpPoint : StateController
	{
		private UnityAction<ushort> _getTouch;
		private ushort _touchIndex;
		[Header("Interactions")]
		[SerializeField, Tooltip("If this point will destroy itself after use.")] private bool _destroyAfter;
		[SerializeField, Tooltip("If this point will trigger with other object.")] private bool _hasTarget;
		internal void GetTouch(ushort touchIndex, UnityAction<ushort> getTouch)
		{
			_getTouch = getTouch;
			_touchIndex = touchIndex;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_hasTarget)
			{
				if (GuwbaAstralMarker.EqualObject(other.gameObject))
					_getTouch.Invoke(_touchIndex);
			}
			else if (other.TryGetComponent<JumperEnemy>(out _))
				_getTouch.Invoke(_touchIndex);
			if (_destroyAfter)
				Destroy(gameObject);
		}
	};
};
