using UnityEngine;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	[CreateAssetMenu(fileName = "Death Enemy", menuName = "Enemy Statistics/Death", order = 10)]
	public sealed class DeathStatistics : ScriptableObject
	{
		[Header("Death Enemy")]
		[SerializeField, Tooltip("The enemy that this enemy will spawn in death.")] private Control _childEnemy;
		[SerializeField, Tooltip("The projectile that this enemy will spawn in death.")] private Projectile _childProjectile;
		[SerializeField, Tooltip("If this enemy will die on touch.")] private bool _onTouch;
		[SerializeField, Tooltip("THe time to this enemy die.")] private float _timeToDie;
		public Control ChildEnemy => _childEnemy;
		public Projectile ChildProjectile => _childProjectile;
		public bool OnTouch => _onTouch;
		public float TimeToDie => _timeToDie;
	};
};
