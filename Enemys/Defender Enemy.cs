using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class DefenderEnemy : EnemyController, IConnector, IDestructible
	{
		private bool _invencible = false;
		private float _timeOperation = 0f;
		[Header("Defender Enemy")]
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private short _biggerDamage;
		[SerializeField, Tooltip("If this enemy will stop moving when become invencible.\nRequires: Moving Enemy")]
		private bool _invencibleStop;
		[SerializeField, Tooltip("If this enemy will become invencible when hurted.")] private bool _invencibleHurted;
		[SerializeField, Tooltip("If this enemy will use time to become invencible/destructible.")] private bool _useAlternatedTime;
		[SerializeField, Tooltip("The amount of time the enemy have to become destructible.")] private float _timeToDestructible;
		[SerializeField, Tooltip("The amount of time the enemy have to become invencible.")] private float _timeToInvencible;
		private new void Awake()
		{
			base.Awake();
			this._sender.SetStateForm(StateForm.State);
			this._timeOperation = this._timeToInvencible;
		}
		private new void Update()
		{
			base.Update();
			if (this._stopWorking || this.IsStunned || !this._useAlternatedTime && !this._invencible)
				return;
			if (this._timeOperation > 0f)
				this._timeOperation -= Time.deltaTime;
			if (this._timeOperation <= 0f)
			{
				if (this._invencible)
				{
					this._invencible = false;
					this._timeOperation = this._timeToInvencible;
					if (this._invencibleStop)
					{
						this._sender.SetToggle(true);
						this._sender.Send();
					}
				}
				else
				{
					this._invencible = true;
					this._timeOperation = this._timeToDestructible;
					if (this._invencibleStop)
					{
						this._sender.SetToggle(false);
						this._sender.Send();
					}
				}
			}
		}
		public new bool Hurt(ushort damage)
		{
			bool isHurted = false;
			if (!this._invencible && damage >= this._biggerDamage)
				isHurted = base.Hurt(damage);
			if (this._invencibleHurted && isHurted)
			{
				this._timeOperation = this._timeToDestructible;
				this._invencible = true;
				if (this._invencibleStop)
				{
					this._sender.SetToggle(true);
					this._sender.Send();
				}
			}
			return isHurted;
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			if (additionalData as GameObject != this.gameObject)
				return;
			if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue)
				if (this._useAlternatedTime && data.ToggleValue.Value)
					this._invencible = true;
				else
					this._invencible = data.ToggleValue.Value;
			if (this._invencibleStop)
			{
				this._sender.SetToggle(!this._invencible);
				this._sender.Send();
			}
		}
	};
};
