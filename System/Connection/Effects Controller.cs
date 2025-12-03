using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace GwambaPrimeAdventure.Connection
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Light2DBase))]
	public sealed class EffectsController : StateController
	{
		private static EffectsController _instance;
		private Light2DBase[] _lightsStack;
		private Surface[] _surfaces;
		private readonly Dictionary<AudioSource, float> _soundSources = new();
		private bool _canHitStop = true;
		[SerializeField] private SurfaceSound[] _surfaceSounds;
		[SerializeField] private AudioSource _sourceObject;
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
		private void OnEnable()
		{
			foreach (KeyValuePair<AudioSource, float> source in _soundSources.ToArray())
				source.Key.UnPause();
		}
		private void OnDisable()
		{
			foreach (KeyValuePair<AudioSource, float> source in _soundSources.ToArray())
				source.Key.UnPause();
		}
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			_surfaces = FindObjectsByType<Surface>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
		}
		private void Update()
		{
			foreach (KeyValuePair<AudioSource, float> source in _soundSources.ToArray())
				if ((_soundSources[source.Key] -= Time.deltaTime) <= 0f)
				{
					_soundSources.Remove(source.Key);
					Destroy(source.Key.gameObject);
				}
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
		private void PrivateSoundEffect(AudioClip clip, Vector2 originSound)
		{
			SettingsController.Load(out Settings settings);
			AudioSource source = Instantiate(_sourceObject, originSound, Quaternion.identity);
			source.clip = clip;
			source.mute = !settings.EffectsVolumeToggle && !settings.GeneralVolumeToggle;
			_soundSources.Add(source, clip.length);
			source.Play();
		}
		private void PrivateSurfaceSound(Vector2 originPosition)
		{
			foreach (Surface surface in _surfaces)
				foreach (SurfaceSound surfaceSound in _surfaceSounds)
					if (surfaceSound.Tiles.Contains(surface.CheckForTile(originPosition)))
					{
						PrivateSoundEffect(surfaceSound.Clip, originPosition);
						return;
					}
		}
		public static void HitStop(float stopTime, float slowTime) => _instance.PrvateHitStop(stopTime, slowTime);
		public static void OnGlobalLight(Light2DBase globalLight) => _instance.PrivateGlobalLight(globalLight, true);
		public static void OffGlobalLight(Light2DBase globalLight) => _instance.PrivateGlobalLight(globalLight, false);
		public static void SoundEffect(AudioClip clip, Vector2 originSound) => _instance.PrivateSoundEffect(clip, originSound);
		public static void SurfaceSound(Vector2 originPosition) => _instance.PrivateSurfaceSound(originPosition);
	};
};
