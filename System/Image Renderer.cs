using UnityEngine;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer))]
	internal sealed class ImageRenderer : MonoBehaviour, IImagePool
	{
		private SpriteRenderer _spriteRenderer;
		private void Awake() => (this._spriteRenderer = this.GetComponent<SpriteRenderer>()).enabled = false;
		public void Pull() => this._spriteRenderer.enabled = true;
		public void Push() => this._spriteRenderer.enabled = false;
	};
	public interface IImageComponents
	{
		public Sprite Image { get; }
		public Vector2 ImageOffset { get; }
	};
	public interface IImagePool
	{
		public void Pull();
		public void Push();
	};
};