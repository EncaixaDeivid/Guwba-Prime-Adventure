using UnityEngine;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class DeathlessSurface : StateController
	{
		[SerializeField] private ushort _damage;
		[SerializeField] private bool _everyone;
		private void OnCollision(GameObject collisionObject)
		{
			if (collisionObject.TryGetComponent<IDamageable>(out var damageable))
				if (this._everyone)
					damageable.Damage(this._damage);
				else if (GuwbaTransformer<VisualGuwba>.EqualObject(collisionObject))
					damageable.Damage(this._damage);
		}
		private void OnCollisionEnter2D(Collision2D other) => this.OnCollision(other.gameObject);
		private void OnCollisionStay2D(Collision2D other) => this.OnCollision(other.gameObject);
		private void OnTriggerEnter2D(Collider2D other) => this.OnCollision(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => this.OnCollision(other.gameObject);
	};
};
