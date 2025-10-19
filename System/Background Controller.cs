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
		private bool _isInTrancision = false;
		private float _startPosition = 0f;
		[Header("Background Objects")]
		[SerializeField, Tooltip("The object that handles the backgrounds.")] private Transform _backgroundTransform;
		[SerializeField, Tooltip("The handler of the background.")] private SpriteAtlas _backgroundHandler;
		[SerializeField, Tooltip("The name of the images that are placed in each background.")] private string[] _backgroundImages;
		[Header("Background Stats")]
		[SerializeField, Tooltip("The amount of speed that the background will move.")] private float _backgroundSpeed;
		[SerializeField, Tooltip("The amount to slow for each layer that is after the first.")] private float _slowSpeed;
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
			_isInTrancision = true;
		}
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			Vector2 imageSize;
			for (ushort i = 0; i < _backgroundImages.Length; i++)
			{
				_childrenTransforms[i] = Instantiate(_backgroundTransform, transform);
				_childrenRenderers[i] = _childrenTransforms[i].GetComponent<SpriteRenderer>();
				_childrenRenderers[i].sprite = _backgroundHandler.GetSprite(_backgroundImages[i]);
				_childrenRenderers[i].sortingOrder = _backgroundImages.Length - 1 - i;
				_childrenTransforms[i].GetComponent<SortingGroup>().sortingOrder = _childrenRenderers[i].sortingOrder;
				imageSize = _childrenRenderers[i].sprite.bounds.size;
				_childrenTransforms[i].GetChild(0).position = new Vector2(_childrenTransforms[i].position.x - imageSize.x, _childrenTransforms[i].position.y);
				_childrenTransforms[i].GetChild(1).position = new Vector2(_childrenTransforms[i].position.x + imageSize.x, _childrenTransforms[i].position.y);
				for (ushort childIndex = 0; childIndex < _childrenTransforms[i].childCount; childIndex++)
					_childrenTransforms[i].GetChild(childIndex).GetComponent<SpriteRenderer>().sprite = _backgroundHandler.GetSprite(_backgroundImages[i]);
			}
			_isInTrancision = false;
		}
		private void FixedUpdate()
		{
			if (_isInTrancision)
				return;
			float axisX;
			for (ushort i = 0; i < _backgroundImages.Length; i++)
			{
				axisX = 1f - (_backgroundSpeed - (i * _slowSpeed));
				_childrenTransforms[i].position = new Vector2(_startPosition + transform.position.x * axisX, transform.position.y);
				if (transform.position.x * (1f - axisX) > _startPosition + _childrenRenderers[i].sprite.bounds.size.x)
					_startPosition += _childrenRenderers[i].sprite.bounds.size.x;
				else if (transform.position.x * (1f - axisX) < _startPosition - _childrenRenderers[i].sprite.bounds.size.x)
					_startPosition -= _childrenRenderers[i].sprite.bounds.size.x;
			}
		}
	};
};
