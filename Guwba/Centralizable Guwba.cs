using UnityEngine;
using UnityEngine.Rendering;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Animator), typeof(SortingGroup))]
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CircleCollider2D))]
	public sealed class CentralizableGuwba : StateController
	{
		private static CentralizableGuwba _instance;
		public static Vector2 Position { get => _instance.transform.position; set => _instance.transform.position = value; }
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
		}
		public static bool EqualObject(params GameObject[] othersObjects)
		{
			foreach (GameObject other in othersObjects)
				if (other == _instance.gameObject)
					return true;
			return false;
		}
		public static bool EqualObject(params RaycastHit2D[] othersObjects)
		{
			foreach (RaycastHit2D other in othersObjects)
				if (other.collider.gameObject == _instance.gameObject)
					return true;
			return false;
		}
	};
};
