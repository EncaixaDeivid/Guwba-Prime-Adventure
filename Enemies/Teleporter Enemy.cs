using UnityEngine;
using System.Collections;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class TeleporterEnemy : EnemyProvider
	{
		private ushort _teleportIndex = 0;
		private bool _canTeleport = true;
		[Header("Teleporter Enemy")]
		[SerializeField, Tooltip("The teleporter statitics of this enemy.")] private TeleporterStatistics _statistics;
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			foreach (TeleportPointStructure teleportPointStructure in _statistics.TeleportPointStructures)
			{
				Instantiate(teleportPointStructure.TeleportPointObject, teleportPointStructure.InstancePoint, Quaternion.identity).GetTouch(() =>
				{
					if (_canTeleport)
					{
						if (teleportPointStructure.RandomTeleports)
							_teleportIndex = (ushort)Random.Range(0, teleportPointStructure.TeleportPoints.Length - 1);
						transform.position = teleportPointStructure.TeleportPoints[_teleportIndex];
						if (!teleportPointStructure.RandomTeleports)
							_teleportIndex = (ushort)(_teleportIndex >= teleportPointStructure.TeleportPoints.Length - 1 ? _teleportIndex + 1 : 0);
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
