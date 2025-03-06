using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class ShooterEnemy : EnemyController
	{
		private Vector2 _targetDirection;
		private float _shootInterval = 0f, _timeStop = 0f, _gravityScale = 0f;
		private bool _isStopped = false;
		[Header("Shooter Enemy"), SerializeField] private Projectile[] _projectiles;
		[SerializeField] private float _perceptionDistance, _rayAngleDirection, _intervalToShoot, _stopTime;
		[SerializeField] private bool _stop, _paralyze, _returnGravity, _circulateDetection, _shootInfinity, _pureInstance, _instanceOnSelf;
		private new void Awake()
		{
			base.Awake();
			this._gravityScale = this._rigidybody.gravityScale;
			this._toggleEvent = (bool toggleValue) => this._stopMovement = !toggleValue;
		}
		private bool AimPerception()
		{
			float originDirection = this._collider.bounds.extents.x * this._movementSide;
			Vector2 origin = new(this.transform.position.x + originDirection, this.transform.position.y);
			Vector2 direction = Quaternion.AngleAxis(this._rayAngleDirection, Vector3.forward) * Vector2.up;
			if (this._circulateDetection)
			{
				foreach (Collider2D collider in Physics2D.OverlapCircleAll(origin, this._perceptionDistance, this._targetLayerMask))
					if (collider.TryGetComponent<IDamageable>(out _))
					{
						this._targetDirection = collider.transform.position - this.transform.position;
						return true;
					}
			}
			else
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(origin, direction, this._perceptionDistance, this._targetLayerMask))
					if (ray.collider.TryGetComponent<IDamageable>(out _))
						return true;
			return false;
		}
		private void FixedUpdate()
		{
			if (this._stopMovement || this.Paralyzed)
				return;
			if (this._shootInterval > 0f)
				this._shootInterval -= Time.deltaTime;
			if (this._timeStop > 0f)
				this._timeStop -= Time.deltaTime;
			else if (this._timeStop <= 0f && this._isStopped)
			{
				this._isStopped = false;
				this.Toggle<GroundEnemy>(true);
				this.Toggle<FlyingEnemy>(true);
				if (this._returnGravity)
					this._rigidybody.gravityScale = this._gravityScale;
			}
			if ((this.AimPerception() || this._shootInfinity) && this._shootInterval <= 0f)
			{
				this._shootInterval = this._intervalToShoot;
				if (this._stop)
				{
					this._timeStop = this._stopTime;
					this._isStopped = true;
					this._rigidybody.linearVelocity = Vector2.zero;
					this.Toggle<GroundEnemy>(false);
					this.Toggle<FlyingEnemy>(false);
					if (this._paralyze)
						this._rigidybody.gravityScale = 0f;
				}
				foreach (Projectile projectile in this._projectiles)
				{
					float positionDirection = this._collider.bounds.extents.x * this._movementSide;
					Vector2 position = this.transform.position;
					if (!this._instanceOnSelf)
						position = new(this.transform.position.x + positionDirection, this.transform.position.y);
					if (this._pureInstance)
						Instantiate(projectile, position, projectile.transform.rotation, this.transform);
					else
					{
						float angle = (Mathf.Atan2(this._targetDirection.y, this._targetDirection.x) * Mathf.Rad2Deg) - 90f;
						Quaternion rotation = Quaternion.AngleAxis(this._rayAngleDirection, Vector3.forward);
						if (this._circulateDetection)
							rotation = Quaternion.AngleAxis(angle, Vector3.forward);
						Instantiate(projectile, position, rotation, this.transform);
					}
				}
			}
		}
	};
};
