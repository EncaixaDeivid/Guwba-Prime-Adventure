using UnityEngine;
namespace GwambaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Enemy Summon", menuName = "Enemy Statistics/Summon", order = 7)]
	internal sealed class SummonObject : ScriptableObject
	{
		[Header("Components Statistics")]
		[SerializeField, Tooltip("The enemy that will be instantiate.")] private GameObject[] _summons;
		[SerializeField, Tooltip("The points that the instance can be instantiate.")] private Vector2[] _summonPoints;
		[SerializeField, Tooltip("The amount of time to execute the instance.")] private ushort _summonTime;
		[SerializeField, Tooltip("The amount of time to wait to execute the next instance.")] private ushort _postSummonTime;
		[SerializeField, Tooltip("The amount of instance to be instantiate.")] private ushort _quantityToSummon;
		[SerializeField, Tooltip("The amount of time to stop the instantiator.")] private float _timeToStop;
		[SerializeField, Tooltip("If the points to summon are relative to it's parent.")] private bool _localPoints;
		[SerializeField, Tooltip("If the instantiator will stop during the summon.")] private bool _stopToSummon;
		[SerializeField, Tooltip("If the instantiator will paralyze during the summon.")] private bool _paralyzeToSummon;
		[SerializeField, Tooltip("If the timed summon will stop permanently.")] private bool _stopPermanently;
		[SerializeField, Tooltip("If the instanciation will be in the same point as the summoner.")] private bool _self;
		[SerializeField, Tooltip("If the instantiation will be randomized at one of the points.")] private bool _random;
		internal GameObject[] Summons => _summons;
		internal Vector2[] SummonPoints => _summonPoints;
		internal ushort SummonTime => _summonTime;
		internal ushort PostSummonTime => _postSummonTime;
		internal ushort QuantityToSummon => _quantityToSummon;
		internal float TimeToStop => _timeToStop;
		internal bool LocalPoints => _localPoints;
		internal bool StopToSummon => _stopToSummon;
		internal bool ParalyzeToSummon => _paralyzeToSummon;
		internal bool StopPermanently => _stopPermanently;
		internal bool Self => _self;
		internal bool Random => _random;
	};
};
