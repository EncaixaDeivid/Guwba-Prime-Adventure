using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Connection;
using GwambaPrimeAdventure.Character;
namespace GwambaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D), typeof(Receptor))]
	internal sealed class Teleporter : StateController, IReceptorSignal, IInteractable
	{
		private Coroutine _timerCoroutine;
		private readonly Sender _sender = Sender.Create();
		private ushort _index = 0;
		private bool _active = false;
		[Header("Teleporter")]
		[SerializeField, Tooltip("The locations that Guwba can teleport to.")] private Vector2[] _locations;
		[SerializeField, Tooltip("If it have to interact to teleport.")] private bool _isInteractive;
		[SerializeField, Tooltip("If it teleports at the touch.")] private bool _onCollision;
		[SerializeField, Tooltip("If it have to receive a signal to work.")] private bool _isReceptor;
		[SerializeField, Tooltip("If it have to waits to teleport.")] private bool _useTimer;
		[SerializeField, Tooltip("The amount of time it have to waits to teleport.")] private float _timeToUse;
		private new void Awake()
		{
			base.Awake();
			_active = !_isReceptor;
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
		private IEnumerator Timer(bool activeValue)
		{
			yield return new WaitTime(this, _timeToUse);
			_active = activeValue;
			_timerCoroutine = null;
		}
		private IEnumerator Timer()
		{
			_sender.SetStateForm(StateForm.State);
			_sender.SetAdditionalData(gameObject);
			_sender.SetToggle(false);
			_sender.Send(PathConnection.Hud);
			yield return new WaitTime(this, _timeToUse);
			Teleport();
			_sender.SetStateForm(StateForm.State);
			_sender.SetAdditionalData(gameObject);
			_sender.SetToggle(true);
			_sender.Send(PathConnection.Hud);
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_active && _onCollision && _useTimer)
				StartCoroutine(Timer());
			else if (_active && _onCollision && GwambaStateMarker.EqualObject(other.gameObject))
				Teleport();
		}
		public void Execute()
		{
			_active = !_active;
			if (_useTimer)
				if (_timerCoroutine == null)
					_timerCoroutine = StartCoroutine(Timer(_active));
				else
					StopCoroutine(_timerCoroutine);
			else
				Teleport();
		}
		public void Interaction()
		{
			if (_active && _isInteractive && _useTimer)
				StartCoroutine(Timer());
			else if (_active && _isInteractive)
				Teleport();
		}
	};
};
