using UnityEngine;
using NaughtyAttributes;
using GwambaPrimeAdventure.Connection;
using GwambaPrimeAdventure.Character;
namespace GwambaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D), typeof(Receptor))]
	internal sealed class Teleporter : StateController, IReceptorSignal, IInteractable
	{
		private readonly Sender _sender = Sender.Create();
		private ushort _index = 0;
		private float _timer = 0f;
		private bool _active = false;
		private bool _use = false;
		private bool _returnActive;
		[Header("Teleporter")]
		[SerializeField, Tooltip("The locations that Guwba can teleport to.")] private Vector2[] _locations;
		[SerializeField, Tooltip("If it have to interact to teleport.")] private bool _isInteractive;
		[SerializeField, Tooltip("If it have to receive a signal to work.")] private bool _isReceptor;
		[SerializeField, Tooltip("If it teleports at the touch.")] private bool _onCollision;
		[SerializeField, Tooltip("If it have to waits to teleport.")] private bool _useTimer;
		[SerializeField, ShowIf(nameof(_useTimer)), Tooltip("The amount of time it have to waits to teleport.")] private float _timeToUse;
		private new void Awake()
		{
			base.Awake();
			_active = !_isReceptor;
		}
		private void Update()
		{
			if (_timer < 0f)
				if ((_timer -= Time.deltaTime) <= 0f)
					if (_use)
					{
						_use = false;
						Teleport();
						_sender.SetStateForm(StateForm.State);
						_sender.SetAdditionalData(gameObject);
						_sender.SetToggle(true);
						_sender.Send(PathConnection.Hud);
					}
					else
						_active = _returnActive;
		}
		private void Teleport()
		{
			_sender.SetStateForm(StateForm.Event);
			_sender.SetAdditionalData(_locations[_index]);
			_sender.SetToggle(false);
			_sender.Send(PathConnection.System);
			_sender.Send(PathConnection.Character);
			_index = (ushort)(_index < _locations.Length - 1f ? _index + 1f : 0f);
		}
		private void Timer()
		{
			_sender.SetStateForm(StateForm.State);
			_sender.SetAdditionalData(gameObject);
			_sender.SetToggle(false);
			_sender.Send(PathConnection.Hud);
			_timer = _timeToUse;
			_use = true;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_active && _onCollision)
				if (_useTimer)
					Timer();
				else if (GwambaStateMarker.EqualObject(other.gameObject))
					Teleport();
		}
		public void Execute()
		{
			_active = !_active;
			if (_useTimer)
				(_timer, _returnActive) = (_timeToUse, !_active);
			else
				Teleport();
		}
		public void Interaction()
		{
			if (_active && _isInteractive)
				if (_useTimer)
					Timer();
				else
					Teleport();
		}
	};
};
