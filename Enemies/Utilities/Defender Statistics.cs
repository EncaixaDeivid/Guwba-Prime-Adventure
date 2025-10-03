using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Defender Enemy", menuName = "Enemy Statistics/Defender", order = 8)]
	internal sealed class DefenderStatistics : ScriptableObject
	{
		[Header("Defender Enemy")]
		[SerializeField, Tooltip("The physics of the enemy.")] private EnemyPhysics _physics;
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private short _biggerDamage;
		[SerializeField, Tooltip("If this enemy will stop moving when become invencible.\nRequires: Moving Enemy")]
		private bool _invencibleStop;
		[SerializeField, Tooltip("If this enemy will become invencible when hurted.")] private bool _invencibleHurted;
		[SerializeField, Tooltip("If this enemy will use time to become invencible/destructible.")] private bool _useAlternatedTime;
		[SerializeField, Tooltip("The amount of time the enemy have to become destructible.")] private float _timeToDestructible;
		[SerializeField, Tooltip("The amount of time the enemy have to become invencible.")] private float _timeToInvencible;
		internal EnemyPhysics Physics => this._physics;
		internal short BiggerDamage => this._biggerDamage;
		internal bool InvencibleStop => this._invencibleStop;
		internal bool InvencibleHurted => this._invencibleHurted;
		internal bool UseAlternatedTime => this._useAlternatedTime;
		internal float TimeToDestructible => this._timeToDestructible;
		internal float TimeToInvencible => this._timeToInvencible;
	};
};