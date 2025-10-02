using UnityEngine;
using System;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	internal abstract class MovingEnemy : EnemyController, IConnector
	{
		private Vector2 _guardVelocity = new();
		protected bool _detected = false;
		protected bool _isDashing = false;
		protected float _stoppedTime = 0f;
		protected short _movementSide = 1;
		[Header("Moving Enemy")]
		[SerializeField, Tooltip("Size of collider for checking the ground below the feet.")] private float _groundChecker;
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
			this.transform.localScale = new Vector3()
			{
				x = this._invertMovementSide ? -MathF.Abs(this.transform.localScale.x) : MathF.Abs(this.transform.localScale.x),
				y = this.transform.localScale.y,
				z = this.transform.localScale.z
			};
		}
		private new void OnEnable()
		{
			base.OnEnable();
			this._rigidybody.linearVelocity = this._guardVelocity;
		}
		private void OnDisable()
		{
			this._guardVelocity = this._rigidybody.linearVelocity;
			this._rigidybody.linearVelocity = Vector2.zero;
		}
		protected bool SurfacePerception()
		{
			float yPoint = this.transform.position.y - this._collider.bounds.extents.y + this._collider.offset.y - this._groundChecker / 2f;
			Vector2 origin = new(this.transform.position.x, yPoint);
			Vector2 size = new(this._collider.bounds.size.x - this._groundChecker, this._groundChecker);
			float angle = this.transform.rotation.z * Mathf.Rad2Deg;
			return Physics2D.BoxCast(origin, size, angle, -this.transform.up, this._groundChecker, this._groundLayer);
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			EnemyController[] enemies = (EnemyController[])additionalData;
			if (enemies != null)
				foreach (EnemyController enemy in enemies)
					if (enemy == this && data.StateForm == StateForm.State && data.ToggleValue.HasValue)
						this._stopWorking = !data.ToggleValue.Value;
		}
	};
};
