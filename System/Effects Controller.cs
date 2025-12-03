using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using System.Linq;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Light2DBase))]
	public sealed class EffectsController : StateController
	{
		private static EffectsController _instance;
		private Light2DBase[] _lightsStack;
		private Surface[] _surfaces;
		private bool _canHitStop = true;
		[SerializeField] private SurfaceSound _surfaceSound;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			_lightsStack = new Light2DBase[] { GetComponent<Light2DBase>() };
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			StopAllCoroutines();
		}
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			_surfaces = FindObjectsByType<Surface>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
		}
		private void PrvateHitStop(float stopTime, float slowTime)
		{
			if (_canHitStop)
				StartCoroutine(HitStop());
			IEnumerator HitStop()
			{
				_canHitStop = false;
				Time.timeScale = slowTime;
				yield return new WaitTime(this, stopTime, true);
				Time.timeScale = 1f;
				_canHitStop = true;
			}
		}
		private void PrivateGlobalLight(Light2DBase globalLight, bool active)
		{
			if ((active && !_lightsStack.Contains(globalLight) || !active && _lightsStack.Contains(globalLight)) && globalLight && _lightsStack[0])
			{
				Light2DBase[] lights;
				foreach (Light2DBase light in lights = _lightsStack)
					if (light)
						light.enabled = false;
				if (active)
				{
					_lightsStack = new Light2DBase[_lightsStack.Length + 1];
					for (ushort i = 0; i < lights.Length; i++)
						_lightsStack[i] = lights[i];
					_lightsStack[^1] = globalLight;
				}
				else
				{
					_lightsStack = new Light2DBase[_lightsStack.Length - 1];
					for (ushort i = 0; i < _lightsStack.Length; i++)
						_lightsStack[i] = lights[i];
				}
				_lightsStack[^1].enabled = true;
			}
		}
		private void PrivateSurfaceSound(Vector2 originPosition)
		{
			foreach (Surface surface in _surfaces)
				foreach (TilesSound tileSound in _surfaceSound.TilesSounds)
					if (tileSound.Tiles.Contains(surface.CheckForTile(originPosition)))
						AudioSource.PlayClipAtPoint(tileSound.Source.clip, originPosition);
		}
		public static void HitStop(float stopTime, float slowTime) => _instance.PrvateHitStop(stopTime, slowTime);
		public static void OnGlobalLight(Light2DBase globalLight) => _instance.PrivateGlobalLight(globalLight, true);
		public static void OffGlobalLight(Light2DBase globalLight) => _instance.PrivateGlobalLight(globalLight, false);
		public static void SurfaceSound(Vector2 originPosition) => _instance.PrivateSurfaceSound(originPosition);
	};
};
