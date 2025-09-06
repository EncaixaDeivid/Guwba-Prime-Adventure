using UnityEngine;
using UnityEngine.Events;
using System;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Collider2D))]
	internal sealed class DamageableGuwba : StateController, IDestructible
	{
		private SpriteRenderer _spriteRenderer;
		private Predicate<ushort> _damageableHurt;
		private UnityAction<ushort, float> _damageableStun;
		private UnityAction<DamageableGuwba, IDestructible> _damageableAttack;
		[Header("Stats")]
		[SerializeField, Tooltip("If this Guwba's part will take damage.")] private bool _takeDamage;
		[SerializeField, Tooltip("The amount of damage that the attack of Guwba hits.")] private ushort _attackDamage;
		[SerializeField, Tooltip("The amount of time that this Guwba's attack stun does.")] private float _stunTime;
		internal Predicate<ushort> DamageableHurt { get => null; set => this._damageableHurt = value; }
		internal UnityAction<ushort, float> DamageableStun { get => null; set => this._damageableStun = value; }
		internal UnityAction<DamageableGuwba, IDestructible> DamageableAttack { get => null; set => this._damageableAttack = value; }
		internal ushort AttackDamage => this._attackDamage;
		internal float StunTime => this._stunTime;
		internal float Alpha { get => this._spriteRenderer.color.a; set => this._spriteRenderer.color = new(1f, 1f, 1f, value); }
		public short Health => 0;
		private new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!this._takeDamage && other.TryGetComponent<IDestructible>(out var destructible))
				this._damageableAttack.Invoke(this, destructible);
		}
		public bool Hurt(ushort damage) => this._damageableHurt.Invoke(damage);
		public void Stun(ushort stunStength, float stunTime) => this._damageableStun.Invoke(stunStength, stunTime);
	};
};
