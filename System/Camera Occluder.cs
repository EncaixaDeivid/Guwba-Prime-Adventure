using UnityEngine;
using Unity.Cinemachine;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Camera), typeof(CinemachineBrain))]
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
	internal sealed class CameraOccluder : StateController
	{
		private static CameraOccluder _instance;
		private Camera _mainCamera;
		private BoxCollider2D _cameraCollider;
		private new void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject);
				return;
			}
			_instance = this;
			base.Awake();
			this._mainCamera = this.GetComponent<Camera>();
			this._cameraCollider = this.GetComponent<BoxCollider2D>();
			float sizeY = this._mainCamera.orthographicSize * 2f;
			float sizeX = sizeY * this._mainCamera.aspect;
			this._cameraCollider.size = new Vector2(sizeX, sizeY);
			foreach (Collider2D objects in Physics2D.OverlapBoxAll(this.transform.position, this._cameraCollider.size, 0f))
				this.SetOtherChildren(objects.gameObject, true);
		}
		private void SetOtherChildren(GameObject gameObject, bool activeValue)
		{
			if (gameObject.TryGetComponent<HiddenObject>(out var hiddenObject))
				for (ushort i = 0; i < hiddenObject.transform.childCount; i++)
					hiddenObject.transform.GetChild(i).gameObject.SetActive(activeValue);
		}
		private void OnTriggerEnter2D(Collider2D other) => this.SetOtherChildren(other.gameObject, true);
		private void OnTriggerExit2D(Collider2D other) => this.SetOtherChildren(other.gameObject, false);
	};
};
