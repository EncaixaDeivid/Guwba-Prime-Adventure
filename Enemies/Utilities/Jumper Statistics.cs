using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Jumper Enemy", menuName = "Enemy Statistics/Jumper", order = 3)]
	internal sealed class JumperStatistics : MovingStatistics
	{
		[Header("Jumper Enemy")]
		[SerializeField, Tooltip("The collection of the objet that carry the jump")] private JumpPointStructure[] _jumpPointStructures;
		[SerializeField, Tooltip("The collection of the jumps timed for this boss.")] private JumpStats[] _timedJumps;
		[SerializeField, Tooltip("The other target to move to on jump.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("If the detection will be circular.")] private bool _circularDetection;
		[SerializeField, Tooltip("If the timmed jumps will be executed in a sequence.")] private bool _sequentialTimmedJumps;
		[SerializeField, Tooltip("If the sequential timmed jumps will be repeated again over again.")] private bool _repeatTimmedJumps;
		[SerializeField, Tooltip("If the react to damage will use other target.")] private bool _useTarget;
		[SerializeField, Tooltip("If the target to follow will be random.")] private bool _randomFollow;
		[SerializeField, Tooltip("The strenght of the basic jump.")] private float _jumpStrenght;
		[SerializeField, Tooltip("The angle of the detection ray.")] private float _detectionAngle;
		[SerializeField, Tooltip("The distance this enemy will be to the follow target.")] private float _distanceToTarget;
		[Header("Reaction")]
		[SerializeField, Tooltip("The strenght of the jump on a react of damage.")] private ushort _strenghtReact;
		[SerializeField, Tooltip("If the react to damage jump is a high jump.")] private bool _followReact;
		[SerializeField, Tooltip("If it will stop moving on react to damage.")] private bool _stopMoveReact;
		internal JumpPointStructure[] JumpPointStructures => _jumpPointStructures;
		internal JumpStats[] TimedJumps => _timedJumps;
		internal Vector2 OtherTarget => _otherTarget;
		internal bool CircularDetection => _circularDetection;
		internal bool SequentialTimmedJumps => _sequentialTimmedJumps;
		internal bool RepeatTimmedJumps => _repeatTimmedJumps;
		internal bool UseTarget => _useTarget;
		internal bool RandomFollow => _randomFollow;
		internal float JumpStrenght => _jumpStrenght;
		internal float DetectionAngle => _detectionAngle;
		internal float DistanceToTarget => _distanceToTarget;
		internal ushort StrenghtReact => _strenghtReact;
		internal bool FollowReact => _followReact;
		internal bool StopMoveReact => _stopMoveReact;
	};
	[System.Serializable]
	internal struct JumpStats
	{
		[SerializeField, Tooltip("To where this have to go if theres no target.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("The strenght of the jump.")] private ushort _strength;
		[SerializeField, Tooltip("If in the high jumo it will stop moving.")] private bool _stopMove;
		[SerializeField, Tooltip("If this is a follow jump.")] private bool _follow;
		[SerializeField, Tooltip("If for this jump it will use the other target.")] private bool _useTarget;
		[SerializeField, Tooltip("The amount of time for this jump to execute.")] private float _timeToExecute;
		internal readonly Vector2 OtherTarget => _otherTarget;
		internal readonly ushort Strength => _strength;
		internal readonly bool StopMove => _stopMove;
		internal readonly bool Follow => _follow;
		internal readonly bool UseTarget => _useTarget;
		internal readonly float TimeToExecute => _timeToExecute;
	};
	[System.Serializable]
	internal struct JumpPointStructure
	{
		[SerializeField, Tooltip("The object to activate the jump.")] private JumpPoint _jumpPointObject;
		[SerializeField, Tooltip("The jump stats to use in this structure.")] private JumpStats _jumpStats;
		[SerializeField, Tooltip("Where the jump point will be.")] private Vector2 _point;
		[SerializeField, Tooltip("The amount of times the boss have to pass by to activate the jump.")] private Vector2Int _jumpCountMaxMin;
		internal readonly JumpPoint JumpPointObject => _jumpPointObject;
		internal readonly JumpStats JumpStats => _jumpStats;
		internal readonly Vector2 Point => _point;
		internal readonly ushort JumpCount => (ushort)Random.Range(_jumpCountMaxMin.x, _jumpCountMaxMin.y);
		internal short RemovalJumpCount { get; set; }
	};
};
