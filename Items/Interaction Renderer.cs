using UnityEngine;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(CircleCollider2D))]
	internal sealed class InteractionRenderer : StateController, IImageComponents, IConnector
	{
		private IImagePool _imagePool;
		private bool _isActive = true;
		private bool _isOnCollision = false;
		[Header("Image Components")]
		[SerializeField, Tooltip("The image it will show.")] private Sprite _image;
		[SerializeField, Tooltip("The offset of the image.")] private Vector2 _imageOffset;
		public Sprite Image => this._image;
		public Vector2 ImageOffset => this._imageOffset;
		public PathConnection PathConnection => PathConnection.Item;
		private new void Awake()
		{
			base.Awake();
			this._imagePool = EffectsController.CreateImageRenderer(this);
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if ((this._isOnCollision = CentralizableGuwba.EqualObject(collision.gameObject)) && this._isActive)
				this._imagePool.Pull();
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (!(this._isOnCollision = !CentralizableGuwba.EqualObject(collision.gameObject)))
				this._imagePool.Push();
		}
		public void Receive(DataConnection data, object additionalData)
		{
			GameObject gameObject = additionalData as GameObject;
			if (gameObject == this.gameObject && data.StateForm == StateForm.Action && data.ToggleValue.HasValue)
				if (data.ToggleValue.Value)
				{
					this._isActive = true;
					if (this._isOnCollision)
						this._imagePool.Pull();
				}
				else
				{
					this._isActive = false;
					this._imagePool.Push();
				}
		}
	};
};
