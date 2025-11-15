using UnityEngine;
using System.Collections.Generic;
namespace GwambaPrimeAdventure.Enemy.Utility
{
	public abstract class Control : StateController
	{
		protected Rigidbody2D _rigidbody;
		protected IDestructible _destructibleEnemy;
		protected short _vitality;
		protected short _armorResistance = 0;
		protected float _fadeTime = 0f;
		protected float _stunTimer = 0f;
		protected bool _stunned = false;
	};
	public abstract class Projectile : StateController
	{
		protected Rigidbody2D _rigidbody;
		protected readonly List<Projectile> _projectiles = new();
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
		protected bool _parabolaCoroutine = false;
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