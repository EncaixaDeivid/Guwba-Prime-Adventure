using UnityEngine;
using UnityEngine.Events;
using System;
using NaughtyAttributes;
namespace GwambaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Collider2D))]
	internal sealed class GwambaDamager : StateController, IDestructible
	{
		private SpriteRenderer _spriteRenderer;
		private Predicate<ushort> _damagerHurt;
		private UnityAction<ushort, float> _damagerStun;
		private UnityAction<GwambaDamager, IDestructible> _damagerAttack;
		[Space(WorldBuild.FIELD_SPACE_LENGTH * 2f)]
		[SerializeField, BoxGroup("Stats"), HideIf(nameof(_takeDamage)), Tooltip("The velocity of the screen shake on the attack.")] private Vector2 _attackShake;
		[SerializeField, BoxGroup("Stats"), HideIf(nameof(_takeDamage)), Tooltip("The amount of damage that the attack of Gwamba hits.")] private ushort _attackDamage;
		[SerializeField, BoxGroup("Stats"), HideIf(nameof(_takeDamage)), Min(0f), Tooltip("The amount of time the attack screen shake will be applied.")] private float _attackShakeTime;
		[SerializeField, BoxGroup("Stats"), HideIf(nameof(_takeDamage)), Min(0f), Tooltip("The amount of time that this Gwamba's attack stun does.")] private float _stunTime;
		[SerializeField, BoxGroup("Stats"), Tooltip("If this Gwamba's part will take damage.")] private bool _takeDamage;
		internal Predicate<ushort> DamagerHurt { get => null; set => _damagerHurt = value; }
		internal UnityAction<ushort, float> DamagerStun { get => null; set => _damagerStun = value; }
		internal UnityAction<GwambaDamager, IDestructible> DamagerAttack { get => null; set => _damagerAttack = value; }
		internal Vector2 AttackShake => _attackShake;
		internal ushort AttackDamage => _attackDamage;
		internal float AttackShakeTime => _attackShakeTime;
		internal float StunTime => _stunTime;
		internal float Alpha { get => _spriteRenderer.color.a; set => _spriteRenderer.color = new(1f, 1f, 1f, value); }
		public short Health => 0;
		private new void Awake()
		{
			base.Awake();
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}
		public bool Hurt(ushort damage)
		{
			if (_takeDamage)
				return _damagerHurt.Invoke(damage);
			return false;
		}
		public void Stun(ushort stunStength, float stunTime)
		{
			if (_takeDamage)
				_damagerStun.Invoke(stunStength, stunTime);
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!_takeDamage && other.TryGetComponent<IDestructible>(out var destructible))
				_damagerAttack.Invoke(this, destructible);
		}
	};
};
