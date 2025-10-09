using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
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
			this._sender.SetStateForm(StateForm.State);
			this._timeOperation = this._statistics.TimeToInvencible;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void Update()
		{
			if (this._stopWorking || this.IsStunned || !this._statistics.UseAlternatedTime && !this._invencible)
				return;
			if (this._timeOperation > 0f)
				this._timeOperation -= Time.deltaTime;
			if (this._timeOperation <= 0f)
			{
				if (this._invencible)
				{
					this._invencible = false;
					this._timeOperation = this._statistics.TimeToInvencible;
					if (this._statistics.InvencibleStop)
					{
						this._sender.SetToggle(true);
						this._sender.Send(PathConnection.Enemy);
					}
				}
				else
				{
					this._invencible = true;
					this._timeOperation = this._statistics.TimeToDestructible;
					if (this._statistics.InvencibleStop)
					{
						this._sender.SetToggle(false);
						this._sender.Send(PathConnection.Enemy);
					}
				}
			}
		}
		public new bool Hurt(ushort damage)
		{
			bool isHurted = false;
			if (!this._invencible && damage >= this._statistics.BiggerDamage)
				isHurted = base.Hurt(damage);
			if (this._statistics.InvencibleHurted && isHurted)
			{
				this._timeOperation = this._statistics.TimeToDestructible;
				this._invencible = true;
				if (this._statistics.InvencibleStop)
				{
					this._sender.SetToggle(true);
					this._sender.Send(PathConnection.Enemy);
				}
			}
			return isHurted;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
				if (enemy != this)
					return;
			if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue)
				if (this._statistics.UseAlternatedTime && data.ToggleValue.Value)
					this._invencible = true;
				else
					this._invencible = data.ToggleValue.Value;
			if (this._statistics.InvencibleStop)
			{
				this._sender.SetToggle(!this._invencible);
				this._sender.Send(PathConnection.Enemy);
			}
		}
	};
};
