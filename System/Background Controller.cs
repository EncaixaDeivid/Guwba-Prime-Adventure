using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using System.Collections;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Camera), typeof(SortingGroup))]
	internal sealed class BackgroundController : StateController
	{
		private static BackgroundController _instance;
		private Transform[] _childrenTransforms;
		private SpriteRenderer[] _childrenRenderers;
		private Vector2 _startPosition = Vector2.zero;
		private bool _isInTrancision = false;
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
			base.Awake();
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			_childrenTransforms = new Transform[_backgroundImages.Length];
			_childrenRenderers = new SpriteRenderer[_backgroundImages.Length];
		}
		private IEnumerator Start()
		{
			_isInTrancision = true;
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			Sprite sprite;
			Vector2 imageSize;
			float centerX;
			float centerY;
			float right;
			float left;
			float top;
			float bottom;
			for (ushort ia = 0; ia < _backgroundImages.Length; ia++)
			{
				_childrenTransforms[ia] = Instantiate(_backgroundTransform, transform);
				_childrenRenderers[ia] = _childrenTransforms[ia].GetComponent<SpriteRenderer>();
				sprite = _backgroundHandler.GetSprite(_backgroundImages[ia]);
				_childrenRenderers[ia].sprite = sprite;
				_childrenRenderers[ia].sortingOrder = _backgroundImages.Length - 1 - ia;
				_childrenTransforms[ia].GetComponent<SortingGroup>().sortingOrder = _childrenRenderers[ia].sortingOrder;
				centerX = _childrenTransforms[ia].position.x;
				centerY = _childrenTransforms[ia].position.y;
				imageSize = _childrenRenderers[ia].sprite.bounds.size;
				right = centerX + imageSize.x;
				left = centerX - imageSize.x;
				top = centerY + imageSize.y;
				bottom = centerY - imageSize.y;
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
			_isInTrancision = false;
		}
		private void FixedUpdate()
		{
			if (_isInTrancision)
				return;
			Vector2 imageSize;
			float axisX;
			float axisY;
			for (ushort i = 0; i < _backgroundImages.Length; i++)
			{
				axisX = 1f - (_horizontalBackgroundSpeed - (i * _slowHorizontal));
				axisY = 1f - (_verticalBackgroundSpeed - (i * _slowVertical));
				_childrenTransforms[i].position = new Vector2(_startPosition.x + transform.position.x * axisX, _startPosition.y + transform.position.y * axisY);
				imageSize = _childrenRenderers[i].sprite.bounds.size;
				if (transform.position.x * (1f - axisX) > _startPosition.x + imageSize.x)
					_startPosition = new Vector2(_startPosition.x + imageSize.x, _startPosition.y);
				else if (transform.position.x * (1f - axisX) < _startPosition.x - imageSize.x)
					_startPosition = new Vector2(_startPosition.x - imageSize.x, _startPosition.y);
				if (transform.position.y * (1f - axisY) > _startPosition.y + imageSize.y)
					_startPosition = new Vector2(_startPosition.x, _startPosition.y + imageSize.y);
				else if (transform.position.y * (1f - axisY) < _startPosition.y - imageSize.y)
					_startPosition = new Vector2(_startPosition.x, _startPosition.y - imageSize.y);
			}
		}
	};
};
