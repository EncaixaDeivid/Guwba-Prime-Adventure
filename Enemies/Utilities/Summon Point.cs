using UnityEngine;
using UnityEngine.Events;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class SummonPoint : StateController
	{
		private UnityAction _getTouch;
		[Header("Interactions")]
		[SerializeField, Tooltip("If this point will destroy itself after use.")] private bool _destroyAfter;
		[SerializeField, Tooltip("If this point will trigger with other object.")] private bool _hasTarget;
		internal void GetTouch(UnityAction getTouch) => _getTouch = getTouch;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_hasTarget)
			{
				if (GuwbaAstralMarker.EqualObject(other.gameObject))
					_getTouch.Invoke();
			}
			else if (other.TryGetComponent<SummonerEnemy>(out _))
				_getTouch.Invoke();
			if (_destroyAfter)
				Destroy(gameObject);
		}
	};
};
