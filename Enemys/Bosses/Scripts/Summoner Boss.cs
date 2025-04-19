using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class SummonerBoss : BossController, IConnector
	{
		private float _gravityScale = 0f;
		private bool _stopSummon = false;
		[Header("Summoner Boss"), SerializeField] private SummonPlaces[] _summonPlaces;
		[SerializeField] private SummonObject[] _eventSummons;
		[SerializeField] private SummonObject[] _timedSummons;
		private void Summon(SummonObject summon)
		{
			Vector2 combinePoint = (Vector2)this.transform.position + summon.SummonPoints[0];
			if (summon.StopToSummon)
				this.StartCoroutine(StopToSummon());
			IEnumerator StopToSummon()
			{
				Sender.Create().SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action)
					.SetBossType(BossType.Runner | BossType.Jumper).SetToggle(false).Send();
				this._rigidybody.linearVelocityX = 0f;
				if (summon.ParalyzeToSummon)
					this._rigidybody.gravityScale = 0f;
				yield return new WaitTime(this, summon.TimeToStop);
				Sender.Create().SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action)
					.SetBossType(BossType.Runner | BossType.Jumper).SetToggle(true).Send();
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
			this._gravityScale = this._rigidybody.gravityScale;
			foreach (SummonPlaces summonPlaces in this._summonPlaces)
			{
				SummonPoint summonPoint = summonPlaces.SummonPointObject;
				Instantiate(summonPoint, summonPlaces.Point, Quaternion.identity)
					.GetTouch(() => this.Summon(this._eventSummons[summonPlaces.IndexValue]));
			}
			foreach (SummonObject summon in this._timedSummons)
			{
				this.StartCoroutine(TimedSummon(summon));
				IEnumerator TimedSummon(SummonObject summon)
				{
					yield return new WaitTime(this, summon.SummonTime);
					if (!summon.StopTimedSummon && !this._stopSummon)
					{
						this.Summon(summon);
						this.StartCoroutine(TimedSummon(summon));
					}
				}
			}
		}
		public new void Receive(DataConnection data)
		{
			base.Receive(data);
			if (!data.BossType.HasFlag(BossType.Summoner))
				return;
			bool has = data.IndexValue.HasValue && this._eventSummons.Length > 0f && this._hasIndex;
			if (data.ConnectionState == ConnectionState.Action && data.ToggleValue.HasValue && this._hasToggle)
				this._stopSummon = !data.ToggleValue.Value;
			else if (data.ConnectionState == ConnectionState.Action && has)
				this.Summon(this._eventSummons[data.IndexValue.Value]);
		}
		[System.Serializable]
		private struct SummonPlaces
		{
			[SerializeField] private SummonPoint _summonPointObject;
			[SerializeField] private Vector2 _point;
			[SerializeField] private ushort _indexValue;
			internal readonly SummonPoint SummonPointObject => this._summonPointObject;
			internal readonly Vector2 Point => this._point;
			internal readonly ushort IndexValue => this._indexValue;
		};
	};
};
