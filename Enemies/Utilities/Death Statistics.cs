using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Death Enemy", menuName = "Enemy Statistics/Death", order = 10)]
	internal sealed class DeathStatistics : ScriptableObject
	{
		[Header("Death Enemy")]
		[SerializeField, Tooltip("The physics of the enemy.")] private EnemyPhysics _physics;
		[SerializeField, Tooltip("The enemy that this enemy will spawn in death.")] private EnemyController _childEnemy;
		[SerializeField, Tooltip("The projectile that this enemy will spawn in death.")] private EnemyProjectile _childProjectile;
		[SerializeField, Tooltip("If this enemy will die on touch.")] private bool _onTouch;
		[SerializeField, Tooltip("THe time to this enemy die.")] private float _timeToDie;
		internal EnemyPhysics Physics => _physics;
		internal EnemyController ChildEnemy => _childEnemy;
		internal EnemyProjectile ChildProjectile => _childProjectile;
		internal bool OnTouch => _onTouch;
		internal float TimeToDie => _timeToDie;
	};
};
