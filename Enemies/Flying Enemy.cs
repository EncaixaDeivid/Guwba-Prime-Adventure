using UnityEngine;
using GwambaPrimeAdventure.Character;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(PolygonCollider2D))]
	internal sealed class FlyingEnemy : MovingEnemy
	{
		private CircleCollider2D _selfCollider;
		private Vector2[] _trail;
		private Vector2 _movementDirection;
		private Vector2 _pointOrigin;
		private Vector2 _targetPoint;
		private bool _normal = true;
		private bool _returnOrigin = false;
		private bool _afterDash = false;
		private bool _returnDash = false;
		private ushort _pointIndex = 0;
		private float _movementSpeed = 0f;
		[Header("Flying Enemy")]
		[SerializeField, Tooltip("The flying statitics of this enemy.")] private FlyingStatistics _statistics;
		[SerializeField, Tooltip("If this enemy will repeat the same way it makes before.")] private bool _repeatWay;
		private new void Awake()
		{
			base.Awake();
			_selfCollider = _collider as CircleCollider2D;
			PolygonCollider2D trail = GetComponent<PolygonCollider2D>();
			_trail = new Vector2[trail.points.Length];
			for (ushort i = 0; i < trail.points.Length; i++)
				_trail[i] = transform.TransformPoint(trail.points[i] + trail.offset);
			_movementDirection = Vector2.right * _movementSide;
			_pointOrigin = Rigidbody.position;
		}
		private void Chase()
		{
			_returnOrigin = true;
			if (_returnDash)
			{
				Rigidbody.MovePosition(Vector2.MoveTowards(Rigidbody.position, _pointOrigin, Time.fixedDeltaTime * _statistics.ReturnSpeed));
				_returnDash = Vector2.Distance(Rigidbody.position, _targetPoint) <= _statistics.TargetDistance;
				return;
			}
			else if (!_isDashing && Vector2.Distance(Rigidbody.position, _targetPoint) <= _statistics.TargetDistance)
				if (_statistics.DetectionStop)
				{
					(_stopWorking, _stoppedTime) = (true, _statistics.StopTime);
					return;
				}
				else
					_isDashing = true;
			if (_isDashing)
				Rigidbody.MovePosition(Vector2.MoveTowards(Rigidbody.position, _targetPoint, Time.fixedDeltaTime * _statistics.DashSpeed));
			else
			{
				_movementDirection = Vector2.MoveTowards(_movementDirection, (_targetPoint - Rigidbody.position).normalized, Time.fixedDeltaTime * _statistics.RotationSpeed);
				Rigidbody.MovePosition(Vector2.MoveTowards(Rigidbody.position, Rigidbody.position + _movementDirection, Time.fixedDeltaTime * _statistics.MovementSpeed));
			}
			if (_isDashing && Vector2.Distance(Rigidbody.position, _targetPoint) <= 1e-3f)
				if (_statistics.DetectionStop)
					(_stopWorking, _stoppedTime) = (_returnDash = _afterDash = true, _statistics.AfterTime);
				else
					_isDashing = !(_returnDash = true);
		}
		private void Trail()
		{
			if (_returnOrigin)
			{
				Rigidbody.MovePosition(Vector2.MoveTowards(Rigidbody.position, _pointOrigin, Time.fixedDeltaTime * _statistics.ReturnSpeed));
				transform.TurnScaleX(_pointOrigin.x < Rigidbody.position.x);
				_returnOrigin = Vector2.Distance(Rigidbody.position, _pointOrigin) > 1e-3f;
			}
			else if (_trail.Length > 0f)
			{
				if (Vector2.Distance(Rigidbody.position, _trail[_pointIndex]) <= 1e-3f)
					if (_repeatWay)
						_pointIndex = (ushort)(_pointIndex < _trail.Length - 1f ? _pointIndex + 1f : 0f);
					else if (_normal)
					{
						_pointIndex += 1;
						_normal = _pointIndex != _trail.Length - 1;
					}
					else if (!_normal)
					{
						_pointIndex -= 1;
						_normal = _pointIndex == 0f;
					}
				Rigidbody.MovePosition(Vector2.MoveTowards(Rigidbody.position, _trail[_pointIndex], Time.fixedDeltaTime * _statistics.MovementSpeed));
				transform.TurnScaleX(_trail[_pointIndex].x < Rigidbody.position.x);
				_pointOrigin = Rigidbody.position;
			}
		}
		private void Update()
		{
			if (IsStunned)
				return;
			if (_statistics.DetectionStop && _stopWorking)
				if ((_stoppedTime -= Time.deltaTime) <= 0f)
					(_isDashing, _afterDash, _stopWorking) = (!_afterDash, false, false);
		}
		private void FixedUpdate()
		{
			if (_stopWorking || IsStunned)
				return;
			if (_statistics.Target)
			{
				_movementSpeed = Time.fixedDeltaTime * _statistics.RotationSpeed;
				_movementDirection = Vector2.MoveTowards(_movementDirection, ((Vector2)_statistics.Target.transform.position - Rigidbody.position).normalized, _movementSpeed);
				Rigidbody.MovePosition(Vector2.MoveTowards(Rigidbody.position, Rigidbody.position + _movementDirection, Time.fixedDeltaTime * _statistics.MovementSpeed));
				transform.TurnScaleX(_statistics.Target.transform.position.x < Rigidbody.position.x);
				return;
			}
			if (_statistics.EndlessPursue)
			{
				_movementSpeed = Time.fixedDeltaTime * _statistics.RotationSpeed;
				_movementDirection = Vector2.MoveTowards(_movementDirection, (GwambaStateMarker.Localization - Rigidbody.position).normalized, _movementSpeed);
				Rigidbody.MovePosition(Vector2.MoveTowards(Rigidbody.position, Rigidbody.position + _movementDirection, Time.fixedDeltaTime * _statistics.MovementSpeed));
				transform.TurnScaleX(GwambaStateMarker.Localization.x < Rigidbody.position.x);
				return;
			}
			if (_isDashing)
			{
				_originCast = Rigidbody.position + _selfCollider.offset + (_targetPoint - _originCast).normalized * 5e-1f;
				if (Physics2D.CircleCast(_originCast, _selfCollider.radius, (_targetPoint - _originCast).normalized, 5e-1f, _statistics.Physics.GroundLayer))
					if (_statistics.DetectionStop)
						(_stopWorking, _stoppedTime) = (_returnDash = _afterDash = true, _statistics.AfterTime);
					else
						_isDashing = !(_returnDash = true);
			}
			else
				_detected = false;
			if (_statistics.LookPerception && !_isDashing)
				foreach (Collider2D verifyCollider in Physics2D.OverlapCircleAll(_pointOrigin, _statistics.LookDistance, _statistics.Physics.TargetLayer))
					if (GwambaStateMarker.EqualObject(verifyCollider.gameObject))
					{
						_targetPoint = verifyCollider.transform.position;
						_originCast = Rigidbody.position + _selfCollider.offset;
						for (ushort i = 0; i < Mathf.FloorToInt(Vector2.Distance(Rigidbody.position + _selfCollider.offset, _targetPoint) / _statistics.DetectionFactor); i++)
						{
							if (Physics2D.OverlapCircle(_originCast, _selfCollider.radius, _statistics.Physics.GroundLayer))
								break;
							_originCast += _statistics.DetectionFactor * (_targetPoint - _originCast).normalized;
						}
						transform.TurnScaleX(verifyCollider.transform.position.x < transform.position.x ? -1f : 1f);
						_detected = true;
						break;
					}
			if (_detected || _returnDash)
				Chase();
			else
				Trail();
		}
	};
};
