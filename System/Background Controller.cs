using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using System.Collections;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Camera), typeof(SortingGroup))]
	internal sealed class BackgroundController : StateController, ILoader
	{
		private static BackgroundController _instance;
		[Header("Background Interaction")]
		[SerializeField, Tooltip("The object that handles the backgrounds.")] private BackgroundMover _backgroundMover;
		[SerializeField, Tooltip("The handler of the background.")] private SpriteAtlas _backgroundHandler;
		[SerializeField, Tooltip("The amount of speed that the background will move.")] private Vector2 _backgroundSpeed;
		[SerializeField, Tooltip("The amount to slow for each layer that is after the first.")] private Vector2 _slowSpeed;
		[SerializeField, Tooltip("The offset of the camera relative to center of the screen.")] private Vector2 _positionOffset;
		[SerializeField, Tooltip("The name of the images that are placed in each background.")] private string[] _backgroundImages;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(gameObject, WorldBuild.MINIMUM_TIME_SPACE_LIMIT);
				return;
			}
			_instance = this;
		}
		public IEnumerator Load()
		{
			BackgroundMover childBackground;
			Transform child;
			SpriteRenderer renderer;
			SpriteRenderer childRenderer;
			Vector3 childOffset = Vector2.zero;
			ushort childIndex;
			for (ushort i = 0; i < _backgroundImages.Length; i++)
			{
				childBackground = Instantiate(_backgroundMover, transform);
				childBackground.transform.position = (Vector2)childBackground.transform.position + _positionOffset;
				renderer = childBackground.GetComponent<SpriteRenderer>();
				renderer.sprite = _backgroundHandler.GetSprite(_backgroundImages[i]);
				renderer.sortingOrder = _backgroundImages.Length - 1 - i;
				childBackground.GetComponent<SortingGroup>().sortingOrder = renderer.sortingOrder;
				childIndex = 0;
				for (short x = -1; x <= 1; x++)
					for (short y = -1; y <= 1; y++)
						if (x != 0 || y != 0)
						{
							child = childBackground.transform.GetChild(childIndex++);
							childOffset.Set(renderer.bounds.size.x * x, renderer.bounds.size.y * y, 0F);
							child.position += childOffset;
							childRenderer = child.GetComponent<SpriteRenderer>();
							childRenderer.sprite = renderer.sprite;
							childRenderer.sortingOrder = renderer.sortingOrder;
						}
				childBackground.SetBackground(_backgroundSpeed - i * _slowSpeed, renderer.bounds.size, _positionOffset);
			}
			yield return null;
		}
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			transform.DetachChildren();
		}
	};
};
