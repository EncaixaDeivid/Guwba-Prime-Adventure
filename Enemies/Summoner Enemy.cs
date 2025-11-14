using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class SummonerEnemy : EnemyProvider, IConnector
	{
		private SummonStats[] _summonStats;
		private bool _stopSummon = false;
		private ushort _randomSummonIndex = 0;
		private float _stopTime = 0f;
		private float _gravityScale = 0f;
		[Header("Summoner Enemy")]
		[SerializeField, Tooltip("The summoner statitics of this enemy.")] private SummonerStatistics _statistics;
		private void Summon(SummonObject summon)
		{
			if (summon.StopToSummon)
			{
				_sender.SetToggle(false);
				_sender.Send(PathConnection.Enemy);
				if (summon.ParalyzeToSummon)
					Rigidbody.gravityScale = 0f;
				_stopTime = summon.TimeToStop;
			}
			Vector2 position;
			Vector2Int summonIndex = new();
			InstantiateParameters instantiateParameters = new() { parent = summon.LocalPoints ? transform : null, worldSpace = !summon.LocalPoints };
			for (ushort i = 0; i < summon.QuantityToSummon; i++)
			{ 
				if (summon.Self)
					position = transform.position;
				else if (summon.Random)
					position = summon.SummonPoints[Random.Range(0, summon.SummonPoints.Length - 1)];
				else
					position = summon.SummonPoints[summonIndex.y];
				Instantiate(summon.Summons[summonIndex.x], position, summon.Summons[summonIndex.x].transform.rotation, instantiateParameters).transform.SetParent(null);
				summonIndex.x = (ushort)(summonIndex.x < summon.Summons.Length - 1f ? summonIndex.x + 1f : 0f);
				summonIndex.y = (ushort)(summonIndex.y < summon.SummonPoints.Length - 1f ? summonIndex.y + 1f : 0f);
			}
		}
		private new void Awake()
		{
			base.Awake();
			_sender.SetStateForm(StateForm.State);
			_summonStats = new SummonStats[_statistics.TimedSummons.Length];
			_gravityScale = Rigidbody.gravityScale;
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
			_randomSummonIndex = (ushort)Random.Range(0, _statistics.TimedSummons.Length - 1);
			for (ushort i = 0; i < _summonStats.Length; i++)
			{
				_summonStats[i].isSummonTime = true;
				_summonStats[i].summonTime = _statistics.TimedSummons[i].SummonTime;
			}
			foreach (SummonPointStructure summonStructure in _statistics.SummonPointStructures)
				Instantiate(summonStructure.SummonPointObject, summonStructure.Point, Quaternion.identity).GetTouch(() => Summon(summonStructure.Summon));
		}
		private void IndexedSummon(ushort summonIndex)
		{
			if (_summonStats[summonIndex].stopPermanently)
				return;
			if (_stopSummon)
			{
				if (_statistics.TimedSummons[summonIndex].StopPermanently && !_summonStats[summonIndex].stopPermanently)
					_summonStats[summonIndex].stopPermanently = true;
				return;
			}
			if (_summonStats[summonIndex].summonTime > 0f)
				if ((_summonStats[summonIndex].summonTime -= Time.deltaTime) <= 0f)
				{
					if (_summonStats[summonIndex].isSummonTime)
					{
						Summon(_statistics.TimedSummons[summonIndex]);
						_summonStats[summonIndex].summonTime = _statistics.TimedSummons[summonIndex].PostSummonTime;
					}
					else
						_summonStats[summonIndex].summonTime = _statistics.TimedSummons[summonIndex].SummonTime;
					_summonStats[summonIndex].isSummonTime = !_summonStats[summonIndex].isSummonTime;
					if (_statistics.RandomTimedSummons && _summonStats[summonIndex].isSummonTime)
						_randomSummonIndex = (ushort)Random.Range(0, _statistics.TimedSummons.Length - 1);
				}
		}
		private void Update()
		{
			if (IsStunned)
				return;
			if (_stopTime > 0f)
				if ((_stopTime -= Time.deltaTime) <= 0f)
				{
					_sender.SetToggle(true);
					_sender.Send(PathConnection.Enemy);
					Rigidbody.gravityScale = _gravityScale;
				}
			if (_statistics.RandomTimedSummons)
				IndexedSummon(_randomSummonIndex);
			else
				for (ushort i = 0; i < _statistics.TimedSummons.Length; i++)
					IndexedSummon(i);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if ((EnemyProvider[])additionalData != null)
				foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
					if (enemy != this)
						return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				_stopSummon = !data.ToggleValue.Value;
			else if (data.StateForm == StateForm.Action && _statistics.HasEventSummon && _statistics.EventSummons.Length > 0f)
				if (_statistics.RandomReactSummons)
					Summon(_statistics.EventSummons[Random.Range(0, _statistics.EventSummons.Length - 1)]);
				else if (data.NumberValue.HasValue && data.NumberValue.Value < _statistics.EventSummons.Length && data.NumberValue.Value >= 0)
					Summon(_statistics.EventSummons[data.NumberValue.Value]);
		}
		private struct SummonStats
		{
			internal bool isSummonTime;
			internal bool stopPermanently;
			internal float summonTime;
		}
	};
};
