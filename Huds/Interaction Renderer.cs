using UnityEngine;
using UnityEngine.UIElements;
using GwambaPrimeAdventure.Connection;
using GwambaPrimeAdventure.Character;
namespace GwambaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D), typeof(IInteractable))]
	internal sealed class InteractionRenderer : StateController, IConnector
	{
		private Animator _animator;
		private UIDocument _document;
		private readonly int IsOn = Animator.StringToHash(nameof(IsOn));
		private bool _isActive = true;
		private bool _isOnCollision = false;
		[Header("Interaction Components")]
		[SerializeField, Tooltip("The UI document of the interaction.")] private UIDocument _documentObject;
		[SerializeField, Tooltip("The offset of the document of interaction.")] private Vector2 _imageOffset;
		public PathConnection PathConnection => PathConnection.Hud;
		private new void Awake()
		{
			base.Awake();
			_animator = GetComponent<Animator>();
			_document = Instantiate(_documentObject, transform);
			_document.enabled = false;
			_document.transform.localPosition = _imageOffset;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void OnEnable()
		{
			if (_animator)
				_animator.SetFloat(IsOn, 1f);
		}
		private void OnDisable()
		{
			if (_animator)
				_animator.SetFloat(IsOn, 0f);
		}
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if ((_isOnCollision = GwambaStateMarker.EqualObject(collision.gameObject)) && _isActive)
				_document.enabled = true;
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (!(_isOnCollision = !GwambaStateMarker.EqualObject(collision.gameObject)))
				_document.enabled = false;
		}
		public void Receive(DataConnection data)
		{
			if (data.AdditionalData as GameObject == gameObject && data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				if (data.ToggleValue.Value)
				{
					_isActive = true;
					if (_isOnCollision)
						_document.enabled = true;
				}
				else
					_isActive = _document.enabled = false;
		}
	};
};
