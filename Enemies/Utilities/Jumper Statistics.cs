using UnityEngine;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	[CreateAssetMenu(fileName = "Jumper Enemy", menuName = "Enemy Statistics/Jumper", order = 3)]
	public sealed class JumperStatistics : MovingStatistics
	{
		[Header("Jumper Enemy")]
		[SerializeField, Tooltip("The collection of the objet that carry the jump")] private JumpPointStructure[] _jumpPointStructures;
		[SerializeField, Tooltip("The collection of the jumps timed for this enemy.")] private JumpStats[] _timedJumps;
		[SerializeField, Tooltip("The other target to move to on jump.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("If this enemy will use input.")] private bool _useInput;
		[SerializeField, Tooltip("If the detection will be circular.")] private bool _circularDetection;
		[SerializeField, Tooltip("If this enemy will not follow the target in the basic jump.")] private bool _unFollow;
		[SerializeField, Tooltip("If the timmed jumps will be executed in a sequence.")] private bool _sequentialTimmedJumps;
		[SerializeField, Tooltip("If the sequential timmed jumps will be repeated again over again.")] private bool _repeatTimmedJumps;
		[SerializeField, Tooltip("If the react to damage will use other target.")] private bool _useTarget;
		[SerializeField, Tooltip("If the target to follow will be random.")] private bool _randomFollow;
		[SerializeField, Tooltip("The strenght of the basic jump.")] private float _jumpStrenght;
		[SerializeField, Tooltip("The amount of time to jump again.")] private float _timeToJump;
		[SerializeField, Tooltip("The angle of the detection ray.")] private float _detectionAngle;
		[SerializeField, Tooltip("The distance this enemy will be to the follow target.")] private float _distanceToTarget;
		[Header("Reaction")]
		[SerializeField, Tooltip("The strenght of the jump on a react of damage.")] private ushort _strenghtReact;
		[SerializeField, Tooltip("If the react to damage jump will follow a target.")] private bool _followReact;
		[SerializeField, Tooltip("If the react to damage jump will turn on the target.")] private bool _turnFollowReact;
		[SerializeField, Tooltip("If it will stop moving on react to damage.")] private bool _stopMoveReact;
		public JumpPointStructure[] JumpPointStructures => _jumpPointStructures;
		public JumpStats[] TimedJumps => _timedJumps;
		public Vector2 OtherTarget => _otherTarget;
		public bool UseInput => _useInput;
		public bool CircularDetection => _circularDetection;
		public bool UnFollow => _unFollow;
		public bool SequentialTimmedJumps => _sequentialTimmedJumps;
		public bool RepeatTimmedJumps => _repeatTimmedJumps;
		public bool UseTarget => _useTarget;
		public bool RandomFollow => _randomFollow;
		public float JumpStrenght => _jumpStrenght;
		public float TimeToJump => _timeToJump;
		public float DetectionAngle => _detectionAngle;
		public float DistanceToTarget => _distanceToTarget;
		public ushort StrenghtReact => _strenghtReact;
		public bool FollowReact => _followReact;
		public bool TurnFollowReact => _turnFollowReact;
		public bool StopMoveReact => _stopMoveReact;
	};
	[System.Serializable]
	public struct JumpPointStructure
	{
		[SerializeField, Tooltip("The object to activate the jump.")] private JumpPoint _jumpPointObject;
		[SerializeField, Tooltip("The jump stats to use in this structure.")] private JumpStats _jumpStats;
		[SerializeField, Tooltip("Where the jump point will be.")] private Vector2 _point;
		[SerializeField, Tooltip("The minimal amount of times the boss have to pass by to activate the jump.")] private ushort _minJumpCount;
		[SerializeField, Tooltip("The maximum amount of times the boss have to pass by to activate the jump.")] private ushort _maxJumpCount;
		public readonly JumpPoint JumpPointObject => _jumpPointObject;
		public readonly JumpStats JumpStats => _jumpStats;
		public readonly Vector2 Point => _point;
		public readonly ushort JumpCount => (ushort)Random.Range(_minJumpCount, _maxJumpCount + 1);
	};
	[System.Serializable]
	public struct JumpStats
	{
		[SerializeField, Tooltip("To where this have to go if theres no target.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("The strenght of the jump.")] private ushort _strength;
		[SerializeField, Tooltip("If in the high jumo it will stop moving.")] private bool _stopMove;
		[SerializeField, Tooltip("If this is a follow jump.")] private bool _follow;
		[SerializeField, Tooltip("If this enemy will turn on the follow.")] private bool _turnFollow;
		[SerializeField, Tooltip("If for this jump it will use the other target.")] private bool _useTarget;
		[SerializeField, Tooltip("The amount of time for this jump to execute.")] private float _timeToExecute;
		public readonly Vector2 OtherTarget => _otherTarget;
		public readonly ushort Strength => _strength;
		public readonly bool StopMove => _stopMove;
		public readonly bool Follow => _follow;
		public readonly bool TurnFollow => _turnFollow;
		public readonly bool UseTarget => _useTarget;
		public readonly float TimeToExecute => _timeToExecute;
	};
};
