using UnityEngine;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(CircleCollider2D))]
	[RequireComponent(typeof(IInteractable))]
	internal sealed class InteractionRenderer : StateController, IImageComponents
	{
		private IImagePool _imagePool;
		[Header("Props")]
		[SerializeField, Tooltip("The image it will show.")] private Sprite _image;
		[SerializeField, Tooltip("The offset of the image.")] private Vector2 _imageOffset;
		public Sprite Image => this._image;
		public Vector2 ImageOffset => this._imageOffset;
		private new void Awake()
		{
			base.Awake();
			this._imagePool = EffectsController.CreateImageRenderer(this);
		}
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (GuwbaAstral<CommandGuwba>.EqualObject(collision.gameObject))
				this._imagePool.Pull();
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (GuwbaAstral<CommandGuwba>.EqualObject(collision.gameObject))
				this._imagePool.Push();
		}
	};
};
