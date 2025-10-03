using UnityEngine;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[RequireComponent(typeof(Transform), typeof(Rigidbody2D)), RequireComponent(typeof(Collider2D))]
	internal abstract class EnemyController : StateController, IConnector, IDestructible
    {
		protected Rigidbody2D _rigidybody;
		protected Collider2D _collider;
		protected readonly Sender _sender = Sender.Create();
		private float _guardGravityScale = 0f;
		private float _stunTimer = 0f;
		private short _vitality;
		private short _armorResistance = 0;
		protected bool _stopWorking = false;
		[Header("Enemy Controller")]
		[SerializeField, Tooltip("The control statitics of this enemy.")] private EnemyStatistics _control;
		[SerializeField, Tooltip("The enemies to send messages.")] private EnemyController[] _enemiesToSend;
		protected bool IsStunned { get; private set; }
		public PathConnection PathConnection => PathConnection.Enemy;
		public short Health => this._vitality;
		protected new void Awake()
		{
			base.Awake();
			this._rigidybody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<Collider2D>();
			this._sender.SetAdditionalData(this._enemiesToSend);
			this._guardGravityScale = this._rigidybody.gravityScale;
			this._vitality = (short)this._control.Vitality;
			this._armorResistance = (short)this._control.HitResistance;
			if (this._control.FadeOverTime)
				Destroy(this.gameObject, this._control.TimeToFadeAway);
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			SaveController.Load(out SaveFile saveFile);
			if (this._control.SaveOnSpecifics&& !saveFile.generalObjects.Contains(this.gameObject.name))
			{
				saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(saveFile);
			}
			Sender.Exclude(this);
		}
		protected void OnEnable() => this._rigidybody.gravityScale = this._guardGravityScale;
		protected void Update()
		{
			if (this.IsStunned)
			{
				this._stunTimer -= Time.deltaTime;
				if (this._stunTimer <= 0f)
					this.IsStunned = false;
			}
		}
		private void OnTrigger(GameObject collisionObject)
		{
			if (collisionObject.TryGetComponent<IDestructible>(out var destructible) && destructible.Hurt(this._control.Damage))
			{
				destructible.Stun(this._control.Damage, this._control.StunTime);
				EffectsController.HitStop(this._control.Physics.HitStopTime, this._control.Physics.HitSlowTime);
			}
		}
		private void OnTriggerEnter2D(Collider2D other) => this.OnTrigger(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => this.OnTrigger(other.gameObject);
		public bool Hurt(ushort damage)
		{
			if (this._control.NoDamage|| damage <= 0)
				return false;
			if (this._control.ReactToDamage)
				if (this._control.HasIndex)
				{
					this._sender.SetStateForm(StateForm.Action);
					this._sender.SetNumber(this._control.IndexEvent);
					this._sender.Send(PathConnection.Enemy);
				}
				else
				{
					this._sender.SetStateForm(StateForm.Action);
					this._sender.Send(PathConnection.Enemy);
				}
			if ((this._vitality -= (short)damage) <= 0f)
				Destroy(this.gameObject);
			return true;
		}
		public void Stun(ushort stunStength, float stunTime)
		{
			if (this.IsStunned)
				return;
			this.IsStunned = true;
			this._stunTimer = stunTime;
			if ((this._armorResistance -= (short)stunStength) <= 0f)
			{
				this._stunTimer = this._control.StunnedTime;
				this._armorResistance = (short)this._control.HitResistance;
			}
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.None && data.ToggleValue.HasValue)
				this.enabled = data.ToggleValue.Value;
		}
	};
};
