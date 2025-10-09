using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[RequireComponent(typeof(Rigidbody2D))]
	internal abstract class MovingEnemy : EnemyProvider, IConnector, IDestructible
	{
		private Vector2 _guardVelocity = new();
		private bool _stunned = false;
		protected bool _detected = false;
		protected bool _isDashing = false;
		protected float _stoppedTime = 0f;
		protected short _movementSide = 1;
		[Header("Moving Enemy")]
		[SerializeField, Tooltip("The moving statitics of this enemy.")] private MovingStatistics _moving;
		[SerializeField, Tooltip("If this enemy will moves firstly to the left.")] private bool _invertMovementSide;
		protected new void Awake()
		{
			base.Awake();
			this._sender.SetStateForm(StateForm.State);
			this.transform.localScale = new Vector3()
			{
				x = (this._movementSide = (short)(this._invertMovementSide ? -1 : 1)) * Mathf.Abs(this.transform.localScale.x),
				y = this.transform.localScale.y,
				z = this.transform.localScale.z
			};
			Sender.Include(this);
		}
		protected new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		protected void OnEnable() => this._rigidybody.linearVelocity = this._guardVelocity;
		protected void OnDisable()
		{
			this._guardVelocity = this._rigidybody.linearVelocity;
			this._rigidybody.linearVelocity = Vector2.zero;
		}
		protected void Update()
		{
			if (!this.IsStunned && this._stunned)
			{
				this._stunned = false;
				this._rigidybody.linearVelocity = this._guardVelocity;
			}
		}
		protected bool SurfacePerception()
		{
			float groundChecker = this._moving.Physics.GroundChecker;
			float yOrigin = this._collider.offset.y + (this._collider.bounds.extents.y + groundChecker / 2f) * -this.transform.up.y;
			Vector2 origin = new(this.transform.position.x + this._collider.offset.x, this.transform.position.y + yOrigin);
			Vector2 size = new(this._collider.bounds.size.x - groundChecker, groundChecker);
			return Physics2D.BoxCast(origin, size, 0f, -this.transform.up, groundChecker, this._moving.Physics.GroundLayer);
		}
		public new void Stun(ushort stunStength, float stunTime)
		{
			base.Stun(stunStength, stunTime);
			this._stunned = true;
			this._guardVelocity = this._rigidybody.linearVelocity;
			this._rigidybody.linearVelocity = Vector2.zero;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
				if (enemy == this && data.StateForm == StateForm.State && data.ToggleValue.HasValue)
					this._stopWorking = !data.ToggleValue.Value;
		}
	};
};
