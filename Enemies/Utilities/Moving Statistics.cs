using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	internal abstract class MovingStatistics : ScriptableObject
	{
		[Header("Moving Statistics")]
		[SerializeField, Tooltip("The physics of the enemy.")] private EnemyPhysics _physics;
		[SerializeField, Tooltip("The speed of the enemy to moves.")] private ushort _movementSpeed;
		[SerializeField, Tooltip("The amount of speed of the dash.")] private ushort _dashSpeed;
		[SerializeField, Tooltip("The distance of the detection of target.")] private ushort _lookDistance;
		[SerializeField, Tooltip("If this enemy will do some action when look to a target.")] private bool _lookPerception;
		[SerializeField, Tooltip("If this enemy will stop on detection of the target.")] private bool _detectionStop;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		[SerializeField, Tooltip("The amount of time this enemy will stop on detection.")] private float _stopTime;
		internal EnemyPhysics Physics => _physics;
		internal ushort MovementSpeed => _movementSpeed;
		internal ushort DashSpeed => _dashSpeed;
		internal ushort LookDistance => _lookDistance;
		internal bool LookPerception => _lookPerception;
		internal bool DetectionStop => _detectionStop;
		internal bool ReactToDamage => _reactToDamage;
		internal float StopTime => _stopTime;
	};
};
