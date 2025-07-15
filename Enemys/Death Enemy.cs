using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	internal sealed class DeathEnemy : EnemyController
	{
		[Header("Death Enemy")]
		[SerializeField, Tooltip("The enemy that this enemy will spawn in death.")] private EnemyController _childEnemy;
		[SerializeField, Tooltip("The projectile that this enemy will spawn in death.")] private Projectile _childProjectile;
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (this._childEnemy)
				Instantiate(this._childEnemy, this.transform.position, this._childEnemy.transform.rotation);
			if (this._childProjectile)
				Instantiate(this._childEnemy, this.transform.position, this._childEnemy.transform.rotation);
		}
	};
};