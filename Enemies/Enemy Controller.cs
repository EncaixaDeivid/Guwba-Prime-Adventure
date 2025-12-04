using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using GwambaPrimeAdventure.Connection;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	internal sealed class EnemyController : Control, ILoader, IConnector, IOccludee, IDestructible
   {
		private EnemyProvider[] _selfEnemies;
		[Header("Enemy Statistics")]
		[SerializeField, Tooltip("The control statitics of this enemy.")] private EnemyStatistics _statistics;
		internal EnemyStatistics ProvidenceStatistics => _statistics;
		internal Rigidbody2D Rigidbody => _rigidbody;
		public MessagePath Path => MessagePath.Enemy;
		public short Health => _vitality;
		internal short Vitality { get => _vitality; set => _vitality = value; }
		internal short ArmorResistance { get => _armorResistance; set => _armorResistance = value; }
		internal float StunTimer { get => _stunTimer; set => _stunTimer = value; }
		internal bool IsStunned { get => _stunned; set => _stunned = value; }
		public bool Occlude => !_statistics.FadeOverTime;
		private new void Awake()
		{
			base.Awake();
			_selfEnemies = GetComponents<EnemyProvider>();
			_rigidbody = GetComponent<Rigidbody2D>();
			_screenShaker = GetComponent<CinemachineImpulseSource>();
			_vitality = (short)_statistics.Vitality;
			_armorResistance = (short)_statistics.HitResistance;
			_fadeTime = _statistics.TimeToFadeAway;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			SaveController.Load(out SaveFile saveFile);
			if (_statistics.SaveOnSpecifics && !saveFile.GeneralObjects.Contains(name))
			{
				saveFile.GeneralObjects.Add(name);
				SaveController.WriteSave(saveFile);
			}
			Sender.Exclude(this);
		}
		private void OnEnable() => Rigidbody.WakeUp();
		private void OnDisable() => Rigidbody.Sleep();
		private IEnumerator Start()
		{
			foreach (EnemyProvider enemy in _selfEnemies)
				enemy.enabled = false;
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			_destructibleEnemy = _selfEnemies[0];
			for (ushort i = 0; i < _selfEnemies.Length - 1; i++)
				if (_selfEnemies[i + 1].DestructilbePriority > _selfEnemies[i].DestructilbePriority)
					_destructibleEnemy = _selfEnemies[i + 1];
			foreach (EnemyProvider enemy in _selfEnemies)
				enemy.enabled = true;
		}
		public IEnumerator Load()
		{
			SaveController.Load(out SaveFile saveFile);
			if (_statistics.SaveOnSpecifics && saveFile.GeneralObjects.Contains(name))
				Destroy(gameObject);
			yield return null;
		}
		private void Update()
		{
			if (SceneInitiator.IsInTrancision())
				return;
			if (_statistics.FadeOverTime)
				if ((_fadeTime -= Time.deltaTime) <= 0F)
					Destroy(gameObject);
			if (_stunned)
				if ((_stunTimer -= Time.deltaTime) <= 0F)
				{
					_stunned = false;
					Rigidbody.WakeUp();
				}
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!_statistics.NoHit && other.TryGetComponent<IDestructible>(out var destructible) && destructible.Hurt(_statistics.Damage))
			{
				destructible.Stun(_statistics.Damage, _statistics.StunTime);
				_screenShaker.GenerateImpulse(_statistics.HurtShake);
				EffectsController.HitStop(_statistics.HitStopTime, _statistics.HitSlowTime);
			}
		}
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
		public void Receive(MessageData message)
		{
			if (message.Format == MessageFormat.None && message.ToggleValue.HasValue)
			{
				Rigidbody.Sleep();
				foreach (EnemyProvider enemy in _selfEnemies)
					enemy.enabled = message.ToggleValue.Value;
			}
		}
	};
};
