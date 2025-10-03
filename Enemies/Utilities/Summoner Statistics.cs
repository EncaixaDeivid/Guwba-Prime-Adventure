using UnityEngine;
using System;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Summoner Enemy", menuName = "Enemy Statistics/Summoner", order = 6)]
	internal sealed class SummonerStatistics : ScriptableObject
	{
		[Header("Summoner Enemy")]
		[SerializeField, Tooltip("The physics of the enemy.")] private EnemyPhysics _physics;
		[SerializeField, Tooltip("The summons that will be activate on an event.")] private SummonObject[] _eventSummons;
		[SerializeField, Tooltip("The summons that will be activate with time.")] private SummonObject[] _timedSummons;
		[SerializeField, Tooltip("The collection of the summon point structure.")] private SummonPointStructure[] _summonPointStructures;
		[SerializeField, Tooltip("If this enemy will summon randomized in the react.")] private bool _randomReactSummons;
		[SerializeField, Tooltip("If this enemy will summon randomized timed.")] private bool _randomTimedSummons;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		internal EnemyPhysics Physics => this._physics;
		internal SummonObject[] EventSummons => this._eventSummons;
		internal SummonObject[] TimedSummons => this._timedSummons;
		internal SummonPointStructure[] SummonPointStructures => this._summonPointStructures;
		internal bool RandomReactSummons => this._randomReactSummons;
		internal bool RandomTimedSummons => this._randomTimedSummons;
		internal bool ReactToDamage => this._reactToDamage;
	};
	[Serializable]
	internal struct SummonPointStructure
	{
		[SerializeField, Tooltip("The object to activate the summon.")] private SummonPoint _summonPointObject;
		[SerializeField, Tooltip("Which summon event the summon point will activate.")] private SummonObject _objectToSummon;
		[SerializeField, Tooltip("The point where the summon point will be.")] private Vector2 _point;
		internal readonly SummonPoint SummonPointObject => this._summonPointObject;
		internal readonly SummonObject Summon => this._objectToSummon;
		internal readonly Vector2 Point => this._point;
	};
};