using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[RequireComponent(typeof(Transform), typeof(Rigidbody2D), typeof(Collider2D))]
	[RequireComponent(typeof(EnemyProvider))]
	internal sealed class EnemyController : StateController, ILoader, IConnector, IDestructible
    {
		private EnemyProvider[] _selfEnemies;
		private Rigidbody2D _rigidybody;
		private IDestructible _destructibleEnemy;
		private short _vitality;
		private short _armorResistance = 0;
		private float _fadeTime = 0f;
		private float _stunTimer = 0f;
		private bool _stunned = false;
		[Header("Enemy Statistics")]
		[SerializeField, Tooltip("The control statitics of this enemy.")] private EnemyStatistics _statistics;
		internal EnemyStatistics ProvidenceStatistics => _statistics;
		public PathConnection PathConnection => PathConnection.Enemy;
		public short Health => _vitality;
		internal short Vitality { get => _vitality; set => _vitality = value; }
		internal short ArmorResistance { get => _armorResistance; set => _armorResistance = value; }
		internal float StunTimer { get => _stunTimer; set => _stunTimer = value; }
		internal bool IsStunned { get => _stunned; set => _stunned = value; }
		private new void Awake()
		{
			base.Awake();
			_selfEnemies = GetComponents<EnemyProvider>();
			_rigidybody = GetComponent<Rigidbody2D>();
			_vitality = (short)_statistics.Vitality;
			_armorResistance = (short)_statistics.HitResistance;
			_fadeTime = _statistics.TimeToFadeAway;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			SaveController.Load(out SaveFile saveFile);
			if (_statistics.SaveOnSpecifics && !saveFile.generalObjects.Contains(gameObject.name))
			{
				saveFile.generalObjects.Add(gameObject.name);
				SaveController.WriteSave(saveFile);
			}
			Sender.Exclude(this);
		}
		private void OnEnable() => _rigidybody.WakeUp();
		private void OnDisable() => _rigidybody.Sleep();
		private IEnumerator Start()
		{
			foreach (EnemyProvider enemy in _selfEnemies)
				enemy.enabled = false;
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			foreach (EnemyProvider enemy in _selfEnemies)
				enemy.enabled = true;
		}
		public IEnumerator Load()
		{
			for (ushort i = 0; i < _selfEnemies.Length - 1f; i++)
				if (_selfEnemies[i + 1].DestructilbePriority > _selfEnemies[i].DestructilbePriority)
					_destructibleEnemy = _selfEnemies[i + 1];
			yield return new WaitForEndOfFrame();
		}
		public IEnumerator Reload() => null;
		private void Update()
		{
			if (_statistics.FadeOverTime)
			{
				_fadeTime -= Time.deltaTime;
				if (_fadeTime <= 0)
					Destroy(gameObject);
			}
			if (_stunned)
			{
				_stunTimer -= Time.deltaTime;
				if (_stunTimer <= 0f)
				{
					_stunned = false;
					_rigidybody.WakeUp();
				}
			}
		}
		private void OnTrigger(GameObject collisionObject)
		{
			if (!_statistics.NoHit && collisionObject.TryGetComponent<IDestructible>(out var destructible) && destructible.Hurt(_statistics.Damage))
			{
				destructible.Stun(_statistics.Damage, _statistics.StunTime);
				EffectsController.HitStop(_statistics.Physics.HitStopTime, _statistics.Physics.HitSlowTime);
			}
		}
		private void OnTriggerEnter2D(Collider2D other) => OnTrigger(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => OnTrigger(other.gameObject);
		public bool Hurt(ushort damage)
		{
			if (_statistics.NoDamage || damage <= 0)
				return false;
			return _destructibleEnemy.Hurt(damage);
		}
		public void Stun(ushort stunStength, float stunTime)
		{
			if (_statistics.NoStun || _stunned)
				return;
			_destructibleEnemy.Stun(stunStength, stunTime);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.None && data.ToggleValue.HasValue)
			{
				_rigidybody.Sleep();
				foreach (EnemyProvider enemy in _selfEnemies)
					enemy.enabled = data.ToggleValue.Value;
			}
		}
	};
};
