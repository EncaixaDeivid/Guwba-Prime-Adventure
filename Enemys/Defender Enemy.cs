using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	internal sealed class DefenderEnemy : EnemyController, IConnector, IDestructible
	{
		private readonly Sender _sender = Sender.Create();
		private bool _invencible = false;
		private float _timeOperation = 0f;
		[Header("Defender Enemy")]
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private short _biggerDamage;
		[SerializeField, Tooltip("If this enemy will stop moving when become invencible.\nRequires: Ground/Flying Enemy")]
		private bool _invencibleStop;
		[SerializeField, Tooltip("If this enemy will become invencible when damaged.")] private bool _invencibleDamaged;
		[SerializeField, Tooltip("If this enemy will use time to become invencible/damageable.")] private bool _useAlternatedTime;
		[SerializeField, Tooltip("The amount of time the enemy have to become damageable.")] private float _timeToDamageable;
		[SerializeField, Tooltip("The amount of time the enemy have to become invencible.")] private float _timeToInvencible;
		private new void Awake()
		{
			base.Awake();
			this._sender.SetToWhereConnection(PathConnection.Enemy);
			this._sender.SetStateForm(StateForm.State);
			this._sender.SetAdditionalData(this.gameObject);
			this._timeOperation = this._timeToInvencible;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private new void Update()
		{
			base.Update();
			if (this._stopMovement || this.IsStunned || this.DoNotWork || !this._useAlternatedTime && !this._invencible)
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
					this._timeOperation = this._timeToDamageable;
					if (this._invencibleStop)
					{
						this._sender.SetToggle(false);
						this._sender.Send();
					}
				}
			}
		}
		public new bool Damage(ushort damage)
		{
			bool isDamaged = false;
			if (!this._invencible && damage >= this._biggerDamage)
				isDamaged = base.Damage(damage);
			if (this._invencibleDamaged && isDamaged)
			{
				this._timeOperation = this._timeToDamageable;
				this._invencible = true;
				if (this._invencibleStop)
				{
					this._sender.SetToggle(true);
					this._sender.Send();
				}
			}
			return isDamaged;
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
