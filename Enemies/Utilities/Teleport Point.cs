using UnityEngine;
using GwambaPrimeAdventure.Character;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class TeleportPoint : StateController
	{
		private ITeleporter _teleporter;
		private ushort _teleportIndex;
		[Header("Interactions")]
		[SerializeField, Tooltip("If this point will destroy itself after use.")] private bool _destroyAfter;
		[SerializeField, Tooltip("If this point will trigger with other object.")] private bool _hasTarget;
		internal void GetTouch(ITeleporter teleporter, ushort teleportIndex)
		{
			_teleporter = teleporter;
			_teleportIndex = teleportIndex;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_hasTarget)
			{
				if (GwambaStateMarker.EqualObject(other.gameObject))
					_teleporter.OnTeleport(_teleportIndex);
			}
			else if (other.TryGetComponent<TeleporterEnemy>(out _))
				_teleporter.OnTeleport(_teleportIndex);
			if (_destroyAfter)
				Destroy(gameObject);
		}
	};
	internal interface ITeleporter
	{
		public void OnTeleport(ushort teleportIndex);
	};
};
