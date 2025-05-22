using UnityEngine;
using UnityEngine.Events;
using System.Collections;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	public abstract class GuwbaTransformer<GuwbaInstance> : StateController where GuwbaInstance : StateController
	{
		private static GuwbaInstance _instance;
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
		public static void SetRotation(float axisZRotation, float speed = 0f)
		{
			Quaternion rotation = Quaternion.AngleAxis(axisZRotation, Vector3.forward);
			if (speed == 0f)
				_instance.transform.rotation = rotation;
			else
				_instance.StartCoroutine(TimeRotation());
			IEnumerator TimeRotation()
			{
				while (_instance.transform.rotation != rotation)
				{
					float speedRotation = speed * Time.fixedDeltaTime;
					_instance.transform.rotation = Quaternion.RotateTowards(_instance.transform.rotation, rotation, speedRotation);
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => _instance.enabled);
				}
			}
		}
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
