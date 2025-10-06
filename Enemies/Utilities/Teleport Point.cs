using UnityEngine;
using UnityEngine.Events;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class TeleportPoint : MonoBehaviour
	{
		private UnityAction _getTouch;
		[Header("Interactions")]
		[SerializeField, Tooltip("If this point will destroy itself after use.")] private bool _destroyAfter;
		[SerializeField, Tooltip("If this point will trigger with other object.")] private bool _hasTarget;
		internal void GetTouch(UnityAction getTouch) => this._getTouch = getTouch;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._hasTarget)
			{
				if (GuwbaCentralizer.EqualObject(other.gameObject))
					this._getTouch.Invoke();
				return;
			}
			if (other.TryGetComponent<TeleporterEnemy>(out _))
				this._getTouch.Invoke();
			if (this._destroyAfter)
				Destroy(this.gameObject);
		}
	};
};
