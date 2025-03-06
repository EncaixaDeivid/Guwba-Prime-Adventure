using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
namespace GuwbaPrimeAdventure.Effects
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Transform), typeof(Light2DBase))]
	public sealed class EffectsController : StateController
	{
		private static EffectsController _instance;
		private Light2DBase _selfLight;
		private bool _canHitStop = true;
		private new void Awake()
		{
			if (_instance)
				Destroy(_instance.gameObject);
			_instance = this;
			base.Awake();
			this._selfLight = this.GetComponent<Light2DBase>();
		}
		public static void SetHitStop(float stopTime, float slowTime)
		{
			if (_instance._canHitStop)
				_instance.StartCoroutine(HitStop());
			IEnumerator HitStop()
			{
				_instance._canHitStop = false;
				Time.timeScale = slowTime;
				yield return new WaitForSecondsRealtime(stopTime);
				_instance._canHitStop = true;
				Time.timeScale = 1f;
			}
		}
		public static void SetGlobalLight(bool lightState) => _instance._selfLight.enabled = lightState;
	};
};