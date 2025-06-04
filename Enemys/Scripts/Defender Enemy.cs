using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	internal sealed class DefenderEnemy : EnemyController, IConnector, IDamageable
	{
		private readonly Sender _sender = Sender.Create();
		private bool _invencible = false;
		private float _timeOperation = 0f;
		[Header("Defender Enemy")]
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private short _biggerDamage;
		[SerializeField, Tooltip("If this enemy will stop moving when become invencible.")] private bool _invencibleStop;
		[SerializeField, Tooltip("If this enemy will become invencible when damaged.")] private bool _invencibleDamaged;
		[SerializeField, Tooltip("If this enemy will use time to become invencible/damageable.")] private bool _useAlternatedTime;
		[SerializeField, Tooltip("The amount of time the enemy will be damageable.")] private float _timeToDamageable;
		[SerializeField, Tooltip("The amount of time the enemy will be invencible.")] private float _timeToInvencible;
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
			if ((this._stopMovement || this.Paralyzed) && (!this._useAlternatedTime || this._invencible))
				return;
			if (this._timeOperation > 0f)
				this._timeOperation -= Time.fixedDeltaTime;
			else if (this._timeOperation <= 0f)
			{
				if (this._invencible)
				{
					this._invencible = false;
					this._timeOperation = this._timeToInvencible;
				}
				else if (this._useAlternatedTime)
				{
					this._invencible = true;
					if (this._useAlternatedTime)
						this._timeOperation = this._timeToDamageable;
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
