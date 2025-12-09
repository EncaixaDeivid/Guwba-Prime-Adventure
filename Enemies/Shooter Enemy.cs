using UnityEngine;
using GwambaPrimeAdventure.Character;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class ShooterEnemy : EnemyProvider, IConnector, IDestructible
	{
		private Vector2 _originCast = Vector2.zero;
		private Vector2 _directionCast = Vector2.zero;
		private Vector2 _targetDirection = Vector2.zero;
		private InstantiateParameters _projectileInstance;
		private float _shootInterval = 0F;
		private float _timeStop = 0F;
		private bool _hasTarget = false;
		private bool _canShoot = false;
		private bool _isStopped = false;
		[Header("Shooter Enemy")]
		[SerializeField, Tooltip("The shooter statitics of this enemy.")] private ShooterStatistics _statistics;
		private new void Awake()
		{
			base.Awake();
			_projectileInstance = new() { parent = transform, worldSpace = false };
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void Shoot()
		{
			Quaternion rotation = Quaternion.identity;
			if (!_statistics.PureInstance)
				if (_statistics.CircularDetection)
					rotation = Quaternion.AngleAxis((Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg) - 90F, Vector3.forward);
				else
					rotation = Quaternion.AngleAxis(_statistics.RayAngleDirection * (_statistics.TurnRay ? (transform.localScale.x < 0F ? -1F : 1F) : 1F), Vector3.forward);
			for (ushort i = 0; i < _statistics.Projectiles.Length; i++)
				if (_statistics.PureInstance)
					Instantiate(_statistics.Projectiles[i], _statistics.SpawnPoint, _statistics.Projectiles[i].transform.rotation, _projectileInstance).transform.SetParent(null);
				else
					Instantiate(_statistics.Projectiles[i], _statistics.SpawnPoint, rotation, _projectileInstance).transform.SetParent(null);
			if (_statistics.InvencibleShoot)
			{
				_sender.SetFormat(MessageFormat.Event);
				_sender.SetToggle(true);
				_sender.Send(MessagePath.Enemy);
			}
		}
		private void Update()
		{
			if (_stopWorking || IsStunned)
				return;
			if (_shootInterval > 0F && !_isStopped)
				_shootInterval -= Time.deltaTime;
			if (_timeStop > 0F)
			{
				if ((_timeStop -= Time.deltaTime) <= _statistics.StopTime / 2F && _statistics.Stop && _canShoot)
				{
					_canShoot = false;
					Shoot();
				}
				if (_timeStop <= 0F && _isStopped)
				{
					_isStopped = false;
					_sender.SetFormat(MessageFormat.State);
					_sender.SetToggle(true);
					_sender.Send(MessagePath.Enemy);
				}
			}
		}
		private void FixedUpdate()
		{
			_hasTarget = false;
			if (_shootInterval <= 0F)
				if (_statistics.CircularDetection)
				{
					foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, _statistics.PerceptionDistance, WorldBuild.CHARACTER_LAYER))
						if (collider.TryGetComponent<IDestructible>(out _))
							if (!Physics2D.Linecast(transform.position, collider.transform.position, WorldBuild.SCENE_LAYER))
							{
								_targetDirection = (collider.transform.position - transform.position).normalized;
								transform.TurnScaleX(transform.position.x < collider.transform.position.x);
								_hasTarget = true;
								break;
							}
				}
				else
				{
					_originCast = (Vector2)transform.position + _collider.offset;
					_originCast.x += _collider.bounds.extents.x * (transform.localScale.x < 0F ? -1F : 1F);
					_directionCast = Quaternion.AngleAxis(_statistics.RayAngleDirection, Vector3.forward) * Vector2.up;
					if (_statistics.TurnRay)
						_directionCast *= transform.localScale.x < 0F ? -1F : 1F;
					foreach (RaycastHit2D ray in Physics2D.RaycastAll(_originCast, _directionCast, _statistics.PerceptionDistance, WorldBuild.CHARACTER_LAYER))
						if (ray.collider.TryGetComponent<IDestructible>(out _))
						{
							_hasTarget = true;
							break;
						}
				}
			if ((_hasTarget || _statistics.ShootInfinity) && _shootInterval <= 0F)
			{
				_shootInterval = _statistics.IntervalToShoot;
				if (_statistics.InvencibleShoot)
				{
					_sender.SetFormat(MessageFormat.Event);
					_sender.SetToggle(false);
					_sender.Send(MessagePath.Enemy);
				}
				if (_statistics.Stop)
				{
					_timeStop = _statistics.StopTime;
					_sender.SetFormat(MessageFormat.State);
					_sender.SetToggle(!(_isStopped = _canShoot = true));
					_sender.Send(MessagePath.Enemy);
					if (_statistics.Paralyze)
						Rigidbody.Sleep();
				}
				else
					Shoot();
			}
		}
		public new bool Hurt(ushort damage)
		{
			if (_statistics.ShootDamaged)
				Shoot();
			return base.Hurt(damage);
		}
		public void Receive(MessageData message)
		{
			if (message.AdditionalData is not null && message.AdditionalData is EnemyProvider[] && (message.AdditionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in message.AdditionalData as EnemyProvider[])
					if (enemy && enemy == this && message.Format == MessageFormat.Event && _statistics.ReactToDamage)
					{
						_targetDirection = (GwambaStateMarker.Localization - (Vector2)transform.position).normalized;
						transform.TurnScaleX(GwambaStateMarker.Localization.x < transform.position.x);
						Shoot();
						return;
					}
		}
	};
};
