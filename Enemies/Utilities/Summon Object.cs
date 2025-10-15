using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Enemy Summon", menuName = "Enemy Statistics/Summon", order = 7)]
	public sealed class SummonObject : ScriptableObject
	{
		[Header("Components Statistics")]
		[SerializeField, Tooltip("The enemy that will be instantiate.")] private GameObject[] _summons;
		[SerializeField, Tooltip("The points that the instance can be instantiate.")] private Vector2[] _summonPoints;
		[SerializeField, Tooltip("The amount of time to execute the instance.")] private ushort _summonTime;
		[SerializeField, Tooltip("The amount of time to wait to execute the next instance.")] private ushort _postSummonTime;
		[SerializeField, Tooltip("The amount of instance to be instantiate.")] private ushort _quantityToSummon;
		[SerializeField, Tooltip("The amount of time to stop the instantiator.")] private float _timeToStop;
		[SerializeField, Tooltip("If the instantiator will stop during the summon.")] private bool _stopToSummon;
		[SerializeField, Tooltip("If the instantiator will paralyze during the summon.")] private bool _paralyzeToSummon;
		[SerializeField, Tooltip("If the timed summon can be stopped.")] private bool _stopTimedSummon;
		[SerializeField, Tooltip("If the timed summon will stop permanently.")] private bool _stopPermanently;
		[SerializeField, Tooltip("If the instanciation will be in the same point as the summoner.")] private bool _self;
		[SerializeField, Tooltip("If the instantiation will be randomized at one of the points.")] private bool _random;
		public GameObject[] Summons => _summons;
		public Vector2[] SummonPoints => _summonPoints;
		public ushort SummonTime => _summonTime;
		public ushort PostSummonTime => _postSummonTime;
		public ushort QuantityToSummon => _quantityToSummon;
		public float TimeToStop => _timeToStop;
		public bool StopToSummon => _stopToSummon;
		public bool ParalyzeToSummon => _paralyzeToSummon;
		public bool StopTimedSummon => _stopTimedSummon;
		public bool StopPermanently => _stopPermanently;
		public bool Self => _self;
		public bool Random => _random;
	};
};
