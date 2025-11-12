using UnityEngine;
namespace GwambaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Enemy Physics", menuName = "Enemy Statistics/Physics", order = 0)]
	internal sealed class EnemyPhysics : ScriptableObject
	{
		[Header("Enemy Physics")]
		[SerializeField, Tooltip("The layer mask to identify the ground.")] private LayerMask _groundLayer;
		[SerializeField, Tooltip("The layer mask to identify the target of the attacks.")] private LayerMask _targetLayer;
		[SerializeField, Tooltip("Size of collider for checking the ground below the feet.")] private float _groundChecker;
		[SerializeField, Tooltip("The amount of time to stop the game when hit is given.")] private float _hitStopTime;
		[SerializeField, Tooltip("The amount of time to slow the game when hit is given.")] private float _hitSlowTime;
		internal LayerMask GroundLayer => _groundLayer;
		internal LayerMask TargetLayer => _targetLayer;
		internal float GroundChecker => _groundChecker;
		internal float HitStopTime => _hitStopTime;
		internal float HitSlowTime => _hitSlowTime;
	};
};
