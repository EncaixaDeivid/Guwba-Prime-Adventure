using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(Collider2D), typeof(Receptor))]
	internal sealed class Teleporter : StateController, Receptor.IReceptor, IInteractable
	{
		private Collider2D _collider;
		private Coroutine _timerCoroutine;
		private readonly Sender _sender = Sender.Create();
		private ushort _index = 0;
		private bool _active = false;
		[Header("Teleporter")]
		[SerializeField, Tooltip("The locations that Guwba can teleport to.")] private Vector2[] _locations;
		[SerializeField, Tooltip("If it have to interact to teleport.")] private bool _isInteractive;
		[SerializeField, Tooltip("Everyone can use the teleporter.")] private bool _everyone;
		[SerializeField, Tooltip("If it teleports at the touch.")] private bool _onCollision;
		[SerializeField, Tooltip("If it have to receive a signal to work.")] private bool _isReceptor;
		[SerializeField, Tooltip("If it have to waits to teleport.")] private bool _useTimer;
		[SerializeField, Tooltip("The amount of time it have to waits to teleport.")] private float _timeToUse;
		private new void Awake()
		{
			base.Awake();
			this._collider = this.GetComponent<Collider2D>();
			this._sender.SetStateForm(StateForm.Action);
			this._sender.SetAdditionalData(this.gameObject);
			this._active = !this._isReceptor;
		}
		private IEnumerator Timer(bool activeValue)
		{
			yield return new WaitTime(this, this._timeToUse);
			this._active = activeValue;
			this._timerCoroutine = null;
		}
		private IEnumerator Timer()
		{
			this._sender.SetToWhereConnection(PathConnection.Item);
			this._sender.SetToggle(false);
			this._sender.Send();
			yield return new WaitTime(this, this._timeToUse);
			foreach (Collider2D collider in Physics2D.OverlapBoxAll(this.transform.position, this._collider.bounds.size, 0f))
				if (this._everyone)
				{
					collider.transform.position = this._locations[this._index];
					break;
				}
				else
				{
					GuwbaAstral<CommandGuwba>.Position = this._locations[this._index];
					this._sender.SetToWhereConnection(PathConnection.Guwba);
					this._sender.SetToggle(false);
					this._sender.Send();
					break;
				}
			this._sender.SetToWhereConnection(PathConnection.Item);
			this._sender.SetToggle(true);
			this._sender.Send();
		}
		public void Execute()
		{
			this._active = !this._active;
			if (this._useTimer)
			{
				if (this._timerCoroutine == null)
					this._timerCoroutine = this.StartCoroutine(this.Timer(this._active));
				else
					this.StopCoroutine(this._timerCoroutine);
			}
		}
		public void Interaction()
		{
			if (this._active && this._isInteractive && this._useTimer)
				this.StartCoroutine(this.Timer());
			else if (this._active && this._isInteractive)
			{
				GuwbaAstral<CommandGuwba>.Position = this._locations[this._index];
				this._sender.SetToWhereConnection(PathConnection.Guwba);
				this._sender.SetToggle(false);
				this._sender.Send();
			}
			this._index = (ushort)(this._index < this._locations.Length - 1f ? this._index + 1f : 0f);
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._active && this._onCollision && this._useTimer)
				this.StartCoroutine(this.Timer());
			else if (this._active && this._onCollision && this._everyone)
				other.transform.position = this._locations[this._index];
			else if (this._active && this._onCollision && GuwbaAstral<CommandGuwba>.EqualObject(other.gameObject))
			{
				GuwbaAstral<CommandGuwba>.Position = this._locations[this._index];
				this._sender.SetToWhereConnection(PathConnection.Guwba);
				this._sender.SetToggle(false);
				this._sender.Send();
			}
			this._index = (ushort)(this._index < this._locations.Length - 1f ? this._index + 1f : 0f);
		}
	};
};
