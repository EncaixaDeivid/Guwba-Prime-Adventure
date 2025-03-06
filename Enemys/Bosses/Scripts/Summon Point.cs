using UnityEngine;
using UnityEngine.Events;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class SummonPoint : StateController
	{
		private UnityAction _getTouch;
		internal void GetTouch(UnityAction getTouch) => this._getTouch = getTouch;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<SummonerBoss>(out _))
				this._getTouch();
		}
	};
};