using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class SummonerEnemy : EnemyProvider, ISummoner, IConnector
	{
		private IEnumerator _summonEvent;
		private bool[] _isSummonTime;
		private bool[] _stopPermanently;
		private bool _stopSummon = false;
		private bool _waitStop = false;
		private ushort _randomSummonIndex = 0;
		private float[] _summonTime;
		private float[] _structureTime;
		private float _fullStopTime = 0F;
		private float _stopTime = 0F;
		private float _gravityScale = 0F;
		[Header("Summoner Enemy")]
		[SerializeField, Tooltip("The summoner statitics of this enemy.")] private SummonerStatistics _statistics;
		private new void Awake()
		{
			base.Awake();
			_sender.SetFormat(MessageFormat.State);
			_isSummonTime = new bool[_statistics.TimedSummons.Length];
			_stopPermanently = new bool[_statistics.TimedSummons.Length];
			_summonTime = new float[_statistics.TimedSummons.Length];
			_structureTime = new float[_statistics.SummonPointStructures.Length];
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
			_randomSummonIndex = (ushort)Random.Range(0, _statistics.TimedSummons.Length + 1);
			for (ushort i = 0; i < _statistics.TimedSummons.Length; i++)
				_isSummonTime[i] = true;
			for (ushort i = 0; i < _statistics.TimedSummons.Length; i++)
				_summonTime[i] = _statistics.TimedSummons[i].SummonTime;
			for (ushort i = 0; i < _statistics.SummonPointStructures.Length; i++)
				Instantiate(_statistics.SummonPointStructures[i].SummonPointObject, _statistics.SummonPointStructures[i].Point, Quaternion.identity).GetTouch(this, i);
		}
		private async void Summon(SummonObject summon)
		{
			while (_summonEvent is not null)
				await Task.Yield();
			_summonEvent = StopToSummon();
			_summonEvent.MoveNext();
			if (summon.InstantlySummon)
				_summonEvent.MoveNext();
			IEnumerator StopToSummon()
			{
				if (summon.StopToSummon)
				{
					_sender.SetToggle(false);
					_sender.Send(MessagePath.Enemy);
					if (summon.ParalyzeToSummon)
						Rigidbody.gravityScale = 0F;
					_waitStop = summon.WaitStop;
					_fullStopTime = _stopTime = summon.TimeToStop;
					yield return null;
				}
				Vector2 position;
				Vector2Int summonIndex = new();
				InstantiateParameters instantiateParameters = new() { parent = summon.LocalPoints ? transform : null, worldSpace = !summon.LocalPoints };
				for (ushort i = 0; i < summon.QuantityToSummon; i++)
				{
					if (summon.Self)
						position = transform.position;
					else if (summon.Random)
						position = summon.SummonPoints[Random.Range(0, summon.SummonPoints.Length + 1)];
					else
						position = summon.SummonPoints[summonIndex.y];
					Instantiate(summon.Summons[summonIndex.x], position, summon.Summons[summonIndex.x].transform.rotation, instantiateParameters).transform.SetParent(null);
					summonIndex.x = (ushort)(summonIndex.x < summon.Summons.Length - 1 ? summonIndex.x + 1 : 0);
					summonIndex.y = (ushort)(summonIndex.y < summon.SummonPoints.Length - 1 ? summonIndex.y + 1 : 0);
				}
				_summonEvent = null;
			}
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
			if (_summonTime[summonIndex] > 0F)
				if ((_summonTime[summonIndex] -= Time.deltaTime) <= 0F)
				{
					if (_isSummonTime[summonIndex])
					{
						Summon(_statistics.TimedSummons[summonIndex]);
						_summonTime[summonIndex] = _statistics.TimedSummons[summonIndex].PostSummonTime;
					}
					else
						_summonTime[summonIndex] = _statistics.TimedSummons[summonIndex].SummonTime;
					_isSummonTime[summonIndex] = !_isSummonTime[summonIndex];
					if (_statistics.RandomTimedSummons && _isSummonTime[summonIndex])
						_randomSummonIndex = (ushort)Random.Range(0, _statistics.TimedSummons.Length);
				}
		}
		private void Update()
		{
			if (IsStunned)
				return;
			for (ushort i = 0; i < _structureTime.Length; i++)
				if (_structureTime[i] > 0F)
					if ((_structureTime[i] -= Time.deltaTime) < 0F)
					{

					}
			if (_stopTime > 0F)
			{
				if ((_stopTime -= Time.deltaTime) <= _fullStopTime / 2F && !_waitStop && _summonEvent is not null)
					_summonEvent.MoveNext();
				if (_stopTime <= 0F)
				{
					_sender.SetToggle(true);
					_sender.Send(MessagePath.Enemy);
					Rigidbody.gravityScale = _gravityScale;
					if (_waitStop)
						_summonEvent?.MoveNext();
				}
			}
			if (_statistics.RandomTimedSummons)
				IndexedSummon(_randomSummonIndex);
			else
				for (ushort i = 0; i < _statistics.TimedSummons.Length; i++)
					IndexedSummon(i);
		}
		public void OnSummon(ushort summonIndex)
		{
			_structureTime[summonIndex] = _statistics.SummonPointStructures[summonIndex].TimeToUse;
			Summon(_statistics.SummonPointStructures[summonIndex].Summon);
		}
		public void Receive(MessageData message)
		{
			if (message.AdditionalData != null && message.AdditionalData is EnemyProvider[] && (message.AdditionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in message.AdditionalData as EnemyProvider[])
					if (enemy && enemy == this)
					{
						if (message.Format == MessageFormat.State && message.ToggleValue.HasValue)
							_stopSummon = !message.ToggleValue.Value;
						else if (message.Format == MessageFormat.Event && _statistics.HasEventSummon && _statistics.EventSummons.Length > 0)
							if (_statistics.RandomReactSummons)
								Summon(_statistics.EventSummons[Random.Range(0, _statistics.EventSummons.Length + 1)]);
							else if (message.NumberValue.HasValue && message.NumberValue.Value < _statistics.EventSummons.Length && message.NumberValue.Value >= 0)
								Summon(_statistics.EventSummons[message.NumberValue.Value]);
						return;
					}
		}
	};
};
