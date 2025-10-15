using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	internal abstract class MovingEnemy : EnemyProvider, IConnector
	{
		protected bool _detected = false;
		protected bool _isDashing = false;
		protected float _stoppedTime = 0f;
		protected short _movementSide = 1;
		[Header("Moving Enemy")]
		[SerializeField, Tooltip("The moving statitics of this enemy.")] private MovingStatistics _moving;
		[SerializeField, Tooltip("If this enemy will moves firstly to the left.")] private bool _invertMovementSide;
		protected new void Awake()
		{
			base.Awake();
			_sender.SetStateForm(StateForm.State);
			transform.localScale = new Vector3()
			{
				x = Mathf.Abs(transform.localScale.x) * (_movementSide = (short)(_invertMovementSide ? -1 : 1)),
				y = transform.localScale.y,
				z = transform.localScale.z
			};
			Sender.Include(this);
		}
		protected new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		protected bool SurfacePerception()
		{
			float originY = (_collider.bounds.extents.y + _moving.Physics.GroundChecker / 2f) * -transform.up.y;
			Vector2 origin = new(transform.position.x + _collider.offset.x, transform.position.y + _collider.offset.y + originY);
			Vector2 size = new(_collider.bounds.size.x - _moving.Physics.GroundChecker, _moving.Physics.GroundChecker);
			return Physics2D.BoxCast(origin, size, 0f, -transform.up, _moving.Physics.GroundChecker, _moving.Physics.GroundLayer);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if ((EnemyProvider[])additionalData != null)
				foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
					if (enemy == this && data.StateForm == StateForm.State && data.ToggleValue.HasValue)
						_stopWorking = !data.ToggleValue.Value;
		}
	};
};
