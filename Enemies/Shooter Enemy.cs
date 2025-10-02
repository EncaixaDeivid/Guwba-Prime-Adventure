using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class ShooterEnemy : EnemyController, IConnector, IDestructible
	{
		private Vector2 _targetDirection;
		private float _shootInterval = 0f;
		private float _timeStop = 0f;
		private float _gravityScale = 0f;
		private bool _hasTarget = false;
		private bool _isStopped = false;
		[Header("Shooter Enemy")]
		[SerializeField, Tooltip("The projectiles that this enemy can instantiate.")] private EnemyProjectile[] _projectiles;
		[SerializeField, Tooltip("The distance this enemy can detect the target.")] private float _perceptionDistance;
		[SerializeField, Tooltip("The angle fo the direction of ray of the detection.")] private float _rayAngleDirection;
		[SerializeField, Tooltip("The amount of time to wait to execute another shoot.")] private float _intervalToShoot;
		[SerializeField, Tooltip("The amount of time to stop this enemy to move.\nRequires: Moving Enemy")]
		private float _stopTime;
		[SerializeField, Tooltip("If this enemy will become invencible while shooting.\nRequires: Defender Enemy")]
		private bool _invencibleShoot;
		[SerializeField, Tooltip("If this enemy gets hurt it will shoot.")] private bool _shootDamaged;
		[SerializeField, Tooltip("If this enemy will stop moving when shoot.\nRequires: Moving Enemy")]
		private bool _stop;
		[SerializeField, Tooltip("If this enemy will paralyze moving.\nRequires: Moving Enemy")]
		private bool _paralyze;
		[SerializeField, Tooltip("If this enemy will return the gravity after paralyze.\nRequires: Moving Enemy")]
		private bool _returnGravity;
		[SerializeField, Tooltip("If the detection will be circular.")] private bool _circulateDetection;
		[SerializeField, Tooltip("Will shoot to infinity without any detection.")] private bool _shootInfinity;
		[SerializeField, Tooltip("If this enemy won't interfere in the projectile.")] private bool _pureInstance;
		[SerializeField, Tooltip("If the projectile will be instantiate on the same point as this enemy.")] private bool _instanceOnSelf;
		private new void Awake()
		{
			base.Awake();
			this._gravityScale = this._rigidybody.gravityScale;
		}
		private void Verify()
		{
			this._hasTarget = false;
			float originDirection = this._collider.bounds.extents.x * (this.transform.right.x < 0f ? -1f : 1f);
			Vector2 origin = new(this.transform.position.x + originDirection, this.transform.position.y);
			Vector2 direction = Quaternion.AngleAxis(this._rayAngleDirection, Vector3.forward) * Vector2.up;
			if (this._circulateDetection)
			{
				Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, this._perceptionDistance, this._targetLayerMask);
				foreach (Collider2D collider in colliders)
					if (collider.TryGetComponent<IDestructible>(out _))
					{
						if (Physics2D.Linecast(this.transform.position, collider.transform.position, this._groundLayer))
							continue;
						this._targetDirection = (collider.transform.position - this.transform.position).normalized;
						this._hasTarget = true;
					}
			}
			else
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(origin, direction, this._perceptionDistance, this._targetLayerMask))
					if (ray.collider.TryGetComponent<IDestructible>(out _))
						this._hasTarget = true;
			if ((this._hasTarget || this._shootInfinity) && this._shootInterval <= 0f)
			{
				this._shootInterval = this._intervalToShoot;
				if (this._invencibleShoot)
				{
					this._sender.SetStateForm(StateForm.Action);
					this._sender.SetToggle(true);
					this._sender.Send(PathConnection.Enemy);
				}
				if (this._stop)
				{
					this._timeStop = this._stopTime;
					this._isStopped = true;
					this._rigidybody.linearVelocity = Vector2.zero;
					this._sender.SetStateForm(StateForm.State);
					this._sender.SetToggle(false);
					this._sender.Send(PathConnection.Enemy);
					if (this._paralyze)
						this._rigidybody.gravityScale = 0f;
				}
			}
		}
		private void Shoot()
		{
			foreach (EnemyProjectile projectile in this._projectiles)
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
		private new void Update()
		{
			base.Update();
			if (this._stopWorking || this.IsStunned)
				return;
			if (this._shootInterval > 0f)
				this._shootInterval -= Time.deltaTime;
			if (this._timeStop > 0f)
				this._timeStop -= Time.deltaTime;
			if (this._hasTarget && this._timeStop <= this._stopTime / 2f)
				this.Shoot();
			if (this._timeStop <= 0f && this._isStopped)
			{
				this._isStopped = false;
				this._sender.SetStateForm(StateForm.State);
				this._sender.SetToggle(true);
				this._sender.Send(PathConnection.Enemy);
				if (this._invencibleShoot)
				{
					this._sender.SetStateForm(StateForm.Action);
					this._sender.SetToggle(false);
					this._sender.Send(PathConnection.Enemy);
				}
				if (this._returnGravity)
					this._rigidybody.gravityScale = this._gravityScale;
			}
			this.Verify();
		}
		public new bool Hurt(ushort damage)
		{
			if (this._shootDamaged)
				this.Verify();
			return base.Hurt(damage);
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			EnemyController[] enemies = (EnemyController[])additionalData;
			if (enemies != null)
				foreach (EnemyController enemy in enemies)
					if (enemy == this && data.StateForm == StateForm.Action && this._reactToDamage)
						this.Verify();
		}
	};
};
