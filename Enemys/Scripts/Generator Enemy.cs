using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class GeneratorEnemy : EnemyController
	{
		private readonly List<GameObject> _enemysGenerated = new();
		private float _timeGeneration = 0f, _gravityScale = 0f;
		private bool _continueGeneration = true, _stopGenerate = false;
		[Header("Generator Enemy"), SerializeField] private SummonObject _summonObject;
		[SerializeField] private bool _especifiedGeneration, _existentEnemys;
		private new void Awake()
		{
			base.Awake();
			this._gravityScale = this._rigidybody.gravityScale;
		}
		private void FixedUpdate()
		{
			if (this._stopGenerate)
				return;
			if (this._continueGeneration)
				if (this._timeGeneration > 0f)
					this._timeGeneration -= Time.deltaTime;
				else if (this._timeGeneration <= 0f)
				{
					this._timeGeneration = this._summonObject.SummonTime;
					if (this._summonObject.StopToSummon)
						this.StartCoroutine(StopToSummon());
					IEnumerator StopToSummon()
					{
						this._rigidybody.linearVelocityX = 0f;
						if (this._summonObject.ParalyzeToSummon)
							this._rigidybody.gravityScale = 0f;
						yield return new WaitTime(this, this._summonObject.TimeToStop);
						this._rigidybody.gravityScale = this._gravityScale;
					}
					GameObject summon = null;
					Vector2 combinePoint = (Vector2)this.transform.position + this._summonObject.SummonPoints[0];
					if (this._summonObject.Self)
						summon = Instantiate(this._summonObject.Summon, this.transform.position, this.transform.rotation);
					else if (this._summonObject.Combine)
						summon = Instantiate(this._summonObject.Summon, combinePoint, this.transform.rotation);
					else if (this._summonObject.Random)
					{
						ushort pointIndex = (ushort)Random.Range(0f, this._summonObject.SummonPoints.Length - 1f);
						summon = Instantiate(this._summonObject.Summon, this._summonObject.SummonPoints[0], this.transform.rotation);
					}
					else
						summon = Instantiate(this._summonObject.Summon, this._summonObject.SummonPoints[0], this.transform.rotation);
					this._enemysGenerated.Add(summon);
				}
			if (this._existentEnemys && !this._especifiedGeneration)
			{
				this._enemysGenerated.RemoveAll(summon => !summon);
				this._continueGeneration = this._summonObject.QuantityToSummon != this._enemysGenerated.Count;
			}
			else if (this._especifiedGeneration && !this._existentEnemys && this._summonObject.QuantityToSummon == this._enemysGenerated.Count)
				this._stopGenerate = true;
		}
	};
};