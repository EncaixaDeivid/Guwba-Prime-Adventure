using UnityEngine;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy
{
	internal sealed class DeathEnemy : EnemyController
	{
		[Header("Death Enemy")]
		[SerializeField, Tooltip("The enemy that this enemy will spawn in death.")] private EnemyController _childEnemy;
		[SerializeField, Tooltip("The projectile that this enemy will spawn in death.")] private Projectile _childProjectile;
		[SerializeField, Tooltip("If this enemy will die on touch.")] private bool _onTouch;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!this.DoNotWork && this._onTouch && CentralizableGuwba.EqualObject(other.gameObject))
				Destroy(this.gameObject);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (this.DoNotWork)
				return;
			if (this._childEnemy)
				Instantiate(this._childEnemy, this.transform.position, Quaternion.identity);
			if (this._childProjectile)
				Instantiate(this._childProjectile, this.transform.position, Quaternion.identity);
		}
	};
};
