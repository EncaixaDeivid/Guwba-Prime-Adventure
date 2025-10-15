using UnityEngine;
using UnityEngine.Rendering;
namespace GuwbaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Animator), typeof(SortingGroup))]
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CircleCollider2D))]
	public sealed class GuwbaCentralizer : StateController
	{
		private static GuwbaCentralizer _instance;
		public static Vector2 Position
		{
			get
			{
				if (_instance)
					return _instance.transform.position;
				return Vector2.zero;
			}
			set
			{
				if (_instance)
					_instance.transform.position = value;
			}
		}
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(this.gameObject, 1e-3f);
				return;
			}
			_instance = this;
		}
		public static bool EqualObject(params GameObject[] othersObjects)
		{
			if (_instance)
				foreach (GameObject other in othersObjects)
					if (other == _instance.gameObject)
						return true;
			return false;
		}
	};
};
