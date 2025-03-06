using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(Collider2D), typeof(Receptor))]
	internal sealed class Teleporter : StateController, Receptor.IReceptor, IInteractable
	{
		private Collider2D _collider;
		private ushort _index = 0;
		private bool _active = true;
		[SerializeField] private Vector2[] _locations;
		[SerializeField] private bool _everyone, _isInteractive, _onCollision, _useTimer, _isReceptor;
		[SerializeField] private float _timeToUse;
		private new void Awake()
		{
			base.Awake();
			this._collider = this.GetComponent<Collider2D>();
			this._active = !this._isReceptor;
		}
		private IEnumerator Timer(bool activeValue)
		{
			yield return new WaitTime(this, this._timeToUse);
			this._active = activeValue;
		}
		private IEnumerator Timer()
		{
			yield return new WaitTime(this, this._timeToUse);
			foreach (Collider2D collider in Physics2D.OverlapBoxAll(this.transform.position, this._collider.bounds.extents * 2f, 0f))
				if (this._everyone)
				{
					collider.transform.position = this._locations[this._index];
					break;
				}
				else
				{
					GuwbaTransformer<CommandGuwba>.Position = this._locations[this._index];
					break;
				}
		}
		public void ActivationEvent()
		{
			if (this._useTimer)
				this.StartCoroutine(this.Timer(true));
			else
				this._active = true;
		}
		public void DesactivationEvent()
		{
			if (this._useTimer)
				this.StartCoroutine(this.Timer(false));
			else
				this._active = false;
		}
		public void Interaction()
		{
			if (this._active && this._isInteractive && this._useTimer)
				this.StartCoroutine(this.Timer());
			else if (this._active && this._isInteractive)
				GuwbaTransformer<CommandGuwba>.Position = this._locations[this._index];
			this._index = (ushort)(this._index < this._locations.Length - 1f ? this._index + 1f : 0f);
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._active && this._onCollision && this._useTimer)
				this.StartCoroutine(this.Timer());
			else if (this._active && this._onCollision && this._everyone)
				other.transform.position = this._locations[this._index];
			else if (this._active && this._onCollision && GuwbaTransformer<CommandGuwba>.EqualObject(other.gameObject))
				GuwbaTransformer<CommandGuwba>.Position = this._locations[this._index];
			this._index = (ushort)(this._index < this._locations.Length - 1f ? this._index + 1f : 0f);
		}
	};
};