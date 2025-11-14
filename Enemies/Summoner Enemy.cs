using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class SummonerEnemy : EnemyProvider, IConnector
	{
		private bool[] _onSummonTime;
		private bool[] _stopPermanently;
		private bool _stopSummon = false;
		private ushort _randomSummonIndex = 0;
		private float[] _summonTime;
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
			_onSummonTime = new bool[_statistics.TimedSummons.Length];
			_stopPermanently = new bool[_statistics.TimedSummons.Length];
			_summonTime = new float[_statistics.TimedSummons.Length];
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
			for (ushort i = 0; i < _onSummonTime.Length; i++)
				_onSummonTime[i] = true;
			for (ushort i = 0; i < _summonTime.Length; i++)
				_summonTime[i] = _statistics.TimedSummons[i].SummonTime;
			foreach (SummonPointStructure summonStructure in _statistics.SummonPointStructures)
				Instantiate(summonStructure.SummonPointObject, summonStructure.Point, Quaternion.identity).GetTouch(() => Summon(summonStructure.Summon));
		}
		private void IndexedSummon(ushort summonIndex)
		{
			if (_stopPermanently[summonIndex])
				return;
			if (_stopSummon)
			{
				if (_statistics.TimedSummons[summonIndex].StopPermanently && !_stopPermanently[summonIndex])
					_stopPermanently[summonIndex] = true;
				return;
			}
			if (_summonTime[summonIndex] > 0f)
				if ((_summonTime[summonIndex] -= Time.deltaTime) <= 0f)
				{
					if (_onSummonTime[summonIndex])
					{
						Summon(_statistics.TimedSummons[summonIndex]);
						_summonTime[summonIndex] = _statistics.TimedSummons[summonIndex].PostSummonTime;
					}
					else
						_summonTime[summonIndex] = _statistics.TimedSummons[summonIndex].SummonTime;
					_onSummonTime[summonIndex] = !_onSummonTime[summonIndex];
					if (_statistics.RandomTimedSummons && _onSummonTime[summonIndex])
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
	};
};
