using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Runner Enemy", menuName = "Enemy Statistics/Runner", order = 2)]
	internal sealed class RunnerStatistics : MovingStatistics
	{
		[Header("Runner Enemy")]
		[SerializeField, Tooltip("If the off edge verifier will be turned off.")] private bool _turnOffEdge;
		[SerializeField, Tooltip("If the dash is timed to start when the boss is instantiate.")] private bool _timedDash;
		[SerializeField, Tooltip("If this enemy will run away from the target.")] private bool _runFromTarget;
		[SerializeField, Tooltip("If this enemy will run toward the target after the run.")] private bool _runTowardsAfter;
		[SerializeField, Tooltip("If the boss can jump while dashing.\nRequires: Jumper Enemy")] private bool _jumpDash;
		[SerializeField, Tooltip("The amount of time to wait the timed dash to go.")] private float _timeToDash;
		[SerializeField, Tooltip("The amount of speed to run in the retreat.")] private float _retreatSpeed;
		[SerializeField, Tooltip("The amount of time to wait for retreat again.")] private float _timeToRetreat;
		[SerializeField, Tooltip("The amount of distance to run in the retreat.")] private float _retreatDistance;
		[SerializeField, Tooltip("The amount of time this enemy will run away from or pursue the target.")] private float _runOfTime;
		[SerializeField, Tooltip("The amount of time this enemy will be dashing upon the target.")] private float _timeDashing;
		[SerializeField, Tooltip("The amount of times this enemy have to run away from the target.")] private ushort _timesToRun;
		internal bool TurnOffEdge => _turnOffEdge;
		internal bool TimedDash => _timedDash;
		internal bool RunFromTarget => _runFromTarget;
		internal bool RunTowardsAfter => _runTowardsAfter;
		internal bool JumpDash => _jumpDash;
		internal float TimeToDash => _timeToDash;
		internal float RetreatSpeed => _retreatSpeed;
		internal float TimeToRetreat => _timeToRetreat;
		internal float RetreatDistance => _retreatDistance;
		internal float RunOfTime => _runOfTime;
		internal float TimeDashing => _timeDashing;
		internal ushort TimesToRun => _timesToRun;
	};
};
