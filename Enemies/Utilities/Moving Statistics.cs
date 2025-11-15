using UnityEngine;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	public abstract class MovingStatistics : ScriptableObject
	{
		[Header("Moving Statistics")]
		[SerializeField, Tooltip("The physics of the enemy.")] private EnemyPhysics _physics;
		[SerializeField, Tooltip("The speed of the enemy to moves towards.")] private float _movementSpeed;
		[SerializeField, Tooltip("The amount of speed of the dash.")] private float _dashSpeed;
		[SerializeField, Tooltip("The distance of the detection of target.")] private float _lookDistance;
		[SerializeField, Tooltip("If this enemy will do some action when look to a target.")] private bool _lookPerception;
		[SerializeField, Tooltip("If this enemy will stop on detection of the target.")] private bool _detectionStop;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		[SerializeField, Tooltip("The amount of time this enemy will stop on detection.")] private float _stopTime;
		public EnemyPhysics Physics => _physics;
		public float MovementSpeed => _movementSpeed;
		public float DashSpeed => _dashSpeed;
		public float LookDistance => _lookDistance;
		public bool LookPerception => _lookPerception;
		public bool DetectionStop => _detectionStop;
		public bool ReactToDamage => _reactToDamage;
		public float StopTime => _stopTime;
	};
};
