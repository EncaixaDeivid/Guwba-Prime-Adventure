using UnityEngine;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class HarmfulSurface : StateController
	{
		[Header("Interactions")]
		[SerializeField, Tooltip("The damage the surface hits.")] private ushort _damage;
		[SerializeField, Tooltip("If anything can be damaged")] private bool _everyone;
		private void OnCollision(GameObject collisionObject)
		{
			if (collisionObject.TryGetComponent<IDestructible>(out var damageable))
				if (this._everyone)
					damageable.Hurt(this._damage);
				else if (CentralizableGuwba.EqualObject(collisionObject))
					damageable.Hurt(this._damage);
		}
		private void OnCollisionEnter2D(Collision2D other) => this.OnCollision(other.gameObject);
		private void OnCollisionStay2D(Collision2D other) => this.OnCollision(other.gameObject);
		private void OnTriggerEnter2D(Collider2D other) => this.OnCollision(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => this.OnCollision(other.gameObject);
	};
};
