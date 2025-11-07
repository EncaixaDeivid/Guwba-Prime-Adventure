using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	internal sealed class EnemyProjectile : StateController, IDestructible
	{
		private Rigidbody2D _rigidbody;
		private readonly List<EnemyProjectile> _projectiles = new();
		private Vector2Int _oldCellPosition = new();
		private Vector2Int _cellPosition = new();
		private short _vitality;
		private ushort _angleMulti = 0;
		private ushort _pointToJump = 0;
		private ushort _pointToBreak = 0;
		private ushort _internalBreakPoint = 0;
		private ushort _pointToReturn = 0;
		private ushort _internalReturnPoint = 0;
		private float _stunTimer = 0f;
		private bool _breakInUse = false;
		[Header("Projectile")]
		[SerializeField, Tooltip("The statitics of this projectile.")] private ProjectileStatistics _statistics;
		public short Health => _vitality;
		private void CommonInstance()
		{
			Quaternion rotation;
			for (ushort i = 0; i < _statistics.QuantityToSummon; i++)
			{
				if (_statistics.UseSelfRotation)
					rotation = Quaternion.AngleAxis(transform.eulerAngles.z + _statistics.BaseAngle + _statistics.SpreadAngle * i, Vector3.forward);
				else
					rotation = Quaternion.AngleAxis(_statistics.BaseAngle + _statistics.SpreadAngle * i, Vector3.forward);
				Instantiate(_statistics.SecondProjectile, transform.position, rotation);
			}
		}
		private void CellInstance()
		{
			if (_oldCellPosition != _cellPosition)
			{
				_oldCellPosition = _cellPosition;
				if (_pointToJump == 0f)
				{
					if (_pointToBreak >= _internalBreakPoint)
						if (_pointToReturn++ >= _internalReturnPoint)
						{
							_pointToBreak = 0;
							_breakInUse = _statistics.AlwaysBreak;
						}
					if (!_breakInUse || _pointToBreak < _internalBreakPoint)
					{
						if (_breakInUse)
						{
							_pointToBreak++;
							_pointToReturn = 0;
						}
						_pointToJump = _statistics.JumpPoints;
						Quaternion rotation = Quaternion.AngleAxis(_statistics.BaseAngle + _statistics.SpreadAngle * _angleMulti, Vector3.forward);
						if (_statistics.UseQuantity)
							_projectiles.Add(Instantiate(_statistics.SecondProjectile, new Vector2(_cellPosition.x + .5f, _cellPosition.y + .5f), rotation));
						else
							Instantiate(_statistics.SecondProjectile, new Vector2(_cellPosition.x + .5f, _cellPosition.y + .5f), rotation);
						_angleMulti++;
					}
				}
				else if (_pointToJump > 0f)
					_pointToJump--;
			}
		}
		private void CellInstanceRange()
		{
			float distance = Physics2D.Raycast(transform.position, transform.up, _statistics.DistanceRay, _statistics.GroundLayer).distance;
			if (_statistics.UseQuantity)
				distance = _statistics.QuantityToSummon;
			for (ushort i = 0; i < distance; i++)
			{
				_cellPosition += new Vector2Int((short)transform.up.x, (short)transform.up.y);
				CellInstance();
			}
		}
		private void ParabolicProjectile()
		{
			StartCoroutine(Parabola());
			IEnumerator Parabola()
			{
				yield return new WaitUntil(() => isActiveAndEnabled && _rigidbody.IsAwake());
				float time = 0f, x, y;
				while (time > _statistics.TimeToFade)
				{
					time += Time.fixedDeltaTime;
					x = Mathf.Cos(_statistics.BaseAngle * Mathf.Deg2Rad);
					y = Mathf.Sin(_statistics.BaseAngle * Mathf.Deg2Rad);
					_rigidbody.MovePosition(new Vector2(_statistics.MovementSpeed * time * x, _statistics.MovementSpeed * time * y - 5e-1f * -Physics2D.gravity.y * Mathf.Pow(time, 2)));
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => isActiveAndEnabled && _rigidbody.IsAwake());
				}
			}
		}
		private new void Awake()
		{
			base.Awake();
			_rigidbody = GetComponent<Rigidbody2D>();
			_vitality = (short)_statistics.Vitality;
			_pointToJump = _statistics.JumpPoints;
			_breakInUse = _statistics.UseBreak;
			_internalBreakPoint = _statistics.BreakPoint;
			_internalReturnPoint = _statistics.ReturnPoint;
			if (_statistics.RandomBreak)
			{
				_internalBreakPoint = (ushort)Random.Range(_statistics.BreakPoint, _statistics.ReturnPoint - _statistics.MinimumRandomValue);
				if (_internalReturnPoint - _internalBreakPoint < _statistics.MinimumRandomValue)
					for (ushort i = 0; i < _statistics.MinimumRandomValue - (_internalReturnPoint - _internalBreakPoint); i++)
						if (_internalBreakPoint <= _statistics.MinimumRandomValue)
							_internalReturnPoint++;
						else
							_internalBreakPoint--;
				else if (_statistics.ExtrictRandom && _internalReturnPoint - _internalBreakPoint > _statistics.MinimumRandomValue)
					for (ushort i = 0; i < _statistics.MinimumRandomValue - (_internalReturnPoint - _internalBreakPoint); i++)
						if (_internalBreakPoint <= _statistics.MinimumRandomValue)
							_internalBreakPoint++;
						else
							_internalReturnPoint--;
			}
			_cellPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
			_oldCellPosition = _cellPosition;
			if (_statistics.SideMovement)
				transform.rotation = Quaternion.AngleAxis(_statistics.InvertSide ? 90f : -90f, Vector3.forward);
			if (_statistics.SecondProjectile && _statistics.InCell && !_statistics.ContinuosSummon)
				CellInstanceRange();
			else if (_statistics.SecondProjectile && !_statistics.InCell)
				CommonInstance();
			Destroy(gameObject, _statistics.TimeToFade);
		}
		private void Start()
		{
			if (!_statistics.StayInPlace)
				if (_statistics.UseForce)
					_rigidbody.AddForce((_statistics.InvertSide ? -transform.up : transform.up) * _statistics.MovementSpeed, _statistics.ForceMode);
				else
					_rigidbody.linearVelocity = (_statistics.InvertSide ? -transform.up : transform.up) * _statistics.MovementSpeed;
		}
		private void Update()
		{
			if (_rigidbody.IsSleeping())
				if ((_stunTimer -= Time.deltaTime) <= 0f)
					_rigidbody.WakeUp();
		}
		private void FixedUpdate()
		{
			if (_rigidbody.IsSleeping())
				return;
			if (_statistics.SecondProjectile && _statistics.InCell && _statistics.ContinuosSummon)
			{
				if (_statistics.UseQuantity && _statistics.QuantityToSummon == _projectiles.Count || _statistics.StayInPlace)
					return;
				_cellPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
				CellInstance();
			}
			_rigidbody.rotation += _statistics.RotationSpeed * Time.fixedDeltaTime;
			if (_statistics.ParabolicMovement)
				ParabolicProjectile();
			else if (!_statistics.StayInPlace && _statistics.RotationMatter)
				_rigidbody.linearVelocity = (_statistics.InvertSide ? -transform.up : transform.up) * _statistics.MovementSpeed;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_statistics.IsInoffensive)
				return;
			if (other.TryGetComponent<IDestructible>(out var destructible) && destructible.Hurt(_statistics.Damage))
			{
				destructible.Stun(_statistics.Damage, _statistics.StunTime);
				EffectsController.HitStop(_statistics.Physics.HitStopTime, _statistics.Physics.HitSlowTime);
			}
			else
			{
				if (_statistics.InDeath)
					if (_statistics.EnemyOnDeath)
						Instantiate(_statistics.EnemyOnDeath, transform.position, _statistics.EnemyOnDeath.transform.rotation);
					else if (_statistics.SecondProjectile)
						if (_statistics.InCell)
						{
							_cellPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
							CellInstanceRange();
						}
						else
							CommonInstance();
				Destroy(gameObject);
			}
		}
		public bool Hurt(ushort damage)
		{
			if (_statistics.IsInoffensive || damage <= 0 || _statistics.Vitality <= 0f)
				return false;
			if ((_vitality -= (short)damage) <= 0f)
				Destroy(gameObject);
			return true;
		}
		public void Stun(ushort stunStength, float stunTime)
		{
			if (_rigidbody.IsSleeping())
				return;
			_rigidbody.Sleep();
			_stunTimer = stunTime;
		}
	};
};
