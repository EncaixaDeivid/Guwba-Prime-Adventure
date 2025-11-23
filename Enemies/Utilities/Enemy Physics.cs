using UnityEngine;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	[CreateAssetMenu(fileName = "Enemy Physics", menuName = "Enemy Statistics/Physics", order = 0)]
	public sealed class EnemyPhysics : ScriptableObject
	{
		[field: SerializeField, Tooltip("The layer mask to identify the ground."), Header("Enemy Physics", order = 0), Space(WorldBuild.FIELD_SPACE_LENGTH * 2f)]
		public LayerMask GroundLayer { get; private set; }
		[field: SerializeField, Tooltip("The layer mask to identify the ground.")] public LayerMask TargetLayer { get; private set; }
		[field: SerializeField, Tooltip("The layer mask to identify the ground.")] public float HitStopTime { get; private set; }
		[field: SerializeField, Tooltip("The layer mask to identify the ground.")] public float HitSlowTime { get; private set; }
	};
};
