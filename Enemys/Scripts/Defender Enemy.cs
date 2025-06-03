using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	internal sealed class DefenderEnemy : EnemyController, IConnector
	{
		private readonly Sender _sender = Sender.Create();
		private bool _invencible = false;
		private float _timeInvencible = 0f;
		private float _timeDamageable = 0f;
		[Header("Defender Enemy")]
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private short _biggerDamage;
		[SerializeField, Tooltip("If this enemy will stop moving when become invencible.")] private bool _invencibleStop;
		[SerializeField, Tooltip("If this enemy will become invencible when damaged.")] private bool _invencibleDamaged;
		[SerializeField, Tooltip("If this enemy will use time to become invencible/damageable.")] private bool _useAlternatedTime;
		[SerializeField, Tooltip("The amount of time the enemy will be invencible.")] private float _timeToInvencible;
		[SerializeField, Tooltip("The amount of time the enemy will be damageable.")] private float _timeToDamageable;
		public PathConnection PathConnection => PathConnection.Enemy;
		private new void Awake()
		{
			base.Awake();
			this._sender.SetToWhereConnection(PathConnection.Enemy).SetConnectionState(ConnectionState.State);
			this._sender.SetAdditionalData(this.gameObject);
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void FixedUpdate()
		{
			if (this._stopMovement || this.Paralyzed)
				return;
			if (this._invencible)
			{
				if (this._timeInvencible > 0f)
					this._timeInvencible -= Time.fixedDeltaTime;
				if (this._timeInvencible <= 0f)
				{
					this._invencible = false;
					if (this._useAlternatedTime)
						this._timeDamageable = this._timeToDamageable;
				}
			}
			if (this._useAlternatedTime)
			{
				if (this._timeDamageable > 0f)
					this._timeDamageable -= Time.fixedDeltaTime;
				if (this._timeDamageable <= 0f)
				{
					this._invencible = true;
					this._timeInvencible = this._timeToInvencible;
				}
			}
		}
		public new bool Damage(ushort damage)
		{
			bool isDamaged = false;
			if (!this._invencible && this._timeInvencible <= 0f && this._timeDamageable > 0f && damage >= this._biggerDamage)
				isDamaged = base.Damage(damage);
			if (this._invencibleDamaged && isDamaged)
			{
				this._timeInvencible = this._timeToInvencible;
				this._invencible = true;
				if (this._invencibleStop)
					this._sender.SetToggle(true).Send();
			}
			return isDamaged;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (additionalData as GameObject != this.gameObject)
				return;
			if (data.ConnectionState == ConnectionState.Action && data.ToggleValue.HasValue)
				this._invencible = data.ToggleValue.Value;
			if (this._invencibleStop)
				this._sender.SetToggle(!this._invencible).Send();
		}
	};
};
