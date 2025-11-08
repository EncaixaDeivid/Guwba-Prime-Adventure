using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class SummonerEnemy : EnemyProvider, IConnector
	{
		private float _gravityScale = 0f;
		private bool _stopSummon = false;
		[Header("Summoner Enemy")]
		[SerializeField, Tooltip("The summoner statitics of this enemy.")] private SummonerStatistics _statistics;
		private void Summon(SummonObject summon)
		{
			if (summon.StopToSummon)
				StartCoroutine(StopToSummon());
			IEnumerator StopToSummon()
			{
				_sender.SetToggle(false);
				_sender.Send(PathConnection.Enemy);
				if (summon.ParalyzeToSummon)
					_rigidybody.gravityScale = 0f;
				yield return new WaitTime(this, summon.TimeToStop);
				_sender.SetToggle(true);
				_sender.Send(PathConnection.Enemy);
				_rigidybody.gravityScale = _gravityScale;
			}
			Vector2 position;
			ushort summonIndex = 0;
			for (ushort i = 0; i < summon.QuantityToSummon; i++)
			{ 
				if (summon.Self)
					position = transform.position;
				else if (summon.Random)
					position = summon.SummonPoints[Random.Range(0, summon.SummonPoints.Length - 1)];
				else
					position = summon.SummonPoints[summonIndex];
				Instantiate(summon.Summons[summonIndex], position, summon.Summons[summonIndex].transform.rotation);
				summonIndex = (ushort)(summonIndex >= summon.Summons.Length - 1f ? 0f : summonIndex + 1f);
			}
		}
		private new void Awake()
		{
			base.Awake();
			_sender.SetStateForm(StateForm.State);
			_gravityScale = _rigidybody.gravityScale;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			foreach (SummonPointStructure summonStructure in _statistics.SummonPointStructures)
				Instantiate(summonStructure.SummonPointObject, summonStructure.Point, Quaternion.identity).GetTouch(() => Summon(summonStructure.Summon));
			if (_statistics.RandomTimedSummons)
			{
				ushort randomIndex;
				StartCoroutine(RandomTimedSummon());
				IEnumerator RandomTimedSummon()
				{
					randomIndex = (ushort)Random.Range(0, _statistics.TimedSummons.Length - 1);
					yield return TimedSummon(_statistics.TimedSummons[randomIndex]);
					yield return new WaitTime(this, _statistics.TimedSummons[randomIndex].PostSummonTime);
					StartCoroutine(RandomTimedSummon());
				}
			}
			else
				foreach (SummonObject summon in _statistics.TimedSummons)
					StartCoroutine(TimedSummon(summon));
			IEnumerator TimedSummon(SummonObject summon)
			{
				yield return new WaitTime(this, summon.SummonTime);
				yield return new WaitUntil(() => !summon.StopTimedSummon && !_stopSummon && isActiveAndEnabled && !IsStunned);
				if (!summon.StopTimedSummon && !summon.StopPermanently && !_stopSummon)
				{
					Summon(summon);
					yield return new WaitTime(this, summon.PostSummonTime);
					if (!_statistics.RandomTimedSummons)
						StartCoroutine(TimedSummon(summon));
				}
			}
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if ((EnemyProvider[])additionalData != null)
				foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
					if (enemy != this)
						return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				_stopSummon = !data.ToggleValue.Value;
			else if (data.StateForm == StateForm.Action && _statistics.ReactToDamage && _statistics.EventSummons.Length > 0f)
				if (_statistics.RandomReactSummons)
					Summon(_statistics.EventSummons[Random.Range(0, _statistics.EventSummons.Length - 1)]);
				else if (data.NumberValue.HasValue && data.NumberValue.Value < _statistics.EventSummons.Length && data.NumberValue.Value >= 0)
					Summon(_statistics.EventSummons[data.NumberValue.Value]);
		}
	};
};
