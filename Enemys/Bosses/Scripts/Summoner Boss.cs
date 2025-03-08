using UnityEngine;
using System.Collections;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class SummonerBoss : BossController
	{
		private float _gravityScale = 0f;
		private bool _stopSummon = false;
		[Header("Summoner Boss"), SerializeField] private SummonPlaces[] _summonPlaces;
		[SerializeField] private SummonObject[] _eventSummons, _timedSummons;
		private new void Awake() // Set Summon Points
		{
			base.Awake();
			this._gravityScale = this._rigidybody.gravityScale;
			this._toggleEvent = (bool toggleValue) => this._stopSummon = !toggleValue;
			this._indexEvent = (ushort indexValue) =>
			{
				if (this._eventSummons.Length > 0f)
					Summon(this._eventSummons[indexValue]);
			};
			foreach (SummonPlaces summonPlaces in this._summonPlaces)
			{
				SummonPoint summonPoint = summonPlaces.SummonPointObject;
				SummonPoint summonPointInstance = Instantiate(summonPoint, summonPlaces.Point, Quaternion.identity);
				summonPointInstance.GetTouch(() => this.Index(summonPlaces.IndexValue));
			}
			foreach (SummonObject summon in this._timedSummons)
			{
				this.StartCoroutine(TimedSummon(summon));
				IEnumerator TimedSummon(SummonObject summon)
				{
					yield return new WaitTime(this, summon.SummonTime);
					if (!summon.StopTimedSummon && !this._stopSummon)
					{
						Summon(summon);
						this.StartCoroutine(TimedSummon(summon));
					}
				}
			}
			void Summon(SummonObject summon)
			{
				Vector2 combinePoint = (Vector2)this.transform.position + summon.SummonPoints[0];
				if (summon.StopToSummon)
					this.StartCoroutine(StopToSummon());
				IEnumerator StopToSummon()
				{
					this.Toggle<RunnerBoss>(false);
					this.Toggle<JumperBoss>(false);
					this._rigidybody.linearVelocityX = 0f;
					if (summon.ParalyzeToSummon)
						this._rigidybody.gravityScale = 0f;
					yield return new WaitTime(this, summon.TimeToStop);
					this.Toggle<RunnerBoss>(true);
					this.Toggle<JumperBoss>(true);
					this._rigidybody.gravityScale = this._gravityScale;
				}
				for (ushort i = 0; i < summon.QuantityToSummon; i++)
					if (summon.Self)
						Instantiate(summon.Summon, this.transform.position, summon.Summon.transform.rotation, this.transform);
					else if (summon.Combine && summon.Sequential)
					{
						Vector2 combineSequentialPoint = (Vector2)this.transform.position + summon.SummonPoints[i];
						Instantiate(summon.Summon, combinePoint, summon.Summon.transform.rotation, this.transform);
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
