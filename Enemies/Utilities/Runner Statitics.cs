using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Runner Enemy", menuName = "Enemy Statistics/Runner", order = 2)]
	internal sealed class RunnerStatistics : MovingStatistics
	{
		[Header("Runner Enemy")]
		[SerializeField, Tooltip("In the react to damage it already have a target.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("If the dash is timed to start when the boss is instantiate.")] private bool _timedDash;
		[SerializeField, Tooltip("If this enemy will run away from the target.")] private bool _runFromTarget;
		[SerializeField, Tooltip("If this enemy will run toward the target after the run.")] private bool _runTowardsAfter;
		[SerializeField, Tooltip("The amount of time to wait the timed dash to go.")] private float _timeToDash;
		[SerializeField, Tooltip("The amount of times this enemy have to run away from the target.")] private ushort _timesToRun;
		[SerializeField, Tooltip("The amount of time this enemy will run away from or pursue the target.")] private float _runOfTime;
		[SerializeField, Tooltip("The amount of time this enemy will be dashing upon the target.")] private float _timeDashing;
		[SerializeField, Tooltip("If the boss can jump while dashing.\nRequires: Jumper Enemy")] private bool _jumpDash;
		[SerializeField, Tooltip("If the boss will target other object when react to damage.")] private bool _useOtherTarget;
		internal Vector2 OtherTarget => this._otherTarget;
		internal bool TimedDash => this._timedDash;
		internal bool RunFromTarget => this._runFromTarget;
		internal bool RunTowardsAfter => this._runTowardsAfter;
		internal float TimeToDash => this._timeToDash;
		internal ushort TimesToRun => this._timesToRun;
		internal float RunOfTime => this._runOfTime;
		internal float TimeDashing => this._timeDashing;
		internal bool JumpDash => this._jumpDash;
		internal bool UseOtherTarget => this._useOtherTarget;
	};
};
