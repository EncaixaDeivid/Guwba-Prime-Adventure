using UnityEngine;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	[CreateAssetMenu(fileName = "Enemy Physics", menuName = "Enemy Statistics/Physics", order = 0)]
	public sealed class EnemyPhysics : ScriptableObject
	{
		[Header("Enemy Physics")]
		[SerializeField, Tooltip("The layer mask to identify the ground.")] private LayerMask _groundLayer;
		[SerializeField, Tooltip("The layer mask to identify the target of the attacks.")] private LayerMask _targetLayer;
		[SerializeField, Tooltip("Size of collider for checking the ground below the feet.")] private float _groundChecker;
		[SerializeField, Tooltip("The amount of time to stop the game when hit is given.")] private float _hitStopTime;
		[SerializeField, Tooltip("The amount of time to slow the game when hit is given.")] private float _hitSlowTime;
		public LayerMask GroundLayer => _groundLayer;
		public LayerMask TargetLayer => _targetLayer;
		public float GroundChecker => _groundChecker;
		public float HitStopTime => _hitStopTime;
		public float HitSlowTime => _hitSlowTime;
	};
};
