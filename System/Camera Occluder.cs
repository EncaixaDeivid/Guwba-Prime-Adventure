using UnityEngine;
using Unity.Cinemachine;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Camera), typeof(CinemachineBrain))]
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
	internal sealed class CameraOccluder : StateController
	{
		private static CameraOccluder _instance;
		private Camera _cameraGuwba;
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
			this._cameraGuwba = this.GetComponent<Camera>();
			this._cameraCollider = this.GetComponent<BoxCollider2D>();
			float sizeY = this._cameraGuwba.orthographicSize * 2f;
			float sizeX = sizeY * this._cameraGuwba.aspect;
			this._cameraCollider.size = new Vector2(sizeX, sizeY);
			foreach (Collider2D objects in Physics2D.OverlapBoxAll(this.transform.position, this._cameraCollider.size, 0f))
				this.SetOtherChildren(objects.gameObject, true);
		}
		private void SetOtherChildren(GameObject gameObject, bool activeValue)
		{
			if (gameObject.TryGetComponent<HiddenCamera>(out var hiddenCamera))
				for (ushort i = 0; i < hiddenCamera.transform.childCount; i++)
					hiddenCamera.transform.GetChild(i).gameObject.SetActive(activeValue);
		}
		private void OnTriggerEnter2D(Collider2D other) => this.SetOtherChildren(other.gameObject, true);
		private void OnTriggerExit2D(Collider2D other) => this.SetOtherChildren(other.gameObject, false);
	};
};
