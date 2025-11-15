using UnityEngine;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	[CreateAssetMenu(fileName = "Defender Enemy", menuName = "Enemy Statistics/Defender", order = 8)]
	public sealed class DefenderStatistics : ScriptableObject
	{
		[Header("Defender Enemy")]
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private short _biggerDamage;
		[SerializeField, Tooltip("If this enemy will stop moving when become invencible.\nRequires: Moving Enemy")] private bool _invencibleStop;
		[SerializeField, Tooltip("If this enemy will become invencible when hurted.")] private bool _invencibleHurted;
		[SerializeField, Tooltip("If this enemy will use time to become invencible/destructible.")] private bool _useAlternatedTime;
		[SerializeField, Tooltip("The amount of time the enemy have to become destructible.")] private float _timeToDestructible;
		[SerializeField, Tooltip("The amount of time the enemy have to become invencible.")] private float _timeToInvencible;
		public short BiggerDamage => _biggerDamage;
		public bool InvencibleStop => _invencibleStop;
		public bool InvencibleHurted => _invencibleHurted;
		public bool UseAlternatedTime => _useAlternatedTime;
		public float TimeToDestructible => _timeToDestructible;
		public float TimeToInvencible => _timeToInvencible;
	};
};
