using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer))]
	internal sealed class SummonerEnemy : EnemyController, IConnector
	{
		private float _gravityScale = 0f;
		private bool _stopSummon = false;
		[Header("Summoner Enemy")]
		[SerializeField, Tooltip("The summoner statitics of this enemy.")] private SummonerStatistics _statistics;
		private void Summon(SummonObject summon)
		{
			if (summon.StopToSummon)
				this.StartCoroutine(StopToSummon());
			IEnumerator StopToSummon()
			{
				this._sender.SetToggle(false);
				this._sender.Send(PathConnection.Enemy);
				if (summon.ParalyzeToSummon)
					this._rigidybody.gravityScale = 0f;
				yield return new WaitTime(this, summon.TimeToStop);
				this._sender.SetToggle(true);
				this._sender.Send(PathConnection.Enemy);
				this._rigidybody.gravityScale = this._gravityScale;
			}
			GameObject gameObject;
			Vector2 position;
			ushort summonIndex = 0;
			for (ushort i = 0; i < summon.QuantityToSummon; i++)
			{ 
				if (summon.Self)
					position = this.transform.position;
				else if (summon.Random)
					position = summon.SummonPoints[Random.Range(0, summon.SummonPoints.Length - 1)];
				else
					position = summon.SummonPoints[summonIndex];
				gameObject = summon.Summons[summonIndex];
				summonIndex = (ushort)(summonIndex >= summon.Summons.Length - 1f ? 0f : summonIndex + 1f);
				Instantiate(gameObject, position, gameObject.transform.rotation);
			}
		}
		private new void Awake()
		{
			base.Awake();
			this._sender.SetStateForm(StateForm.State);
			this._gravityScale = this._rigidybody.gravityScale;
			foreach (SummonPointStructure summonStructure in this._statistics.SummonPointStructures)
			{
				SummonPoint summonPoint = summonStructure.SummonPointObject;
				Instantiate(summonPoint, summonStructure.Point, Quaternion.identity).GetTouch(() => this.Summon(summonStructure.Summon));
			}
			if (this._statistics.RandomTimedSummons)
			{
				this.StartCoroutine(RandomTimedSummon());
				IEnumerator RandomTimedSummon()
				{
					ushort randomIndex = (ushort)Random.Range(0f, this._statistics.TimedSummons.Length);
					yield return TimedSummon(this._statistics.TimedSummons[randomIndex]);
					yield return new WaitTime(this, this._statistics.TimedSummons[randomIndex].PostSummonTime);
					this.StartCoroutine(RandomTimedSummon());
				}
			}
			else
				foreach (SummonObject summon in this._statistics.TimedSummons)
					this.StartCoroutine(TimedSummon(summon));
			IEnumerator TimedSummon(SummonObject summon)
			{
				yield return new WaitTime(this, summon.SummonTime);
				yield return new WaitUntil(() => !summon.StopTimedSummon && !this._stopSummon && this.isActiveAndEnabled);
				if (!summon.StopTimedSummon && !summon.StopPermanently && !this._stopSummon)
				{
					this.Summon(summon);
					yield return new WaitTime(this, summon.PostSummonTime);
					if (!this._statistics.RandomTimedSummons)
						this.StartCoroutine(TimedSummon(summon));
				}
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			EnemyController[] enemies = (EnemyController[])additionalData;
			if (enemies != null)
				foreach (EnemyController enemy in enemies)
					if (enemy != this)
						return;
			bool numberValid = data.NumberValue.HasValue && data.NumberValue.Value < this._statistics.EventSummons.Length;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				this._stopSummon = !data.ToggleValue.Value;
			else if (data.StateForm == StateForm.Action && this._statistics.ReactToDamage && this._statistics.EventSummons.Length > 0f)
				if (this._statistics.RandomReactSummons)
				{
					ushort randomIndex = (ushort)Random.Range(0f, this._statistics.EventSummons.Length - 1f);
					this.Summon(this._statistics.EventSummons[randomIndex]);
				}
				else if (numberValid && data.NumberValue.Value >= 0)
					this.Summon(this._statistics.EventSummons[data.NumberValue.Value]);
		}
	};
};
