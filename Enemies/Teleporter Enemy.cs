using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class TeleporterEnemy : EnemyProvider, ITeleporter
	{
		private bool _canTeleport = true;
		private ushort _teleportIndex = 0;
		private float _teleportTime = 0F;
		[Header("Teleporter Enemy")]
		[SerializeField, Tooltip("The teleporter statitics of this enemy.")] private TeleporterStatistics _statistics;
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			for (ushort i = 0; i < _statistics.TeleportPointStructures.Length; i++)
				Instantiate(_statistics.TeleportPointStructures[i].TeleportPointObject, _statistics.TeleportPointStructures[i].InstancePoint, Quaternion.identity).GetTouch(this, i);
		}
		private void Update()
		{
			if (IsStunned)
				return;
			if (_teleportTime > 0F)
				_canTeleport = (_teleportTime -= Time.deltaTime) <= 0F;
		}
		public void OnTeleport(ushort teleportIndex)
		{
			if (_canTeleport)
			{
				if (_statistics.TeleportPointStructures[teleportIndex].RandomTeleports)
					_teleportIndex = (ushort)Random.Range(0, _statistics.TeleportPointStructures[teleportIndex].TeleportPoints.Length);
				transform.position = _statistics.TeleportPointStructures[teleportIndex].TeleportPoints[_teleportIndex];
				if (!_statistics.TeleportPointStructures[teleportIndex].RandomTeleports)
					_teleportIndex = (ushort)(_teleportIndex < _statistics.TeleportPointStructures[teleportIndex].TeleportPoints.Length - 1 ? _teleportIndex + 1 : 0);
				(_canTeleport, _teleportTime) = (false, _statistics.TimeToUse);
			}
		}
	};
};
