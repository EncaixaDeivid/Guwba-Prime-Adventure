using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class ShooterEnemy : EnemyController, IDamageable
	{
		private readonly Sender _sender = Sender.Create();
		private Vector2 _targetDirection;
		private float _shootInterval = 0f;
		private float _timeStop = 0f;
		private float _gravityScale = 0f;
		private bool _isStopped = false;
		[Header("Shooter Enemy")]
		[SerializeField, Tooltip("The projectiles that this enemy can instantiate.")] private Projectile[] _projectiles;
		[SerializeField, Tooltip("The distance this enemy can detect the target.")] private float _perceptionDistance;
		[SerializeField, Tooltip("The angle fo the direction of ray of the detection.")] private float _rayAngleDirection;
		[SerializeField, Tooltip("The amount of time to wait to execute another shoot.")] private float _intervalToShoot;
		[SerializeField, Tooltip("The amount of time to stop this enemy to move.")] private float _stopTime;
		[SerializeField, Tooltip("If this enemy will become invencible while shooting. Requires: Defender Enemy")] private bool _invencibleShoot;
		[SerializeField, Tooltip("If this enemy gets hurt it will shoot.")] private bool _shootDamaged;
		[SerializeField, Tooltip("If this enemy will stop moving when shoot.")] private bool _stop;
		[SerializeField, Tooltip("If this enemy will paralyze moving.")] private bool _paralyze;
		[SerializeField, Tooltip("If this enemy will return the gravity after the paralyze.")] private bool _returnGravity;
		[SerializeField, Tooltip("If the detection will be circular.")] private bool _circulateDetection;
		[SerializeField, Tooltip("Will shoot to infinity without any detection.")] private bool _shootInfinity;
		[SerializeField, Tooltip("If this enemy won't interfere in the projectile.")] private bool _pureInstance;
		[SerializeField, Tooltip("If the projectile will be instantiate on the same point as this enemy.")] private bool _instanceOnSelf;
		private new void Awake()
		{
			base.Awake();
			this._sender.SetToWhereConnection(PathConnection.Enemy).SetAdditionalData(this.gameObject);
			this._gravityScale = this._rigidybody.gravityScale;
		}
		private void Shoot()
		{
			bool hasTarget = false;
			float originDirection = this._collider.bounds.extents.x * this._movementSide;
			Vector2 origin = new(this.transform.position.x + originDirection, this.transform.position.y);
			Vector2 direction = Quaternion.AngleAxis(this._rayAngleDirection, Vector3.forward) * Vector2.up;
			if (this._circulateDetection)
			{
				Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, this._perceptionDistance, this._targetLayerMask);
				foreach (Collider2D collider in colliders)
					if (collider.TryGetComponent<IDamageable>(out _))
					{
						if (Physics2D.Linecast(this.transform.position, collider.transform.position, this._groundLayer))
							continue;
						this._targetDirection = (collider.transform.position - this.transform.position).normalized;
						hasTarget = true;
					}
			}
			else
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(origin, direction, this._perceptionDistance, this._targetLayerMask))
					if (ray.collider.TryGetComponent<IDamageable>(out _))
						hasTarget = true;
			if ((hasTarget || this._shootInfinity) && this._shootInterval <= 0f)
			{
				this._shootInterval = this._intervalToShoot;
				if (this._invencibleShoot)
					this._sender.SetConnectionState(ConnectionState.Action).SetToggle(true).Send();
				if (this._stop)
				{
					this._timeStop = this._stopTime;
					this._isStopped = true;
					this._rigidybody.linearVelocity = Vector2.zero;
					this._sender.SetConnectionState(ConnectionState.State).SetToggle(false).Send();
					if (this._paralyze)
						this._rigidybody.gravityScale = 0f;
				}
				foreach (Projectile projectile in this._projectiles)
					if (this._pureInstance)
						Instantiate(projectile, this.transform.position, projectile.transform.rotation, this.transform);
					else
					{
						Vector2 position = this.transform.position;
						float angle = (Mathf.Atan2(this._targetDirection.y, this._targetDirection.x) * Mathf.Rad2Deg) - 90f;
						Quaternion rotation = Quaternion.AngleAxis(this._rayAngleDirection, Vector3.forward);
						if (this._circulateDetection)
							rotation = Quaternion.AngleAxis(angle, Vector3.forward);
						if (!this._instanceOnSelf)
							position += (Vector2)(rotation * Vector2.up);
						Instantiate(projectile, position, rotation, this.transform);
					}
			}
		}
		private void FixedUpdate()
		{
			if (this._stopMovement || this.Paralyzed)
				return;
			if (this._shootInterval > 0f)
				this._shootInterval -= Time.fixedDeltaTime;
			if (this._timeStop > 0f)
				this._timeStop -= Time.fixedDeltaTime;
			else if (this._timeStop <= 0f && this._isStopped)
			{
				this._isStopped = false;
				this._sender.SetConnectionState(ConnectionState.State).SetToggle(true).Send();
				if (this._invencibleShoot)
					this._sender.SetConnectionState(ConnectionState.Action).SetToggle(false).Send();
				if (this._returnGravity)
					this._rigidybody.gravityScale = this._gravityScale;
			}
			this.Shoot();
		}
		public new bool Damage(ushort damage)
		{
			if (this._shootDamaged)
				this.Shoot();
			return base.Damage(damage);
		}
	};
};
