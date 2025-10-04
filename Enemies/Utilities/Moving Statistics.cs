using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	internal abstract class MovingStatistics : ScriptableObject
	{
		[Header("Moving Statistics")]
		[SerializeField, Tooltip("The physics of the enemy.")] private EnemyPhysics _physics;
		[SerializeField, Tooltip("The speed of the enemy to moves.")] private ushort _movementSpeed;
		[SerializeField, Tooltip("The amount of speed of the dash.")] private ushort _dashSpeed;
		[SerializeField, Tooltip("If this enemy will stop on detection of the target.")] private bool _detectionStop;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		[SerializeField, Tooltip("The amount of time this enemy will stop on detection.")] private float _stopTime;
		internal EnemyPhysics Physics => this._physics;
		internal ushort MovementSpeed => this._movementSpeed;
		internal ushort DashSpeed => this._dashSpeed;
		internal bool DetectionStop => this._detectionStop;
		internal bool ReactToDamage => this._reactToDamage;
		internal float StopTime => this._stopTime;
	};
};
