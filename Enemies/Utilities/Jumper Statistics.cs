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
		[SerializeField, Tooltip("If the timmed jumps will be executed in a sequence.")] private bool _sequentialTimmedJumps;
		[SerializeField, Tooltip("If the sequential timmed jumps will be repeated again over again.")] private bool _repeatTimmedJumps;
		[SerializeField, Tooltip("The speed to moves on a high jump.")] private ushort _followSpeed;
		[SerializeField, Tooltip("If the react to damage will use other target.")] private bool _useTarget;
		[SerializeField, Tooltip("If the target to follow will be random.")] private bool _randomFollow;
		[SerializeField, Tooltip("The distance the boss will be to the follow target.")] private float _distanceToTarget;
		[Header("Reaction")]
		[SerializeField, Tooltip("The strenght of the jump on a react of damage.")] private ushort _strenghtReact;
		[SerializeField, Tooltip("If the react to damage jump is a high jump.")] private bool _highReact;
		[SerializeField, Tooltip("If it will stop moving on react to damage.")] private bool _stopMoveReact;
		internal JumpPointStructure[] JumpPointStructures => this._jumpPointStructures;
		internal JumpStats[] TimedJumps => this._timedJumps;
		internal Vector2 OtherTarget => this._otherTarget;
		internal bool SequentialTimmedJumps => this._sequentialTimmedJumps;
		internal bool RepeatTimmedJumps => this._repeatTimmedJumps;
		internal ushort FollowSpeed => this._followSpeed;
		internal bool UseTarget => this._useTarget;
		internal bool RandomFollow => this._randomFollow;
		internal float DistanceToTarget => this._distanceToTarget;
		internal ushort StrenghtReact => this._strenghtReact;
		internal bool HighReact => this._highReact;
		internal bool StopMoveReact => this._stopMoveReact;
	};
	[System.Serializable]
	internal struct JumpStats
	{
		[SerializeField, Tooltip("To where this have to go if theres no target.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("The strenght of the jump.")] private ushort _strength;
		[SerializeField, Tooltip("If in the high jumo it will stop moving.")] private bool _stopMove;
		[SerializeField, Tooltip("If this is a high jump.")] private bool _high;
		[SerializeField, Tooltip("If for this jump it will use the other target.")] private bool _useTarget;
		[SerializeField, Tooltip("The amount of time for this jump to execute.")] private float _timeToExecute;
		internal readonly Vector2 OtherTarget => this._otherTarget;
		internal readonly ushort Strength => this._strength;
		internal readonly bool StopMove => this._stopMove;
		internal readonly bool High => this._high;
		internal readonly bool UseTarget => this._useTarget;
		internal readonly float TimeToExecute => this._timeToExecute;
	};
	[System.Serializable]
	internal struct JumpPointStructure
	{
		[SerializeField, Tooltip("The object to activate the jump.")] private JumpPoint _jumpPointObject;
		[SerializeField, Tooltip("The jump stats to use in this structure.")] private JumpStats _jumpStats;
		[SerializeField, Tooltip("Where the jump point will be.")] private Vector2 _point;
		[SerializeField, Tooltip("The amount of times the boss have to pass by to activate the jump.")] private Vector2Int _jumpCountMaxMin;
		internal readonly JumpPoint JumpPointObject => this._jumpPointObject;
		internal readonly JumpStats JumpStats => this._jumpStats;
		internal readonly Vector2 Point => this._point;
		internal readonly ushort JumpCount => (ushort)Random.Range(this._jumpCountMaxMin.x, this._jumpCountMaxMin.y);
		internal short RemovalJumpCount { get; set; }
	};
};