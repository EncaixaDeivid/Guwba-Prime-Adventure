using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class SummonerBoss : BossController, IConnector
	{
		private readonly Sender _sender = Sender.Create();
		private float _gravityScale = 0f;
		private bool _stopSummon = false;
		[Header("Summoner Boss")]
		[SerializeField, Tooltip("The collection of the summon places.")] private SummonPlaces[] _summonPlaces;
		[SerializeField, Tooltip("The summons that will be activate on an event.")] private SummonObject[] _eventSummons;
		[SerializeField, Tooltip("The summons that will be activate with time.")] private SummonObject[] _timedSummons;
		[SerializeField, Tooltip("If this enemy will summon randomized in the react.")] private bool _randomReactSummons;
		[SerializeField, Tooltip("If this enemy will summon randomized timed.")] private bool _randomTimedSummons;
		[SerializeField, Tooltip("The time the timed randomized summons will be executed.")] private float _randomSummonsTime;
		private void Summon(SummonObject summon)
		{
			Vector2 combinePoint = (Vector2)this.transform.position + summon.SummonPoints[0];
			if (summon.StopToSummon)
				this.StartCoroutine(StopToSummon());
			IEnumerator StopToSummon()
			{
				this._sender.SetToggle(false).Send();
				this._rigidybody.linearVelocityX = 0f;
				if (summon.ParalyzeToSummon)
					this._rigidybody.gravityScale = 0f;
				yield return new WaitTime(this, summon.TimeToStop);
				this._sender.SetToggle(true).Send();
				this._rigidybody.gravityScale = this._gravityScale;
			}
			for (ushort i = 0; i < summon.QuantityToSummon; i++)
				if (summon.Self)
					Instantiate(summon.Summon, this.transform.position, summon.Summon.transform.rotation, this.transform);
				else if (summon.Combine && summon.Sequential)
				{
					Vector2 combineSequentialPoint = (Vector2)this.transform.position + summon.SummonPoints[i];
					Instantiate(summon.Summon, combineSequentialPoint, summon.Summon.transform.rotation, this.transform);
				}
				else if (summon.Combine)
					Instantiate(summon.Summon, combinePoint, summon.Summon.transform.rotation, this.transform);
				else if (summon.Sequential)
					Instantiate(summon.Summon, summon.SummonPoints[i], summon.Summon.transform.rotation, this.transform);
				else if (summon.Random)
				{
					ushort pointIndex = (ushort)Random.Range(0f, summon.SummonPoints.Length - 1f);
					Instantiate(summon.Summon, summon.SummonPoints[pointIndex], summon.Summon.transform.rotation, this.transform);
				}
				else
					Instantiate(summon.Summon, summon.SummonPoints[0], summon.Summon.transform.rotation, this.transform);
		}
		private new void Awake()
		{
			base.Awake();
			this._sender.SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action);
			this._sender.SetAdditionalData(BossType.Runner | BossType.Jumper);
			this._gravityScale = this._rigidybody.gravityScale;
			foreach (SummonPlaces summonPlaces in this._summonPlaces)
			{
				SummonPoint summonPoint = summonPlaces.SummonPointObject;
				summonPoint = Instantiate(summonPoint, summonPlaces.Point, Quaternion.identity);
				summonPoint.GetTouch(() => this.Summon(this._eventSummons[summonPlaces.IndexValue]));
			}
			if (this._randomTimedSummons)
			{
				this.StartCoroutine(RandomTimedSummon());
				IEnumerator RandomTimedSummon()
				{
					ushort randomIndex = (ushort)Random.Range(0f, this._timedSummons.Length - 1f);
					this.StartCoroutine(TimedSummon(this._timedSummons[randomIndex]));
					yield return new WaitTime(this, this._randomSummonsTime);
					this.StartCoroutine(RandomTimedSummon());
				}
			}
			else
				foreach (SummonObject summon in this._timedSummons)
					this.StartCoroutine(TimedSummon(summon));
			IEnumerator TimedSummon(SummonObject summon)
			{
				yield return new WaitTime(this, summon.SummonTime);
				yield return new WaitUntil(() => !summon.StopTimedSummon && !this._stopSummon);
				if (!summon.StopTimedSummon && !summon.StopPermanently && !this._stopSummon)
				{
					this.Summon(summon);
					if (!this._randomTimedSummons)
						this.StartCoroutine(TimedSummon(summon));
				}
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			BossType bossType = (BossType)additionalData;
			if (bossType.HasFlag(BossType.Summoner) || bossType.HasFlag(BossType.All))
			{
				bool has = data.IndexValue.HasValue && this._hasIndex;
				if (data.ConnectionState == ConnectionState.Action && data.ToggleValue.HasValue && this._hasToggle)
					this._stopSummon = !data.ToggleValue.Value;
				else if (data.ConnectionState == ConnectionState.Action && this._reactToDamage && this._eventSummons.Length > 0f)
					if (this._randomReactSummons)
					{
						ushort randomIndex = (ushort)Random.Range(0f, this._eventSummons.Length - 1f);
						this.Summon(this._eventSummons[randomIndex]);
					}
					else if (has && data.IndexValue.Value < this._eventSummons.Length && data.IndexValue.Value >= 0)
						this.Summon(this._eventSummons[data.IndexValue.Value]);
			}
		}
		[System.Serializable]
		private struct SummonPlaces
		{
			[SerializeField, Tooltip("The object to activate the summon.")] private SummonPoint _summonPointObject;
			[SerializeField, Tooltip("The point where the summon point will be.")] private Vector2 _point;
			[SerializeField, Tooltip("Which summon event the summon point will activate.")] private ushort _indexValue;
			internal readonly SummonPoint SummonPointObject => this._summonPointObject;
			internal readonly Vector2 Point => this._point;
			internal readonly ushort IndexValue => this._indexValue;
		};
	};
};
