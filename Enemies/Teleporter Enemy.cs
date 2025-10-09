using UnityEngine;
using System.Collections;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class TeleporterEnemy : EnemyProvider
	{
		private bool _canTeleport = true;
		[Header("Teleporter Enemy")]
		[SerializeField, Tooltip("The teleporter statitics of this enemy.")] private TeleporterStatistics _statistics;
		private new void Awake()
		{
			base.Awake();
			foreach (TeleportPointStructure teleportPointStructure in this._statistics.TeleportPointStructures)
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
						yield return new WaitTime(this, this._statistics.TimeToUse);
						this._canTeleport = true;
					}
				});
			}
		}
	};
};
