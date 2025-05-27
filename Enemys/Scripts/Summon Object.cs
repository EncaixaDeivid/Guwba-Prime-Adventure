using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Summon", menuName = "Scriptable Objects/Summon", order = 1)]
	public sealed class SummonObject : ScriptableObject
	{
		[SerializeField] private EnemyController _enemySummon;
		[SerializeField] private Projectile _projectileSummon;
		[SerializeField] private Vector2[] _summonPoints;
		[SerializeField] private ushort _summonTime;
		[SerializeField] private ushort _quantityToSummon;
		[SerializeField] private float _timeToStop;
		[SerializeField] private bool _stopToSummon;
		[SerializeField] private bool _paralyzeToSummon;
		[SerializeField] private bool _stopTimedSummon;
		[SerializeField] private bool _self;
		[SerializeField] private bool _combine;
		[SerializeField] private bool _sequential;
		[SerializeField] private bool _random;
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
		public bool Self => this._self;
		public bool Combine => this._combine;
		public bool Sequential => this._sequential;
		public bool Random => this._random;
	};
};
