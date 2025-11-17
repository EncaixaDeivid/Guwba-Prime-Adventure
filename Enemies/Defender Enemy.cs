using UnityEngine;
using GwambaPrimeAdventure.Connection;
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
			_sender.SetStateForm(StateForm.State);
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
				_timeOperation -= Time.deltaTime;
			if (_timeOperation <= 0f)
			{
				if (_invencible)
				{
					_invencible = false;
					_timeOperation = _statistics.TimeToInvencible;
					if (_statistics.InvencibleStop)
					{
						_sender.SetToggle(true);
						_sender.Send(PathConnection.Enemy);
					}
				}
				else
				{
					_invencible = true;
					_timeOperation = _statistics.TimeToDestructible;
					if (_statistics.InvencibleStop)
					{
						_sender.SetToggle(false);
						_sender.Send(PathConnection.Enemy);
					}
				}
			}
		}
		public new bool Hurt(ushort damage)
		{
			bool isHurted = false;
			if (!_invencible && damage >= _statistics.BiggerDamage)
				if (_statistics.InvencibleHurted && (isHurted = base.Hurt(damage)))
				{
					_timeOperation = _statistics.TimeToDestructible;
					_invencible = true;
					if (_statistics.InvencibleStop)
					{
						_sender.SetToggle(true);
						_sender.Send(PathConnection.Enemy);
					}
				}
			return isHurted;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (additionalData != null || additionalData is EnemyProvider[] || additionalData as EnemyProvider[] != null || (additionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in additionalData as EnemyProvider[])
					if (enemy == this)
					{
						if (data.StateForm == StateForm.Event && data.ToggleValue.HasValue)
							if (_statistics.UseAlternatedTime && data.ToggleValue.Value)
								_invencible = true;
							else
								_invencible = data.ToggleValue.Value;
						if (_statistics.InvencibleStop)
						{
							_sender.SetToggle(!_invencible);
							_sender.Send(PathConnection.Enemy);
						}
						return;
					}
		}
	};
};
