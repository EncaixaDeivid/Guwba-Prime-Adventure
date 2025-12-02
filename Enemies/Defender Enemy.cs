using UnityEngine;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class DefenderEnemy : EnemyProvider, IConnector, IDestructible
	{
		private bool _invencible = false;
		private float _timeOperation = 0f;
		[Header("Defender Enemy")]
		[SerializeField, Tooltip("The defender statitics of this enemy.")] private DefenderStatistics _statistics;
		private new void Awake()
		{
			base.Awake();
			_sender.SetFormat(MessageFormat.State);
			_timeOperation = _statistics.TimeToInvencible;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void Update()
		{
			if (_stopWorking || IsStunned || !_statistics.UseAlternatedTime && !_invencible)
				return;
			if (_timeOperation > 0f)
				if ((_timeOperation -= Time.deltaTime) <= 0f)
					if (_invencible)
					{
						_invencible = false;
						_timeOperation = _statistics.UseAlternatedTime ? _statistics.TimeToInvencible : _statistics.TimeToDestructible;
						if (_statistics.InvencibleStop)
						{
							_sender.SetToggle(true);
							_sender.Send(MessagePath.Enemy);
						}
					}
					else
					{
						_invencible = true;
						_timeOperation = _statistics.TimeToDestructible;
						if (_statistics.InvencibleStop)
						{
							_sender.SetToggle(false);
							_sender.Send(MessagePath.Enemy);
						}
					}
		}
		public new bool Hurt(ushort damage)
		{
			bool isHurted = false;
			if (!_invencible && damage >= _statistics.BiggerDamage)
				if ((isHurted = base.Hurt(damage)) && _statistics.InvencibleHurted)
				{
					_timeOperation = _statistics.TimeToDestructible;
					_invencible = true;
					if (_statistics.InvencibleStop)
					{
						_sender.SetToggle(true);
						_sender.Send(MessagePath.Enemy);
					}
				}
			return isHurted;
		}
		public void Receive(MessageData message)
		{
			if (message.AdditionalData != null && message.AdditionalData is EnemyProvider[] && (message.AdditionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in message.AdditionalData as EnemyProvider[])
					if (enemy && enemy == this)
					{
						if (message.Format == MessageFormat.Event && _statistics.ReactToDamage && message.ToggleValue.HasValue)
							if (_statistics.UseAlternatedTime && message.ToggleValue.Value)
								(_invencible, _timeOperation) = (true, _statistics.TimeToDestructible);
							else
								(_invencible, _timeOperation) = (message.ToggleValue.Value, _statistics.TimeToDestructible);
						if (_statistics.InvencibleStop)
						{
							_sender.SetToggle(!_invencible);
							_sender.Send(MessagePath.Enemy);
						}
						return;
					}
		}
	};
};
