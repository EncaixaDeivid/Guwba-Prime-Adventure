using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Camera), typeof(SortingGroup))]
	internal sealed class BackgroundController : StateController
	{
		private static BackgroundController _instance;
		private Transform[] _childrenTransforms;
		private SpriteRenderer[] _childrenRenderers;
		private Vector2 _startPosition = Vector2.zero;
		[Header("Background Objects")]
		[SerializeField, Tooltip("The object that handles the backgrounds.")] private Transform _backgroundTransform;
		[SerializeField, Tooltip("The handler of the background.")] private SpriteAtlas _backgroundHandler;
		[SerializeField, Tooltip("The name of the images that are placed in each background.")] private string[] _backgroundImages;
		[Header("Background Stats")]
		[SerializeField, Tooltip("The amount of speed that the background will move horizontally.")] private float _horizontalBackgroundSpeed;
		[SerializeField, Tooltip("The amount of speed that the background will move vertically.")] private float _verticalBackgroundSpeed;
		[SerializeField, Tooltip("The amount to slow horizontally for each layer that is after the first.")] private float _slowHorizontal;
		[SerializeField, Tooltip("The amount to slow vertically for each layer that is after the first.")] private float _slowVertical;
		private new void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			base.Awake();
			_childrenTransforms = new Transform[_backgroundImages.Length];
			_childrenRenderers = new SpriteRenderer[_backgroundImages.Length];
			for (ushort ia = 0; ia < _backgroundImages.Length; ia++)
			{
				_childrenTransforms[ia] = Instantiate(_backgroundTransform, transform);
				_childrenRenderers[ia] = _childrenTransforms[ia].GetComponent<SpriteRenderer>();
				Sprite sprite = _backgroundHandler.GetSprite(_backgroundImages[ia]);
				_childrenRenderers[ia].sprite = sprite;
				_childrenRenderers[ia].sortingOrder = _backgroundImages.Length - 1 - ia;
				_childrenTransforms[ia].GetComponent<SortingGroup>().sortingOrder = _childrenRenderers[ia].sortingOrder;
				float centerX = _childrenTransforms[ia].position.x;
				float centerY = _childrenTransforms[ia].position.y;
				Vector2 imageSize = _childrenRenderers[ia].sprite.bounds.size;
				float right = centerX + imageSize.x;
				float left = centerX - imageSize.x;
				float top = centerY + imageSize.y;
				float bottom = centerY - imageSize.y;
				_childrenTransforms[ia].GetChild(0).position = new Vector2(left, top);
				_childrenTransforms[ia].GetChild(1).position = new Vector2(centerX, top);
				_childrenTransforms[ia].GetChild(2).position = new Vector2(right, top);
				_childrenTransforms[ia].GetChild(3).position = new Vector2(left, centerY);
				_childrenTransforms[ia].GetChild(4).position = new Vector2(right, centerY);
				_childrenTransforms[ia].GetChild(5).position = new Vector2(left, bottom);
				_childrenTransforms[ia].GetChild(6).position = new Vector2(centerX, bottom);
				_childrenTransforms[ia].GetChild(7).position = new Vector2(right, bottom);
				for (ushort ib = 0; ib < _childrenTransforms[ia].childCount; ib++)
					_childrenTransforms[ia].GetChild(ib).GetComponent<SpriteRenderer>().sprite = sprite;
			}
		}
		private void FixedUpdate()
		{
			for (ushort i = 0; i < _backgroundImages.Length; i++)
			{
				float axisX = 1f - (_horizontalBackgroundSpeed - (i * _slowHorizontal));
				float axisY = 1f - (_verticalBackgroundSpeed - (i * _slowVertical));
				float movementAxisX = transform.position.x * axisX;
				float movementAxisY = transform.position.y * axisY;
				_childrenTransforms[i].position = new Vector2(_startPosition.x + movementAxisX, _startPosition.y + movementAxisY);
				Vector2 imageSize = _childrenRenderers[i].sprite.bounds.size;
				float distanceAxisX = transform.position.x * (1f - axisX);
				float distanceAxisY = transform.position.y * (1f - axisY);
				if (distanceAxisX > _startPosition.x + imageSize.x)
					_startPosition = new Vector2(_startPosition.x + imageSize.x, _startPosition.y);
				else if (distanceAxisX < _startPosition.x - imageSize.x)
					_startPosition = new Vector2(_startPosition.x - imageSize.x, _startPosition.y);
				if (distanceAxisY > _startPosition.y + imageSize.y)
					_startPosition = new Vector2(_startPosition.x, _startPosition.y + imageSize.y);
				else if (distanceAxisY < _startPosition.y - imageSize.y)
					_startPosition = new Vector2(_startPosition.x, _startPosition.y - imageSize.y);
			}
		}
	};
};
