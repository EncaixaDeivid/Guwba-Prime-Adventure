using UnityEngine;
using System;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Teleporter Enemy", menuName = "Enemy Statistics/Teleporter", order = 9)]
	internal sealed class TeleporterStatistics : ScriptableObject
	{
		[Header("Teleporter Enemy")]
		[SerializeField, Tooltip("The physics of the enemy.")] private EnemyPhysics _physics;
		[SerializeField, Tooltip("The collection of the summon places.")] private TeleportPointStructure[] _teleportPointStructures;
		[SerializeField, Tooltip("The amount of time to use the teleport again.")] private float _timeToUse;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		internal EnemyPhysics Physics => _physics;
		internal TeleportPointStructure[] TeleportPointStructures => _teleportPointStructures;
		internal float TimeToUse => _timeToUse;
		internal bool ReactToDamage => _reactToDamage;
	};
	[Serializable]
	internal struct TeleportPointStructure
	{
		[SerializeField, Tooltip("The point where the teleport point will be.")] private TeleportPoint _teleportPoint;
		[SerializeField, Tooltip("The point where the teleport point will be.")] private Vector2 _instancePoint;
		[SerializeField, Tooltip("The points where the point will teleport to.")] private Vector2[] _teleportPoints;
		[SerializeField, Tooltip("If the points to teleport will be random.")] private bool _randomTeleports;
		internal readonly TeleportPoint TeleportPointObject => _teleportPoint;
		internal readonly Vector2 InstancePoint => _instancePoint;
		internal readonly Vector2[] TeleportPoints => _teleportPoints;
		internal readonly bool RandomTeleports => _randomTeleports;
	}
};
