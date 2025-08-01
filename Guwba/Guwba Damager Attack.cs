using UnityEngine;
using UnityEngine.Events;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Collider2D))]
	internal sealed class GuwbaDamagerAttack : StateController
	{
		private SpriteRenderer _spriteRenderer;
		[Header("Stat")]
		[SerializeField, Tooltip("The amount of damage that the attack of Guwba hits.")] private ushort _attackDamage;
		internal UnityAction<GuwbaDamagerAttack, IDamageable> Attack { get; set; }
		internal ushort AttackDamage => this._attackDamage;
		internal float Alpha { get => this._spriteRenderer.color.a; set => this._spriteRenderer.color = new(1f, 1f, 1f, value); }
		private new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<IDamageable>(out var damageable))
				this.Attack.Invoke(this, damageable);
		}
	};
};
