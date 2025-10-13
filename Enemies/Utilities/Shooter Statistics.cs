using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Shooter Enemy", menuName = "Enemy Statistics/Shooter", order = 5)]
	internal sealed class ShooterStatistics : ScriptableObject
	{
		[Header("Shooter Enemy")]
		[SerializeField, Tooltip("The physics of the enemy.")] private EnemyPhysics _physics;
		[SerializeField, Tooltip("The projectiles that this enemy can instantiate.")] private EnemyProjectile[] _projectiles;
		[SerializeField, Tooltip("The distance this enemy can detect the target.")] private float _perceptionDistance;
		[SerializeField, Tooltip("The angle fo the direction of ray of the detection.")] private float _rayAngleDirection;
		[SerializeField, Tooltip("The amount of time to wait to execute another shoot.")] private float _intervalToShoot;
		[SerializeField, Tooltip("The amount of time to stop this enemy to move.\nRequires: Moving Enemy")]
		private float _stopTime;
		[SerializeField, Tooltip("If this enemy will turn the ray to the looking side.")] private bool _turnRay;
		[SerializeField, Tooltip("If this enemy will become invencible while shooting.\nRequires: Defender Enemy")]
		private bool _invencibleShoot;
		[SerializeField, Tooltip("If this enemy gets hurt it will shoot.")] private bool _shootDamaged;
		[SerializeField, Tooltip("If this enemy will stop moving when shoot.\nRequires: Moving Enemy")]
		private bool _stop;
		[SerializeField, Tooltip("If this enemy will paralyze moving.\nRequires: Moving Enemy")]
		private bool _paralyze;
		[SerializeField, Tooltip("If this enemy will return the gravity after paralyze.\nRequires: Moving Enemy")]
		private bool _returnGravity;
		[SerializeField, Tooltip("If the detection will be circular.")] private bool _circularDetection;
		[SerializeField, Tooltip("Will shoot to infinity without any detection.")] private bool _shootInfinity;
		[SerializeField, Tooltip("If this enemy won't interfere in the projectile.")] private bool _pureInstance;
		[SerializeField, Tooltip("If the projectile will be instantiate on the same point as this enemy.")] private bool _instanceOnSelf;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		internal EnemyPhysics Physics => this._physics;
		internal EnemyProjectile[] Projectiles => this._projectiles;
		internal float PerceptionDistance => this._perceptionDistance;
		internal float RayAngleDirection => this._rayAngleDirection;
		internal float IntervalToShoot => this._intervalToShoot;
		internal float StopTime => this._stopTime;
		internal bool TurnRay => this._turnRay;
		internal bool InvencibleShoot => this._invencibleShoot;
		internal bool ShootDamaged => this._shootDamaged;
		internal bool Stop => this._stop;
		internal bool Paralyze => this._paralyze;
		internal bool ReturnGravity => this._returnGravity;
		internal bool CircularDetection => this._circularDetection;
		internal bool ShootInfinity => this._shootInfinity;
		internal bool PureInstance => this._pureInstance;
		internal bool InstanceOnSelf => this._instanceOnSelf;
		internal bool ReactToDamage => this._reactToDamage;
	};
};
