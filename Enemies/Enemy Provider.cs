using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[RequireComponent(typeof(Transform), typeof(EnemyController), typeof(Collider2D))]
	internal abstract class EnemyProvider : StateController, IDestructible
	{
		private EnemyController _controller;
		protected Rigidbody2D _rigidybody;
		protected Collider2D _collider;
		protected readonly Sender _sender = Sender.Create();
		protected bool _stopWorking = false;
		[Header("Enemy Provider")]
		[SerializeField, Tooltip("The enemies to send messages.")] private EnemyProvider[] _enemiesToSend;
		[SerializeField, Tooltip("The level of priority to use the destructible side.")] private ushort _destructilbePriority = 0;
		internal EnemyStatistics Statistics => this._controller.ProvidenceStatistics;
		public PathConnection PathConnection => PathConnection.Enemy;
		public short Health => this._controller.Health;
		internal ushort DestructilbePriority => this._destructilbePriority;
		internal bool IsStunned => this._controller.IsStunned;
		protected new void Awake()
		{
			base.Awake();
			this._controller = this.GetComponent<EnemyController>();
			this._rigidybody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<Collider2D>();
			this._sender.SetAdditionalData(this._enemiesToSend);
		}
		public bool Hurt(ushort damage)
		{
			if (this._controller.ProvidenceStatistics.ReactToDamage)
				if (this._controller.ProvidenceStatistics.HasIndex)
				{
					this._sender.SetStateForm(StateForm.Action);
					this._sender.SetNumber(this._controller.ProvidenceStatistics.IndexEvent);
					this._sender.Send(PathConnection.Enemy);
				}
				else
				{
					this._sender.SetStateForm(StateForm.Action);
					this._sender.Send(PathConnection.Enemy);
				}
			if ((this._controller.Vitality -= (short)damage) <= 0f)
				Destroy(this.gameObject);
			return true;
		}
		public void Stun(ushort stunStength, float stunTime)
		{
			this._controller.IsStunned = true;
			this._controller.StunTimer = stunTime;
			this._rigidybody.gravityScale = 0f;
			this._rigidybody.linearVelocity = Vector2.zero;
			if ((this._controller.ArmorResistance -= (short)stunStength) <= 0f)
			{
				this._rigidybody.gravityScale = this._controller.GuardGravityScale;
				this._controller.StunTimer = this._controller.ProvidenceStatistics.StunnedTime;
				this._controller.ArmorResistance = (short)this._controller.ProvidenceStatistics.HitResistance;
			}
		}
	};
};
