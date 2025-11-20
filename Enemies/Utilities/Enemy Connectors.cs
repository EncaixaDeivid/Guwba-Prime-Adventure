using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Rigidbody2D), typeof(Collider2D)), RequireComponent(typeof(CinemachineImpulseSource))]
	public abstract class Control : StateController
	{
		protected Rigidbody2D _rigidbody;
		protected CinemachineImpulseSource _screenShaker;
		protected IDestructible _destructibleEnemy;
		protected short _vitality;
		protected short _armorResistance = 0;
		protected float _fadeTime = 0f;
		protected float _stunTimer = 0f;
		protected bool _stunned = false;
	};
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Rigidbody2D)), RequireComponent(typeof(CinemachineImpulseSource), typeof(Collider2D))]
	public abstract class Projectile : StateController
	{
		protected Rigidbody2D _rigidbody;
		protected CinemachineImpulseSource _screenShaker;
		protected readonly List<Projectile> _projectiles = new();
		protected IEnumerator _parabolicEvent;
		protected Vector2Int _oldCellPosition = new();
		protected Vector2Int _cellPosition = new();
		protected short _vitality;
		protected ushort _angleMulti = 0;
		protected ushort _pointToJump = 0;
		protected ushort _pointToBreak = 0;
		protected ushort _internalBreakPoint = 0;
		protected ushort _pointToReturn = 0;
		protected ushort _internalReturnPoint = 0;
		protected float _deathTimer = 0f;
		protected float _stunTimer = 0f;
		protected bool _breakInUse = false;
	};
	public interface IJumper
	{
		public void OnJump(ushort jumpIndex);
	};
	public interface ISummoner
	{
		public void OnSummon(ushort summonIndex);
	};
	public interface ITeleporter
	{
		public void OnTeleport(ushort teleportIndex);
	};
};
