using UnityEngine;
using UnityEngine.UIElements;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D), typeof(IInteractable))]
	internal sealed class InteractionRenderer : StateController, IConnector
	{
		private Animator _animator;
		private UIDocument _document;
		private readonly int _isOn = Animator.StringToHash("IsOn");
		private bool _isActive = true;
		private bool _isOnCollision = false;
		[Header("Interaction Components")]
		[SerializeField, Tooltip("The UI document of the interaction.")] private UIDocument _documentObject;
		[SerializeField, Tooltip("The offset of the document of interaction.")] private Vector2 _imageOffset;
		public PathConnection PathConnection => PathConnection.Hud;
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
			this._document = Instantiate(this._documentObject, this.transform);
			this._document.enabled = false;
			this._document.transform.localPosition = this._imageOffset;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void OnEnable()
		{
			if (this._animator)
				this._animator.SetFloat(this._isOn, 1f);
		}
		private void OnDisable()
		{
			if (this._animator)
				this._animator.SetFloat(this._isOn, 0f);
		}
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if ((this._isOnCollision = GuwbaCentralizer.EqualObject(collision.gameObject)) && this._isActive)
				this._document.enabled = true;
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (!(this._isOnCollision = !GuwbaCentralizer.EqualObject(collision.gameObject)))
				this._document.enabled = false;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			GameObject gameObject = additionalData as GameObject;
			if (gameObject == this.gameObject && data.StateForm == StateForm.Action && data.ToggleValue.HasValue)
				if (data.ToggleValue.Value)
				{
					this._isActive = true;
					if (this._isOnCollision)
						this._document.enabled = true;
				}
				else
				{
					this._isActive = false;
					this._document.enabled = false;
				}
		}
	};
};
