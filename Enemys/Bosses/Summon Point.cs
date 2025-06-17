using UnityEngine;
using UnityEngine.Events;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class SummonPoint : StateController
	{
		private UnityAction _getTouch;
		[Header("Extern Interaction")]
		[SerializeField, Tooltip("If this point will trigger with other object.")] private bool _hasTarget;
		internal void GetTouch(UnityAction getTouch) => this._getTouch = getTouch;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._hasTarget)
			{
				if (GuwbaAstral<VisualGuwba>.EqualObject(other.gameObject))
					this._getTouch.Invoke();
				return;
			}
			if (other.TryGetComponent<SummonerBoss>(out _))
				this._getTouch.Invoke();
		}
	};
};