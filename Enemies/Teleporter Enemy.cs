using UnityEngine;
using System.Collections;
namespace GuwbaPrimeAdventure.Enemy
{
	internal sealed class TeleporterEnemy : EnemyController
	{
		private bool _canTeleport = true;
		[Header("Teleporter Enemy")]
		[SerializeField, Tooltip("The collection of the summon places.")] private TeleportPointStructure[] _teleportPointStructures;
		[SerializeField, Tooltip("The amount of time to use the teleport again.")] private float _timeToUse;
		private new void Awake()
		{
			base.Awake();
			foreach (TeleportPointStructure teleportPointStructure in this._teleportPointStructures)
			{
				TeleportPoint teleportPoint = teleportPointStructure.TeleportPointObject;
				Instantiate(teleportPoint, teleportPointStructure.InstancePoint, Quaternion.identity).GetTouch(() =>
				{
					if (this._canTeleport)
					{
						Vector2 teleportPointToUse;
						if (teleportPointStructure.RandomTeleports)
						{
							ushort index = (ushort)Random.Range(0, teleportPointStructure.TeleportPoints.Length - 1f);
							teleportPointToUse = teleportPointStructure.TeleportPoints[index];
						}
						else
							teleportPointToUse = teleportPointStructure.TeleportPoints[0];
						this.transform.position = teleportPointToUse;
						this.StartCoroutine(UseTimer());
					}
					IEnumerator UseTimer()
					{
						this._canTeleport = false;
						yield return new WaitTime(this, this._timeToUse);
						this._canTeleport = true;
					}
				});
			}
		}
		[System.Serializable]
		private struct TeleportPointStructure
		{
			[SerializeField, Tooltip("The point where the teleport point will be.")] private TeleportPoint _teleportPoint;
			[SerializeField, Tooltip("The point where the teleport point will be.")] private Vector2 _instancePoint;
			[SerializeField, Tooltip("The points where the point will teleport to.")] private Vector2[] _teleportPoints;
			[SerializeField, Tooltip("If the points to teleport will be random.")] private bool _randomTeleports;
			internal readonly TeleportPoint TeleportPointObject => this._teleportPoint;
			internal readonly Vector2 InstancePoint => this._instancePoint;
			internal readonly Vector2[] TeleportPoints => this._teleportPoints;
			internal readonly bool RandomTeleports => this._randomTeleports;
		}
	};
};