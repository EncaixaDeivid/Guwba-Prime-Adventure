using UnityEngine;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	[CreateAssetMenu(fileName = "Provider Enemy", menuName = "Enemy Statistics/Provider", order = 1)]
	public sealed class EnemyStatistics : ScriptableObject
	{
		[Header("Providence Statistics")]
		[SerializeField, Tooltip("The physics of the enemy.")] private EnemyPhysics _physics;
		[SerializeField, Tooltip("The velocity of the screen shake on the hurt.")] private Vector2 _hurtShake;
		[SerializeField, Tooltip("The vitality of the enemy.")] private ushort _vitality;
		[SerializeField, Tooltip("The amount of stun that this enemy can resists.")] private ushort _hitResistance;
		[SerializeField, Tooltip("The amount of damage that the enemy hit.")] private ushort _damage;
		[SerializeField, Tooltip("If this enemy receives no type of damage.")] private bool _noDamage;
		[SerializeField, Tooltip("If this enemy won't do damage at contact.")] private bool _noHit;
		[SerializeField, Tooltip("If this enemy won't get stunned.")] private bool _noHitStun;
		[SerializeField, Tooltip("If this enemy won't get stunned.")] private bool _noStun;
		[SerializeField, Tooltip("If this enemy will fade away over time.")] private bool _fadeOverTime;
		[SerializeField, Tooltip("The amount of time this enemy will fade away.")] private float _timeToFadeAway;
		[SerializeField, Tooltip("The amount of time this enemy will stun.")] private float _stunTime;
		[SerializeField, Tooltip("The amount of time this enemy will be stunned when armor be broken.")] private float _stunnedTime;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		[SerializeField, Tooltip("If this enemy has a index atribute to use.")] private bool _hasIndex;
		[SerializeField, Tooltip("The index to a event to a enemy make.")] private ushort _indexEvent;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		public EnemyPhysics Physics => _physics;
		public Vector2 HurtShake => _hurtShake;
		public ushort Vitality => _vitality;
		public ushort HitResistance => _hitResistance;
		public ushort Damage => _damage;
		public bool NoDamage => _noDamage;
		public bool NoHit => _noHit;
		public bool NoHitStun => _noHitStun;
		public bool NoStun => _noStun;
		public bool FadeOverTime => _fadeOverTime;
		public float TimeToFadeAway => _timeToFadeAway;
		public float StunTime => _stunTime;
		public float StunnedTime => _stunnedTime;
		public bool ReactToDamage => _reactToDamage;
		public bool HasIndex => _hasIndex;
		public ushort IndexEvent => _indexEvent;
		public bool SaveOnSpecifics => _saveOnSpecifics;
	};
};
