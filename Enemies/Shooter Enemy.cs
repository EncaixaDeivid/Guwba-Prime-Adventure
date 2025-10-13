using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class ShooterEnemy : EnemyProvider, IConnector, IDestructible
	{
		private Vector2 _targetDirection;
		private float _shootInterval = 0f;
		private float _timeStop = 0f;
		private float _gravityScale = 0f;
		private bool _canShoot = false;
		private bool _isStopped = false;
		[Header("Shooter Enemy")]
		[SerializeField, Tooltip("The shooter statitics of this enemy.")] private ShooterStatistics _statistics;
		private new void Awake()
		{
			base.Awake();
			this._gravityScale = this._rigidybody.gravityScale;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void Verify()
		{
			bool hasTarget = false;
			float originDirection = this._collider.bounds.extents.x * (this.transform.localScale.x < 0f ? -1f : 1f);
			Vector2 origin = new(this.transform.position.x + originDirection, this.transform.position.y);
			Vector2 direction = Quaternion.AngleAxis(this._statistics.RayAngleDirection, Vector3.forward) * Vector2.up;
			if (this._statistics.TurnRay)
				direction *= this.transform.localScale.x < 0f ? -1f : 1f;
			LayerMask targetLayer = this._statistics.Physics.TargetLayer;
			float perceptionDistance = this._statistics.PerceptionDistance;
			if (this._statistics.CircularDetection)
			{
				Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, perceptionDistance, targetLayer);
				foreach (Collider2D collider in colliders)
					if (collider.TryGetComponent<IDestructible>(out _))
					{
						if (Physics2D.Linecast(this.transform.position, collider.transform.position, this._statistics.Physics.GroundLayer))
							continue;
						this._targetDirection = (collider.transform.position - this.transform.position).normalized;
						hasTarget = true;
					}
			}
			else
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(origin, direction, perceptionDistance, targetLayer))
					if (ray.collider.TryGetComponent<IDestructible>(out _))
						hasTarget = true;
			if ((hasTarget || this._statistics.ShootInfinity) && this._shootInterval <= 0f)
			{
				this._shootInterval = this._statistics.IntervalToShoot;
				if (this._statistics.InvencibleShoot)
				{
					this._sender.SetStateForm(StateForm.Action);
					this._sender.SetToggle(true);
					this._sender.Send(PathConnection.Enemy);
				}
				if (this._statistics.Stop)
				{
					this._canShoot = true;
					this._timeStop = this._statistics.StopTime;
					this._isStopped = true;
					this._rigidybody.linearVelocity = Vector2.zero;
					this._sender.SetStateForm(StateForm.State);
					this._sender.SetToggle(false);
					this._sender.Send(PathConnection.Enemy);
					if (this._statistics.Paralyze)
						this._rigidybody.gravityScale = 0f;
				}
				else
					this.Shoot();
			}
		}
		private void Shoot()
		{
			foreach (EnemyProjectile projectile in this._statistics.Projectiles)
				if (this._statistics.PureInstance)
					Instantiate(projectile, this.transform.position, projectile.transform.rotation, this.transform);
				else
				{
					Vector2 position = this.transform.position;
					float angle = (Mathf.Atan2(this._targetDirection.y, this._targetDirection.x) * Mathf.Rad2Deg) - 90f;
					Quaternion rotation = Quaternion.AngleAxis(this._statistics.RayAngleDirection, Vector3.forward);
					if (this._statistics.CircularDetection)
						rotation = Quaternion.AngleAxis(angle, Vector3.forward);
					if (!this._statistics.InstanceOnSelf)
						position += (Vector2)(rotation * Vector2.up);
					Instantiate(projectile, position, rotation, this.transform);
				}
			if (this._statistics.InvencibleShoot)
			{
				this._sender.SetStateForm(StateForm.Action);
				this._sender.SetToggle(false);
				this._sender.Send(PathConnection.Enemy);
			}
		}
		private void Update()
		{
			if (this._stopWorking || this._rigidybody.IsSleeping())
				return;
			if (this._shootInterval > 0f && !this._isStopped)
				this._shootInterval -= Time.deltaTime;
			if (this._timeStop > 0f)
				this._timeStop -= Time.deltaTime;
			if (this._statistics.Stop && this._canShoot && this._timeStop <= this._statistics.StopTime / 2f)
			{
				this._canShoot = false;
				this.Shoot();
			}
			if (this._timeStop <= 0f && this._isStopped)
			{
				this._isStopped = false;
				this._sender.SetStateForm(StateForm.State);
				this._sender.SetToggle(true);
				this._sender.Send(PathConnection.Enemy);
				if (this._statistics.ReturnGravity)
					this._rigidybody.gravityScale = this._gravityScale;
			}
			this.Verify();
		}
		public new bool Hurt(ushort damage)
		{
			if (this._statistics.ShootDamaged)
				this.Shoot();
			return base.Hurt(damage);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if ((EnemyProvider[])additionalData != null)
				foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
					if (enemy == this && data.StateForm == StateForm.Action && this._statistics.ReactToDamage)
						this.Shoot();
		}
	};
};
