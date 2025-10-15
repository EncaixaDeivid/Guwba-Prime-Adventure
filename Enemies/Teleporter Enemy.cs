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
			foreach (TeleportPointStructure teleportPointStructure in _statistics.TeleportPointStructures)
			{
				Instantiate(teleportPointStructure.TeleportPointObject, teleportPointStructure.InstancePoint, Quaternion.identity).GetTouch(() =>
				{
					if (_canTeleport)
					{
						Vector2 teleportPointToUse;
						if (teleportPointStructure.RandomTeleports)
						{
							ushort index = (ushort)Random.Range(0, teleportPointStructure.TeleportPoints.Length - 1f);
							teleportPointToUse = teleportPointStructure.TeleportPoints[index];
						}
						else
							teleportPointToUse = teleportPointStructure.TeleportPoints[0];
						transform.position = teleportPointToUse;
						StartCoroutine(UseTimer());
					}
					IEnumerator UseTimer()
					{
						_canTeleport = false;
						yield return new WaitTime(this, _statistics.TimeToUse);
						_canTeleport = true;
					}
				});
			}
		}
	};
};
