using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(PolygonCollider2D))]
	internal sealed class FlyingEnemy : MovingEnemy
	{
		private PolygonCollider2D _trail;
		private Vector2 _pointOrigin;
		private Vector2 _targetPoint;
		private bool _normal = true;
		private bool _afterDash = false;
		private bool _returnDash = false;
		private ushort _pointIndex = 0;
		private float _fadeTime = 0f;
		[Header("Flying Enemy")]
		[SerializeField, Tooltip("The flying statitics of this enemy.")] private FlyingStatistics _statistics;
		[SerializeField, Tooltip("If this enemy will repeat the same way it makes before.")] private bool _repeatWay;
		private new void Awake()
		{
			base.Awake();
			_trail = GetComponent<PolygonCollider2D>();
			_pointOrigin = transform.position;
		}
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			_fadeTime = _statistics.FadeTime;
		}
		private void Chase()
		{
			if (_returnDash)
			{
				transform.position = Vector2.MoveTowards(transform.position, _pointOrigin, Time.deltaTime * _statistics.ReturnSpeed);
				_returnDash = Vector2.Distance(transform.position, _targetPoint) <= _statistics.TargetDistance;
				return;
			}
			else if (!_isDashing && Vector2.Distance(transform.position, _targetPoint) <= _statistics.TargetDistance)
				if (_statistics.DetectionStop)
				{
					(_stopWorking, _stoppedTime) = (true, _statistics.StopTime);
					return;
				}
				else
					_isDashing = true;
			transform.position = Vector2.MoveTowards(transform.position, _targetPoint, Time.deltaTime * (_isDashing ? _statistics.DashSpeed : _statistics.MovementSpeed));
			if (_isDashing && Vector2.Distance(transform.position, _targetPoint) <= 1e-3f)
				if (_statistics.DetectionStop)
					(_stopWorking, _stoppedTime) = (_returnDash = _afterDash = true, _statistics.AfterTime);
				else
					_isDashing = !(_returnDash = true);
		}
		private void Trail()
		{
			if ((Vector2)transform.position != _pointOrigin)
			{
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (_pointOrigin.x < transform.position.x ? -1f : 1f), transform.localScale.y, transform.localScale.z);
				transform.position = Vector2.MoveTowards(transform.position, _pointOrigin, Time.deltaTime * _statistics.ReturnSpeed);
			}
			else if (_trail.points.Length > 0f)
			{
				if (_repeatWay)
				{
					if ((ushort)Vector2.Distance(transform.localPosition, _trail.points[_pointIndex]) <= 1e-3f)
						_pointIndex = (ushort)(_pointIndex < _trail.points.Length - 1f ? _pointIndex + 1f : 0f);
				}
				else if (_normal)
				{
					if ((ushort)Vector2.Distance(transform.localPosition, _trail.points[_pointIndex]) <= 1e-3f)
						_pointIndex += 1;
					_normal = _pointIndex != _trail.points.Length - 1f;
				}
				else if (!_normal)
				{
					if ((ushort)Vector2.Distance(transform.localPosition, _trail.points[_pointIndex]) <= 1e-3f)
						_pointIndex -= 1;
					_normal = _pointIndex == 0f;
				}
				transform.localScale = new Vector3()
				{
					x = Mathf.Abs(transform.localScale.x) * (_trail.points[_pointIndex].x < transform.localPosition.x ? -1f : 1f),
					y = transform.localScale.y,
					z = transform.localScale.z
				};
				transform.localPosition = Vector2.MoveTowards(transform.localPosition, _trail.points[_pointIndex], Time.deltaTime * _statistics.MovementSpeed);
				_pointOrigin = transform.position;
			}
		}
		private void Update()
		{
			if (_statistics.EndlessPursue && _fadeTime > 0f)
				if ((_fadeTime -= Time.deltaTime) <= 0f)
					Destroy(gameObject);
			if (IsStunned)
				return;
			if (_statistics.DetectionStop && _stopWorking)
				if ((_stoppedTime -= Time.deltaTime) <= 0f)
					(_isDashing, _afterDash, _stopWorking) = (!_afterDash, false, false);
			if (_stopWorking)
				return;
			if (_statistics.Target)
			{
				_targetPoint = _statistics.Target.transform.position;
				transform.position = Vector2.MoveTowards(transform.position, _targetPoint, Time.deltaTime * _statistics.MovementSpeed);
				return;
			}
			if (_statistics.EndlessPursue)
			{
				transform.position = Vector2.MoveTowards(transform.position, GuwbaAstralMarker.Localization, Time.deltaTime * _statistics.MovementSpeed);
				return;
			}
			if (_detected || _returnDash)
				Chase();
			else
				Trail();
		}
		private void FixedUpdate()
		{
			if (_stopWorking || IsStunned || _statistics.Target || _statistics.EndlessPursue)
				return;
			if (!_isDashing)
				_detected = false;
			if (_statistics.LookPerception && !_isDashing)
				foreach (Collider2D verifyCollider in Physics2D.OverlapCircleAll(_pointOrigin, _statistics.LookDistance, _statistics.Physics.TargetLayer))
					if (GuwbaAstralMarker.EqualObject(verifyCollider.gameObject))
					{
						_targetPoint = verifyCollider.transform.position;
						_detected = !Physics2D.Linecast(transform.position, _targetPoint, _statistics.Physics.GroundLayer);
						_originCast = (Vector2)transform.position + _collider.offset;
						if (!_detected)
							return;
						for (ushort i = 0; i < Mathf.FloorToInt(Vector2.Distance((Vector2)transform.position + _collider.offset, _targetPoint) / _statistics.DetectionFactor); i++)
						{
							_detected = !Physics2D.OverlapCircle(_originCast, (_collider as CircleCollider2D).radius, _statistics.Physics.GroundLayer);
							if (!_detected)
								return;
							_originCast += _statistics.DetectionFactor * Vector2.one * (_targetPoint - _originCast).normalized;
						}
						transform.localScale = new Vector3()
						{
							x = Mathf.Abs(transform.localScale.x) * (verifyCollider.transform.position.x < transform.position.x ? -1f : 1f),
							y = transform.localScale.y,
							z = transform.localScale.z
						};
						return;
					}
		}
	};
};
