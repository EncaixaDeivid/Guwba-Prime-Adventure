using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Summon", menuName = "Scriptable Objects/Summon", order = 1)]
	public sealed class SummonObject : ScriptableObject
	{
		[SerializeField] private EnemyController _enemySummon;
		[SerializeField] private Projectile _projectileSummon;
		[SerializeField] private Vector2[] _summonPoints;
		[SerializeField] private ushort _summonTime, _quantityToSummon;
		[SerializeField] private float _timeToStop;
		[SerializeField] private bool _stopToSummon, _paralyzeToSummon, _stopTimedSummon, _self, _combine, _sequential, _random;
		public GameObject Summon
		{
			get
			{
				if (this._enemySummon)
					return this._enemySummon.gameObject;
				if (this._projectileSummon)
					return this._projectileSummon.gameObject;
				return null;
			}
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