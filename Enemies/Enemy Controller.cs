using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[RequireComponent(typeof(Transform), typeof(Rigidbody2D), typeof(Collider2D))]
	internal sealed class EnemyController : StateController, IConnector, IDestructible
    {
		private EnemyProvider[] _selfEnemies;
		private Rigidbody2D _rigidybody;
		private short _vitality;
		private short _armorResistance = 0;
		private float _stunTimer = 0f;
		private bool _stunned = false;
		[Header("Enemy Statistics")]
		[SerializeField, Tooltip("The control statitics of this enemy.")] private EnemyStatistics _statistics;
		internal EnemyStatistics ProvidenceStatistics => this._statistics;
		public PathConnection PathConnection => PathConnection.Enemy;
		public short Health => this._vitality;
		internal short Vitality { get => this._vitality; set => this._vitality = value; }
		internal short ArmorResistance { get => this._armorResistance; set => this._armorResistance = value; }
		internal float StunTimer { get => this._stunTimer; set => this._stunTimer = value; }
		internal bool IsStunned { get => this._stunned; set => this._stunned = value; }
		private new void Awake()
		{
			base.Awake();
			this._selfEnemies = this.GetComponents<EnemyProvider>();
			this._rigidybody = this.GetComponent<Rigidbody2D>();
			this._vitality = (short)this._statistics.Vitality;
			this._armorResistance = (short)this._statistics.HitResistance;
			if (this._statistics.FadeOverTime)
				Destroy(this.gameObject, this._statistics.TimeToFadeAway);
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			SaveController.Load(out SaveFile saveFile);
			if (this._statistics.SaveOnSpecifics&& !saveFile.generalObjects.Contains(this.gameObject.name))
			{
				saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(saveFile);
			}
			Sender.Exclude(this);
		}
		private void OnEnable() => this._rigidybody.WakeUp();
		private void OnDisable() => this._rigidybody.Sleep();
		private IEnumerator Start()
		{
			foreach (EnemyProvider enemy in this._selfEnemies)
				enemy.enabled = false;
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			foreach (EnemyProvider enemy in this._selfEnemies)
				enemy.enabled = true;
		}
		private void Update()
		{
			if (this._stunned)
			{
				this._stunTimer -= Time.deltaTime;
				if (this._stunTimer <= 0f)
				{
					this._stunned = false;
					this._rigidybody.WakeUp();
				}
			}
		}
		private void OnTrigger(GameObject collisionObject)
		{
			if (collisionObject.TryGetComponent<IDestructible>(out var destructible) && destructible.Hurt(this._statistics.Damage))
			{
				destructible.Stun(this._statistics.Damage, this._statistics.StunTime);
				EffectsController.HitStop(this._statistics.Physics.HitStopTime, this._statistics.Physics.HitSlowTime);
			}
		}
		private void OnTriggerEnter2D(Collider2D other) => this.OnTrigger(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => this.OnTrigger(other.gameObject);
		public bool Hurt(ushort damage)
		{
			if (this._statistics.NoDamage || damage <= 0)
				return false;
			ushort priority = 0;
			for (ushort i = 0; i < this._selfEnemies.Length - 1f; i++)
			{
				if (this._selfEnemies[i + 1].DestructilbePriority > this._selfEnemies[i].DestructilbePriority)
					priority = (ushort)(i + 1);
			}
			return this._selfEnemies[priority].Hurt(damage);
		}
		public void Stun(ushort stunStength, float stunTime)
		{
			if (this._stunned)
				return;
			ushort priority = 0;
			for (ushort i = 0; i < this._selfEnemies.Length - 1f; i++)
			{
				if (this._selfEnemies[i + 1].DestructilbePriority > this._selfEnemies[i].DestructilbePriority)
					priority = (ushort)(i + 1);
			}
			this._selfEnemies[priority].Stun(stunStength, stunTime);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.None && data.ToggleValue.HasValue)
			{
				this._rigidybody.Sleep();
				foreach (EnemyProvider enemy in this._selfEnemies)
					enemy.enabled = data.ToggleValue.Value;
			}
		}
	};
};
