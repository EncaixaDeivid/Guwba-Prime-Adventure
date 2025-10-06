using UnityEngine;
using UnityEngine.Events;
using System;
namespace GuwbaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Collider2D))]
	internal sealed class GuwbaDamager : StateController, IDestructible
	{
		private SpriteRenderer _spriteRenderer;
		private Predicate<ushort> _damagerHurt;
		private UnityAction<ushort, float> _damagerStun;
		private UnityAction<GuwbaDamager, IDestructible> _damagerAttack;
		[Header("Stats")]
		[SerializeField, Tooltip("If this Guwba's part will take damage.")] private bool _takeDamage;
		[SerializeField, Tooltip("The amount of damage that the attack of Guwba hits.")] private ushort _attackDamage;
		[SerializeField, Tooltip("The amount of time that this Guwba's attack stun does.")] private float _stunTime;
		internal Predicate<ushort> DamagerHurt { get => null; set => this._damagerHurt = value; }
		internal UnityAction<ushort, float> DamagerStun { get => null; set => this._damagerStun = value; }
		internal UnityAction<GuwbaDamager, IDestructible> DamagerAttack { get => null; set => this._damagerAttack = value; }
		internal ushort AttackDamage => this._attackDamage;
		internal float StunTime => this._stunTime;
		internal float Alpha { get => this._spriteRenderer.color.a; set => this._spriteRenderer.color = new(1f, 1f, 1f, value); }
		public short Health => 0;
		private new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
		}
		public bool Hurt(ushort damage)
		{
			if (this._takeDamage)
				return this._damagerHurt.Invoke(damage);
			return false;
		}
		public void Stun(ushort stunStength, float stunTime)
		{
			if (this._takeDamage)
				this._damagerStun.Invoke(stunStength, stunTime);
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!this._takeDamage && other.TryGetComponent<IDestructible>(out var destructible))
				this._damagerAttack.Invoke(this, destructible);
		}
	};
};
