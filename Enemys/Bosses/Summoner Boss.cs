using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
	internal sealed class SummonerBoss : BossController, IConnector
	{
		private float _gravityScale = 0f;
		private bool _stopSummon = false;
		[Header("Summoner Boss")]
		[SerializeField, Tooltip("The collection of the summon places.")] private SummonPlaces[] _summonPlaces;
		[SerializeField, Tooltip("The summons that will be activate on an event.")] private SummonObject[] _eventSummons;
		[SerializeField, Tooltip("The summons that will be activate with time.")] private SummonObject[] _timedSummons;
		[SerializeField, Tooltip("If this enemy will summon randomized in the react.")] private bool _randomReactSummons;
		[SerializeField, Tooltip("If this enemy will summon randomized timed.")] private bool _randomTimedSummons;
		private void Summon(SummonObject summon)
		{
			if (summon.StopToSummon)
				this.StartCoroutine(StopToSummon());
			IEnumerator StopToSummon()
			{
				this._sender.SetToggle(false);
				this._sender.Send();
				if (summon.ParalyzeToSummon)
					this._rigidybody.gravityScale = 0f;
				yield return new WaitTime(this, summon.TimeToStop);
				this._sender.SetToggle(true);
				this._sender.Send();
				this._rigidybody.gravityScale = this._gravityScale;
			}
			GameObject gameObject;
			for (ushort i = 0; i < summon.QuantityToSummon; i++)
			{
				gameObject = summon.Summons[0];
				if (summon.Self)
					Instantiate(gameObject, this.transform.position, gameObject.transform.rotation, this.transform);
				else if (summon.Sequential)
				{
					gameObject = summon.Summons[i];
					Instantiate(gameObject, summon.SummonPoints[i], gameObject.transform.rotation, this.transform);
				}
				else if (summon.Random)
				{
					ushort pointIndex = (ushort)Random.Range(0f, summon.SummonPoints.Length - 1f);
					Instantiate(gameObject, summon.SummonPoints[pointIndex], gameObject.transform.rotation, this.transform);
				}
				else
					Instantiate(gameObject, summon.SummonPoints[0], gameObject.transform.rotation, this.transform);
			}
		}
		private new void Awake()
		{
			base.Awake();
			this._sender.SetStateForm(StateForm.State);
			this._gravityScale = this._rigidybody.gravityScale;
			foreach (SummonPlaces summonPlaces in this._summonPlaces)
			{
				SummonPoint summonPoint = summonPlaces.SummonPointObject;
				summonPoint = Instantiate(summonPoint, summonPlaces.Point, Quaternion.identity);
				summonPoint.GetTouch(() => this.Summon(summonPlaces.Summon));
			}
			if (this._randomTimedSummons)
			{
				this.StartCoroutine(RandomTimedSummon());
				IEnumerator RandomTimedSummon()
				{
					ushort randomIndex = (ushort)Random.Range(0f, this._timedSummons.Length);
					yield return TimedSummon(this._timedSummons[randomIndex]);
					yield return new WaitTime(this, this._timedSummons[randomIndex].PostSummonTime);
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
					yield return new WaitTime(this, summon.PostSummonTime);
					if (!this._randomTimedSummons)
						this.StartCoroutine(TimedSummon(summon));
				}
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			BossController[] bosses = (BossController[])additionalData;
			if (bosses != null)
				foreach (BossController boss in bosses)
					if (boss == this)
					{
						bool numberValid = data.NumberValue.HasValue && data.NumberValue.Value < this._eventSummons.Length;
						if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
							this._stopSummon = !data.ToggleValue.Value;
						else if (data.StateForm == StateForm.Action && this._reactToDamage && this._eventSummons.Length > 0f)
							if (this._randomReactSummons)
							{
								ushort randomIndex = (ushort)Random.Range(0f, this._eventSummons.Length - 1f);
								this.Summon(this._eventSummons[randomIndex]);
							}
							else if (numberValid && data.NumberValue.Value >= 0)
								this.Summon(this._eventSummons[data.NumberValue.Value]);
						break;
					}
		}
		[System.Serializable]
		private struct SummonPlaces
		{
			[SerializeField, Tooltip("The object to activate the summon.")] private SummonPoint _summonPointObject;
			[SerializeField, Tooltip("Which summon event the summon point will activate.")] private SummonObject _objectToSummon;
			[SerializeField, Tooltip("The point where the summon point will be.")] private Vector2 _point;
			internal readonly SummonPoint SummonPointObject => this._summonPointObject;
			internal readonly SummonObject Summon => this._objectToSummon;
			internal readonly Vector2 Point => this._point;
		};
	};
};
