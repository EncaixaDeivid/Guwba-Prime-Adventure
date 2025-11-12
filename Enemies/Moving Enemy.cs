using UnityEngine;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Enemy
{
	internal abstract class MovingEnemy : EnemyProvider, IConnector
	{
		protected Vector2 _originCast;
		protected Vector2 _sizeCast;
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
			transform.TurnScaleX(_movementSide = (short)(_invertMovementSide ? -1 : 1));
			Sender.Include(this);
		}
		protected new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		protected bool GroundCheck()
		{
			_originCast = new Vector2(transform.position.x + _collider.offset.x, transform.position.y + _collider.offset.y);
			_originCast += new Vector2(0f, (_collider.bounds.extents.y + _moving.Physics.GroundChecker / 2f) * -transform.up.y);
			_sizeCast = new Vector2(_collider.bounds.size.x - _moving.Physics.GroundChecker, _moving.Physics.GroundChecker);
			return Physics2D.BoxCast(_originCast, _sizeCast, 0f, -transform.up, _moving.Physics.GroundChecker, _moving.Physics.GroundLayer);
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
