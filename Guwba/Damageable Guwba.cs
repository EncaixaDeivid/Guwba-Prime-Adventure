using UnityEngine;
using UnityEngine.Events;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Collider2D))]
	internal sealed class DamageableGuwba : StateController
	{
		private SpriteRenderer _spriteRenderer;
		private UnityAction<DamageableGuwba, IDestructible> _attack;
		[Header("Stats")]
		[SerializeField, Tooltip("The amount of damage that the attack of Guwba hits.")] private ushort _attackDamage;
		[SerializeField, Tooltip("The amount of time that this Guwba's attack stun does.")] private float _stunTime;
		internal void SetAttack(UnityAction<DamageableGuwba, IDestructible> attack) => this._attack += attack;
		internal void UnsetAttack(UnityAction<DamageableGuwba, IDestructible> attack) => this._attack -= attack;
		internal ushort AttackDamage => this._attackDamage;
		internal float StunTime => this._stunTime;
		internal float Alpha { get => this._spriteRenderer.color.a; set => this._spriteRenderer.color = new(1f, 1f, 1f, value); }
		private new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<IDestructible>(out var damageable))
				this._attack.Invoke(this, damageable);
		}
	};
};
