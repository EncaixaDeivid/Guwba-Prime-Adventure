using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Provider Enemy", menuName = "Enemy Statistics/Provider", order = 1)]
	internal sealed class EnemyStatistics : ScriptableObject
	{
		[Header("Providence Statistics")]
		[SerializeField, Tooltip("The physics of the enemy.")] private EnemyPhysics _physics;
		[SerializeField, Tooltip("The vitality of the enemy.")] private ushort _vitality;
		[SerializeField, Tooltip("The amount of stun that this enemy can resists.")] private ushort _hitResistance;
		[SerializeField, Tooltip("The amount of damage that the enemy hit.")] private ushort _damage;
		[SerializeField, Tooltip("If this enemy receives no type of damage.")] private bool _noDamage;
		[SerializeField, Tooltip("If this enemy won't do damage at contact.")] private bool _noHit;
		[SerializeField, Tooltip("If this enemy will fade away over time.")] private bool _fadeOverTime;
		[SerializeField, Tooltip("The amount of time this enemy will fade away.")] private float _timeToFadeAway;
		[SerializeField, Tooltip("The amount of time this enemy will stun.")] private float _stunTime;
		[SerializeField, Tooltip("The amount of time this enemy will be stunned when armor be broken.")] private float _stunnedTime;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		[SerializeField, Tooltip("If this enemy has a index atribute to use.")] private bool _hasIndex;
		[SerializeField, Tooltip("The index to a event to a enemy make.")] private ushort _indexEvent;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		internal EnemyPhysics Physics => _physics;
		internal ushort Vitality => _vitality;
		internal ushort HitResistance => _hitResistance;
		internal ushort Damage => _damage;
		internal bool NoDamage => _noDamage;
		internal bool NoHit => _noHit;
		internal bool FadeOverTime => _fadeOverTime;
		internal float TimeToFadeAway => _timeToFadeAway;
		internal float StunTime => _stunTime;
		internal float StunnedTime => _stunnedTime;
		internal bool ReactToDamage => _reactToDamage;
		internal bool HasIndex => _hasIndex;
		internal ushort IndexEvent => _indexEvent;
		internal bool SaveOnSpecifics => _saveOnSpecifics;
	};
};
