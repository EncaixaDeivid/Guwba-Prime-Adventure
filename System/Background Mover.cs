using UnityEngine;
using UnityEngine.Rendering;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(SortingGroup))]
	internal sealed class BackgroundMover : StateController
	{
		private BackgroundController _backgroundController;
		private Vector2 _startPosition = Vector2.zero;
		private Vector2 _movementSpeed = Vector2.zero;
		private Vector2 _movement = Vector2.zero;
		private Vector2 _imageSize = Vector2.zero;
		private Vector2 _positionOffset;
		internal void SetBackground(Vector2 movementSpeed, Vector2 imageSize, Vector2 positionOffset)
		{
			_movementSpeed = movementSpeed;
			_imageSize = imageSize;
			_positionOffset = positionOffset;
		}
		private new void Awake()
		{
			base.Awake();
			_backgroundController = GetComponentInParent<BackgroundController>();
			_startPosition = transform.position;
		}
		private void LateUpdate()
		{
			if (SceneInitiator.IsInTrancision())
				return;
			_movement.Set((_backgroundController.transform.position.x + _positionOffset.x) * (1F - _movementSpeed.x), (_backgroundController.transform.position.y + _positionOffset.y) * (1F + _movementSpeed.y));
			transform.position = _startPosition + _movement;
			if (_backgroundController.transform.position.x * _movementSpeed.x > _startPosition.x + _imageSize.x)
				_startPosition.x += _imageSize.x;
			else if (_backgroundController.transform.position.x * _movementSpeed.x < _startPosition.x - _imageSize.x)
				_startPosition.x -= _imageSize.x;
			if (_backgroundController.transform.position.y * _movementSpeed.y > _startPosition.y + _imageSize.y)
				_startPosition.y += _imageSize.y;
			else if (_backgroundController.transform.position.y * _movementSpeed.y < _startPosition.y - _imageSize.y)
				_startPosition.y -= _imageSize.y;
		}
	};
};