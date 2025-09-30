using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure
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
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			this._lightsStack = new List<Light2DBase>() { this.GetComponent<Light2DBase>() };
		}
		private void PrvateHitStop(float stopTime, float slowTime)
		{
			if (this._canHitStop)
				this.StartCoroutine(HitStop());
			IEnumerator HitStop()
			{
				this._canHitStop = false;
				Time.timeScale = slowTime;
				yield return new WaitForSecondsRealtime(stopTime);
				this._canHitStop = true;
				Time.timeScale = 1f;
			}
		}
		private void PrivateOnGlobalLight(Light2DBase globalLight)
		{
			if (this._lightsStack.Contains(globalLight))
				return;
			foreach (Light2DBase light in this._lightsStack)
				light.enabled = false;
			globalLight.enabled = true;
			this._lightsStack.Add(globalLight);
		}
		private void PrivateOffGlobalLight(Light2DBase globalLight)
		{
			if (!this._lightsStack.Contains(globalLight))
				return;
			foreach (Light2DBase light in this._lightsStack)
				light.enabled = false;
			this._lightsStack.Remove(globalLight);
			this._lightsStack[^1].enabled = true;
		}
		public static void HitStop(float stopTime, float slowTime) => _instance.PrvateHitStop(stopTime, slowTime);
		public static void OnGlobalLight(Light2DBase globalLight) => _instance.PrivateOnGlobalLight(globalLight);
		public static void OffGlobalLight(Light2DBase globalLight) => _instance.PrivateOffGlobalLight(globalLight);
	};
};
