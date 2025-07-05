using UnityEngine;
using UnityEngine.Events;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	public abstract class GuwbaAstral<GuwbaInstance> : StateController where GuwbaInstance : StateController
	{
		private static GuwbaInstance _instance;
		protected readonly Sender _sender = Sender.Create();
		protected static GrabBody _grabObject;
		protected static UnityAction<bool> _actualState;
		protected static bool _returnAttack;
		protected new void Awake()
		{
			_instance = this.GetComponent<GuwbaInstance>();
			if (!_instance || _instance is not CommandGuwba && _instance is not VisualGuwba && _instance is not AttackGuwba)
				Destroy(this.gameObject, 0.001f);
			else
				base.Awake();
		}
		public static Vector2 Position { get => _instance.transform.position; set => _instance.transform.position = value; }
		public static bool EqualObject(params GameObject[] unknowGameObjects)
		{
			foreach (GameObject gameObject in unknowGameObjects)
				if (gameObject == _instance.gameObject)
					return true;
			return false;
		}
		public static bool EqualObject(params RaycastHit2D[] unknowRaycastHits)
		{
			foreach (RaycastHit2D raycastHit in unknowRaycastHits)
				if (raycastHit.collider.gameObject == _instance.gameObject)
					return true;
			return false;
		}
	};
};
