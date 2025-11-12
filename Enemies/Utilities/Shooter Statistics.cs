using UnityEngine;
namespace GwambaPrimeAdventure.Enemy
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
		[SerializeField, Tooltip("The amount of time to stop this enemy to move.\nRequires: Moving Enemy")] private float _stopTime;
		[SerializeField, Tooltip("If this enemy will turn the ray to the looking side.")] private bool _turnRay;
		[SerializeField, Tooltip("If this enemy will become invencible while shooting.\nRequires: Defender Enemy")]	private bool _invencibleShoot;
		[SerializeField, Tooltip("If this enemy gets hurt it will shoot.")] private bool _shootDamaged;
		[SerializeField, Tooltip("If this enemy will stop moving when shoot.\nRequires: Moving Enemy")]	private bool _stop;
		[SerializeField, Tooltip("If this enemy will paralyze moving.\nRequires: Moving Enemy")] private bool _paralyze;
		[SerializeField, Tooltip("If this enemy will return the gravity after paralyze.\nRequires: Moving Enemy")] private bool _returnParalyze;
		[SerializeField, Tooltip("If the detection will be circular.")] private bool _circularDetection;
		[SerializeField, Tooltip("Will shoot to infinity without any detection.")] private bool _shootInfinity;
		[SerializeField, Tooltip("If this enemy won't interfere in the projectile.")] private bool _pureInstance;
		[SerializeField, Tooltip("If the projectile will be instantiate on the same point as this enemy.")] private bool _instanceOnSelf;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		internal EnemyPhysics Physics => _physics;
		internal EnemyProjectile[] Projectiles => _projectiles;
		internal float PerceptionDistance => _perceptionDistance;
		internal float RayAngleDirection => _rayAngleDirection;
		internal float IntervalToShoot => _intervalToShoot;
		internal float StopTime => _stopTime;
		internal bool TurnRay => _turnRay;
		internal bool InvencibleShoot => _invencibleShoot;
		internal bool ShootDamaged => _shootDamaged;
		internal bool Stop => _stop;
		internal bool Paralyze => _paralyze;
		internal bool ReturnParalyze => _returnParalyze;
		internal bool CircularDetection => _circularDetection;
		internal bool ShootInfinity => _shootInfinity;
		internal bool PureInstance => _pureInstance;
		internal bool InstanceOnSelf => _instanceOnSelf;
		internal bool ReactToDamage => _reactToDamage;
	};
};
