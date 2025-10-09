using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Enemy Control", menuName = "Enemy Statistics/Control", order = 1)]
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
		internal EnemyPhysics Physics => this._physics;
		internal ushort Vitality => this._vitality;
		internal ushort HitResistance => this._hitResistance;
		internal ushort Damage => this._damage;
		internal bool NoDamage => this._noDamage;
		internal bool NoHit => this._noHit;
		internal bool FadeOverTime => this._fadeOverTime;
		internal float TimeToFadeAway => this._timeToFadeAway;
		internal float StunTime => this._stunTime;
		internal float StunnedTime => this._stunnedTime;
		internal bool ReactToDamage => this._reactToDamage;
		internal bool HasIndex => this._hasIndex;
		internal ushort IndexEvent => this._indexEvent;
		internal bool SaveOnSpecifics => this._saveOnSpecifics;
	};
};
