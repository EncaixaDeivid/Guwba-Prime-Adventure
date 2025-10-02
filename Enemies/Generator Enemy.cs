using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class GeneratorEnemy : EnemyController
	{
		private readonly List<GameObject> _enemysGenerated = new();
		private float _timeGeneration = 0f;
		private float _gravityScale = 0f;
		private bool _continueGeneration = true;
		private bool _stopGenerate = false;
		[Header("Generator Enemy")]
		[SerializeField, Tooltip("The object that will be summoned.")] private SummonObject _summonObject;
		[SerializeField, Tooltip("If this enemy has a limited generation.")] private bool _especifiedGeneration;
		[SerializeField, Tooltip("If it always generate to a existence of enemies.")] private bool _existentEnemys;
		private new void Awake()
		{
			base.Awake();
			this._gravityScale = this._rigidybody.gravityScale;
		}
		private new void OnEnable()
		{
			base.OnEnable();
			foreach (GameObject gameObject in this._enemysGenerated.FindAll(gameObject => gameObject))
				gameObject.SetActive(true);
		}
		private void OnDisable()
		{
			foreach (GameObject gameObject in this._enemysGenerated.FindAll(gameObject => gameObject))
				gameObject.SetActive(false);
		}
		private new void Update()
		{
			base.Update();
			if (this._stopGenerate)
				return;
			if (this._continueGeneration)
			{
				if (this._timeGeneration > 0f)
					this._timeGeneration -= Time.deltaTime;
				if (this._timeGeneration <= 0f)
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
					GameObject enemySummon = this._summonObject.Summons[0];
					if (this._summonObject.Self)
						summon = Instantiate(enemySummon, this.transform.position, this.transform.rotation);
					else if (this._summonObject.Random)
					{
						ushort pointIndex = (ushort)Random.Range(0f, this._summonObject.SummonPoints.Length - 1f);
						summon = Instantiate(enemySummon, this._summonObject.SummonPoints[pointIndex], this.transform.rotation);
					}
					else
						summon = Instantiate(enemySummon, this._summonObject.SummonPoints[0], this.transform.rotation);
					this._enemysGenerated.Add(summon);
				}
			}
			bool valid = this._summonObject.QuantityToSummon == this._enemysGenerated.Count;
			if (this._existentEnemys && !this._especifiedGeneration)
			{
				this._enemysGenerated.RemoveAll(summon => !summon);
				this._continueGeneration = this._summonObject.QuantityToSummon != this._enemysGenerated.Count;
			}
			else if (this._especifiedGeneration && !this._existentEnemys && valid)
				this._stopGenerate = true;
		}
	};
};
