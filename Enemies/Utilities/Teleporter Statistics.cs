using UnityEngine;
using System;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	[CreateAssetMenu(fileName = "Teleporter Enemy", menuName = "Enemy Statistics/Teleporter", order = 9)]
	public sealed class TeleporterStatistics : ScriptableObject
	{
		[Header("Teleporter Enemy")]
		[SerializeField, Tooltip("The collection of the summon places.")] private TeleportPointStructure[] _teleportPointStructures;
		[SerializeField, Tooltip("The amount of time to use the teleport again.")] private float _timeToUse;
		public TeleportPointStructure[] TeleportPointStructures => _teleportPointStructures;
		public float TimeToUse => _timeToUse;
	};
	[Serializable]
	public struct TeleportPointStructure
	{
		[SerializeField, Tooltip("The point where the teleport point will be.")] private TeleportPoint _teleportPoint;
		[SerializeField, Tooltip("The point where the teleport point will be.")] private Vector2 _instancePoint;
		[SerializeField, Tooltip("The points where the point will teleport to.")] private Vector2[] _teleportPoints;
		[SerializeField, Tooltip("If the points to teleport will be random.")] private bool _randomTeleports;
		public readonly TeleportPoint TeleportPointObject => _teleportPoint;
		public readonly Vector2 InstancePoint => _instancePoint;
		public readonly Vector2[] TeleportPoints => _teleportPoints;
		public readonly bool RandomTeleports => _randomTeleports;
	}
};
