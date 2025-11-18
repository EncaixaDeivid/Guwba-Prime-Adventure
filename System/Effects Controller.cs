using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Light2DBase))]
	public sealed class EffectsController : StateController
	{
		private static EffectsController _instance;
		private List<Light2DBase> _lightsStack;
		private bool _canHitStop = true;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			_lightsStack = new List<Light2DBase>() { GetComponent<Light2DBase>() };
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			StopAllCoroutines();
		}
		private void PrvateHitStop(float stopTime, float slowTime)
		{
			if (_canHitStop)
				StartCoroutine(HitStop());
			IEnumerator HitStop()
			{
				_canHitStop = false;
				Time.timeScale = slowTime;
				yield return new WaitTime(this, stopTime);
				_canHitStop = true;
				Time.timeScale = 1f;
			}
		}
		private void PrivateGlobalLight(Light2DBase globalLight, bool active)
		{
			if ((active && !_lightsStack.Contains(globalLight) || !active && _lightsStack.Contains(globalLight)) && globalLight)
			{
				foreach (Light2DBase light in _lightsStack)
					if (light)
						light.enabled = false;
				if (active)
				{
					globalLight.enabled = true;
					_lightsStack.Add(globalLight);
				}
				else
				{
					_lightsStack.Remove(globalLight);
					_lightsStack[^1].enabled = true;
				}
			}
		}
		public static void HitStop(float stopTime, float slowTime) => _instance.PrvateHitStop(stopTime, slowTime);
		public static void OnGlobalLight(Light2DBase globalLight) => _instance.PrivateGlobalLight(globalLight, true);
		public static void OffGlobalLight(Light2DBase globalLight) => _instance.PrivateGlobalLight(globalLight, false);
	};
};
