using UnityEngine;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator)), RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
	internal abstract class EnemyController : StateController, IConnector, IDestructible
    {
		protected SpriteRenderer _spriteRenderer;
		protected Animator _animator;
		protected Rigidbody2D _rigidybody;
		protected Collider2D _collider;
		protected readonly Sender _sender = Sender.Create();
		private float _guardGravityScale = 0f;
		private float _stunTimer = 0f;
		private short _armorResistance = 0;
		protected bool _stopWorking = false;
		[Header("Enemy Controller")]
		[SerializeField, Tooltip("The layer mask to identify the ground.")] protected LayerMask _groundLayer;
		[SerializeField, Tooltip("The layer mask to identify the target of the attacks.")] protected LayerMask _targetLayerMask;
		[SerializeField, Tooltip("The vitality of the enemy.")] private short _vitality;
		[SerializeField, Tooltip("The amount of stun that this enemy can resists.")] private ushort _hitResistance;
		[SerializeField, Tooltip("The amount of damage that the enemy hit.")] private ushort _damage;
		[SerializeField, Tooltip("If this enemy receives no type of damage.")] private bool _noDamage;
		[SerializeField, Tooltip("If this enemy will fade away over time.")] private bool _fadeOverTime;
		[SerializeField, Tooltip("The amount of time this enemy will fade away.")] private float _timeToFadeAway;
		[SerializeField, Tooltip("The amount of time this enemy will stun.")] private float _stunTime;
		[SerializeField, Tooltip("The amount of time this enemy will be stunned when armor be broken.")] private float _stunnedTime;
		[SerializeField, Tooltip("The amount of time to stop the game when hit is given.")] private float _hitStopTime;
		[SerializeField, Tooltip("The amount of time to slow the game when hit is given.")] private float _hitSlowTime;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		protected bool IsStunned { get; private set; }
		public PathConnection PathConnection => PathConnection.Enemy;
		public short Health => this._vitality;
		protected new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
			this._animator = this.GetComponent<Animator>();
			this._rigidybody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<Collider2D>();
			this._sender.SetToWhereConnection(PathConnection.Enemy);
			this._sender.SetAdditionalData(this.gameObject);
			this._guardGravityScale = this._rigidybody.gravityScale;
			this._armorResistance = (short)this._hitResistance;
			if (this._fadeOverTime)
				Destroy(this.gameObject, this._timeToFadeAway);
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			SaveController.Load(out SaveFile saveFile);
			if (this._saveOnSpecifics && !saveFile.generalObjects.Contains(this.gameObject.name))
			{
				saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(saveFile);
			}
			Sender.Exclude(this);
		}
		protected void OnEnable()
		{
			this._animator.enabled = true;
			this._rigidybody.gravityScale = this._guardGravityScale;
		}
		protected void OnDisable() => this._animator.enabled = false;
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
			if (collisionObject.TryGetComponent<IDestructible>(out var destructible) && destructible.Hurt(this._damage))
			{
				destructible.Stun(this._damage, this._stunTime);
				EffectsController.HitStop(this._hitStopTime, this._hitSlowTime);
			}
		}
		private void OnTriggerEnter2D(Collider2D other) => this.OnTrigger(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => this.OnTrigger(other.gameObject);
		public bool Hurt(ushort damage)
		{
			if (this._noDamage || damage <= 0)
				return false;
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
				this._stunTimer = this._stunnedTime;
				this._armorResistance = (short)this._hitResistance;
			}
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Disable && data.ToggleValue.HasValue)
				this.enabled = data.ToggleValue.Value;
		}
	};
};
