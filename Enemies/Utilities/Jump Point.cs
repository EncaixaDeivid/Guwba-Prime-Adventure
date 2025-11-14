using UnityEngine;
using GwambaPrimeAdventure.Character;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class JumpPoint : StateController
	{
		private IJumper _jumper;
		private ushort _touchIndex;
		[Header("Interactions")]
		[SerializeField, Tooltip("If this point will destroy itself after use.")] private bool _destroyAfter;
		[SerializeField, Tooltip("If this point will trigger with other object.")] private bool _hasTarget;
		internal void GetTouch(IJumper jumperEnemy, ushort touchIndex)
		{
			_jumper = jumperEnemy;
			_touchIndex = touchIndex;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_hasTarget)
			{
				if (GwambaStateMarker.EqualObject(other.gameObject))
					_jumper.OnJump(_touchIndex);
			}
			else if (other.TryGetComponent<IJumper>(out _))
				_jumper.OnJump(_touchIndex);
			if (_destroyAfter)
				Destroy(gameObject);
		}
	};
	internal interface IJumper
	{
		public void OnJump(ushort jumpIndex);
	};
};
