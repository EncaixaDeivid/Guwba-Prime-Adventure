using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Summon", menuName = "Scriptable Objects/Summon", order = 1)]
	public sealed class SummonObject : ScriptableObject
	{
		[Header("Components Stats")]
		[SerializeField, Tooltip("The enemy that will be instantiate.")] private EnemyController _enemySummon;
		[SerializeField, Tooltip("The projectile that will be instantiate.")] private Projectile _projectileSummon;
		[SerializeField, Tooltip("The points that the instance can be instantiate.")] private Vector2[] _summonPoints;
		[SerializeField, Tooltip("The amount of time to execute the instance.")] private ushort _summonTime;
		[SerializeField, Tooltip("The amount of instance to be instantiate.")] private ushort _quantityToSummon;
		[SerializeField, Tooltip("The amount of time to stop the instantiator.")] private float _timeToStop;
		[SerializeField, Tooltip("If the instantiator will stop during the summon.")] private bool _stopToSummon;
		[SerializeField, Tooltip("If the instantiator will paralyze during the summon.")] private bool _paralyzeToSummon;
		[SerializeField, Tooltip("If the timed summon can be stopped.")] private bool _stopTimedSummon;
		[SerializeField, Tooltip("If the timed summon will stop permanently.")] private bool _stopPermanently;
		[SerializeField, Tooltip("If the instanciation will be in the same point as the summoner.")] private bool _self;
		[SerializeField, Tooltip("If the instanciation will combine the point of the summoner and the instantiation.")] private bool _combine;
		[SerializeField, Tooltip("If the instantiation will be for every point for the amount of quantity.")] private bool _sequential;
		[SerializeField, Tooltip("If the instantiation will be randomized at one of the points.")] private bool _random;
		public GameObject Summon
		{
			get => this._enemySummon ? this._enemySummon.gameObject : this._projectileSummon ? this._projectileSummon.gameObject : null;
		}
		public Vector2[] SummonPoints => this._summonPoints;
		public ushort SummonTime => this._summonTime;
		public ushort QuantityToSummon => this._quantityToSummon;
		public float TimeToStop => this._timeToStop;
		public bool StopToSummon => this._stopToSummon;
		public bool ParalyzeToSummon => this._paralyzeToSummon;
		public bool StopTimedSummon => this._stopTimedSummon;
		public bool StopPermanently => this._stopPermanently;
		public bool Self => this._self;
		public bool Combine => this._combine;
		public bool Sequential => this._sequential;
		public bool Random => this._random;
	};
};
