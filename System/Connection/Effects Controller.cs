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
		private readonly List<Light2DBase> _lightsStack = new();
		private readonly List<AudioSource> _soundSources = new();
		private readonly List<float> _sourceTimers = new();
		private Collider2D _surfaceCollider;
		private bool _canHitStop = true;
		[SerializeField] private SurfaceSound[] _surfaceSounds;
		[SerializeField] private AudioSource _sourceObject;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(gameObject, WorldBuild.MINIMUM_TIME_SPACE_LIMIT);
				return;
			}
			_instance = this;
			_lightsStack.Add(GetComponent<Light2DBase>());
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			StopAllCoroutines();
		}
		private void OnEnable()
		{
			foreach (AudioSource source in _soundSources.ToArray())
				source.UnPause();
		}
		private void OnDisable()
		{
			foreach (AudioSource source in _soundSources.ToArray())
				source.Pause();
		}
		private void Update()
		{
			for (ushort i = 0; i < _sourceTimers.Count; i++)
				if ((_sourceTimers[i] -= Time.deltaTime) <= 0f)
				{
					Destroy(_soundSources[i].gameObject);
					_soundSources.RemoveAt(i);
					_sourceTimers.RemoveAt(i);
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
				Time.timeScale = 1F;
				_canHitStop = true;
			}
		}
		private void PrivateGlobalLight(Light2DBase globalLight, bool active)
		{
			if ((active && !_lightsStack.Contains(globalLight) || !active && _lightsStack.Contains(globalLight)) && globalLight && _lightsStack[0])
			{
				for (ushort i = 0; i < _lightsStack.Count; i++)
					if (_lightsStack[i])
						_lightsStack[i].enabled = false;
				if (active)
					_lightsStack.Add(globalLight);
				else
					_lightsStack.Remove(globalLight);
				_lightsStack[^1].enabled = true;
			}
		}
		private void PrivateSoundEffect(AudioClip clip, Vector2 originSound)
		{
			SettingsController.Load(out Settings settings);
			AudioSource source = Instantiate(_sourceObject, originSound, Quaternion.identity);
			source.clip = clip;
			source.mute = !settings.EffectsVolumeToggle && !settings.GeneralVolumeToggle;
			_soundSources.Add(source);
			_sourceTimers.Add(clip.length);
			source.Play();
		}
		private void PrivateSurfaceSound(Vector2 originPosition)
		{
			if ((_surfaceCollider = Physics2D.OverlapPoint(originPosition, WorldBuild.SceneMask)) && _surfaceCollider.TryGetComponent<Surface>(out var surface))
				for (ushort i = 0; i < _surfaceSounds.Length; i++)
					if (_surfaceSounds[i].Tiles.Contains(surface.CheckForTile(originPosition)))
					{
						PrivateSoundEffect(_surfaceSounds[i].Clip, originPosition);
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
