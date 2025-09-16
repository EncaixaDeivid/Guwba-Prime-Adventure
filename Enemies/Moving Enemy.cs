using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal abstract class MovingEnemy : EnemyController, IConnector
	{
		private Vector2 _guardVelocity = new();
		protected bool _detected = false;
		protected bool _isDashing = false;
		protected float _stoppedTime = 0f;
		[Header("Moving Enemy")]
		[SerializeField, Tooltip("The speed of the enemy to moves.")] protected ushort _movementSpeed;
		[SerializeField, Tooltip("The amount of speed of the dash.")] protected ushort _dashSpeed;
		[SerializeField, Tooltip("If this enemy will moves firstly to the left.")] private bool _invertMovementSide;
		[SerializeField, Tooltip("If this enemy will stop on detection of the target.")] protected bool _detectionStop;
		[SerializeField, Tooltip("If this enemy will shoot a projectile on detection.\nRequires: Shooter Enemy")] protected bool _shootDetection;
		[SerializeField, Tooltip("If this enemy will stop to shoot a projectile.\nRequires: Shooter Enemy")] protected bool _stopToShoot;
		[SerializeField, Tooltip("The amount of time this enemy will stop on detection.")] protected float _stopTime;
		protected new void Awake()
		{
			base.Awake();
			this._sender.SetStateForm(StateForm.Action);
			this.transform.right = this._invertMovementSide ? -Vector2.right : Vector2.right;
		}
		private new void OnEnable()
		{
			base.OnEnable();
			this._rigidybody.linearVelocity = this._guardVelocity;
		}
		private new void OnDisable()
		{
			base.OnDisable();
			this._guardVelocity = this._rigidybody.linearVelocity;
			this._rigidybody.linearVelocity = Vector2.zero;
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			if (additionalData as GameObject != this.gameObject)
				return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				this._stopWorking = !data.ToggleValue.Value;
		}
	};
};