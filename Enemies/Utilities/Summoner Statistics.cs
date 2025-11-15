using UnityEngine;
using System;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	[CreateAssetMenu(fileName = "Summoner Enemy", menuName = "Enemy Statistics/Summoner", order = 6)]
	public sealed class SummonerStatistics : ScriptableObject
	{
		[Header("Summoner Enemy")]
		[SerializeField, Tooltip("The summons that will be activate on an event.")] private SummonObject[] _eventSummons;
		[SerializeField, Tooltip("The summons that will be activate with time.")] private SummonObject[] _timedSummons;
		[SerializeField, Tooltip("The collection of the summon point structure.")] private SummonPointStructure[] _summonPointStructures;
		[SerializeField, Tooltip("If this enemy will summon randomized in the react.")] private bool _randomReactSummons;
		[SerializeField, Tooltip("If this enemy will summon randomized timed.")] private bool _randomTimedSummons;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _hasEventSummon;
		public SummonObject[] EventSummons => _eventSummons;
		public SummonObject[] TimedSummons => _timedSummons;
		public SummonPointStructure[] SummonPointStructures => _summonPointStructures;
		public bool RandomReactSummons => _randomReactSummons;
		public bool RandomTimedSummons => _randomTimedSummons;
		public bool HasEventSummon => _hasEventSummon;
	};
	[Serializable]
	public struct SummonPointStructure
	{
		[SerializeField, Tooltip("The object to activate the summon.")] private SummonPoint _summonPointObject;
		[SerializeField, Tooltip("Which summon event the summon point will activate.")] private SummonObject _objectToSummon;
		[SerializeField, Tooltip("The point where the summon point will be.")] private Vector2 _point;
		public readonly SummonPoint SummonPointObject => _summonPointObject;
		public readonly SummonObject Summon => _objectToSummon;
		public readonly Vector2 Point => _point;
	};
};
