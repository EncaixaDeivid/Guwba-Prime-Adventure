using UnityEngine;
using GwambaPrimeAdventure.Character;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class SummonPoint : StateController
	{
		private ISummoner _summoner;
		private ushort _summonIndex;
		[Header("Interactions")]
		[SerializeField, Tooltip("If this point will destroy itself after use.")] private bool _destroyAfter;
		[SerializeField, Tooltip("If this point will trigger with other object.")] private bool _hasTarget;
		internal void GetTouch(ISummoner summoner, ushort summonIndex)
		{
			_summoner = summoner;
			_summonIndex = summonIndex;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_hasTarget)
			{
				if (GwambaStateMarker.EqualObject(other.gameObject))
					_summoner.OnSummon(_summonIndex);
			}
			else if (other.TryGetComponent<SummonerEnemy>(out _))
				_summoner.OnSummon(_summonIndex);
			if (_destroyAfter)
				Destroy(gameObject);
		}
	};
	internal interface ISummoner
	{
		public void OnSummon(ushort summonIndex);
	};
};
