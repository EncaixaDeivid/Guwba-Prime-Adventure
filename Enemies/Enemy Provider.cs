using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[RequireComponent(typeof(Transform), typeof(EnemyController))]
	internal abstract class EnemyProvider : StateController, IDestructible
	{
		private EnemyController _controller;
		protected Rigidbody2D _rigidybody;
		protected Collider2D _collider;
		protected readonly Sender _sender = Sender.Create();
		protected bool _stopWorking = false;
		[Header("Enemy Provider")]
		[SerializeField, Tooltip("The enemies to send messages.")] private EnemyProvider[] _enemiesToSend;
		[SerializeField, Tooltip("The level of priority to use the destructible side.")] private ushort _destructilbePriority = 0;
		public PathConnection PathConnection => PathConnection.Enemy;
		protected bool IsStunned => _controller.IsStunned;
		public short Health => _controller.Health;
		internal ushort DestructilbePriority => _destructilbePriority;
		protected new void Awake()
		{
			base.Awake();
			_controller = GetComponent<EnemyController>();
			_rigidybody = GetComponent<Rigidbody2D>();
			_collider = GetComponent<Collider2D>();
			_sender.SetAdditionalData(_enemiesToSend);
		}
		public bool Hurt(ushort damage)
		{
			if (_controller.ProvidenceStatistics.ReactToDamage)
			{
				if (_controller.ProvidenceStatistics.HasIndex)
					_sender.SetNumber(_controller.ProvidenceStatistics.IndexEvent);
				_sender.SetStateForm(StateForm.Action);
				_sender.Send(PathConnection.Enemy);
			}
			if ((_controller.Vitality -= (short)damage) <= 0f)
				Destroy(gameObject);
			return true;
		}
		public void Stun(ushort stunStength, float stunTime)
		{
			_controller.StunTimer = stunTime;
			_controller.IsStunned = true;
			_rigidybody.Sleep();
			if ((_controller.ArmorResistance -= (short)stunStength) <= 0f)
			{
				_controller.StunTimer = _controller.ProvidenceStatistics.StunnedTime;
				_controller.ArmorResistance = (short)_controller.ProvidenceStatistics.HitResistance;
			}
		}
	};
};
