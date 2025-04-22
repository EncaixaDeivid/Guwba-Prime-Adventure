using UnityEngine;
using System.Collections;
namespace GuwbaPrimeAdventure.Effects
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	public sealed class EffectsController : StateController
	{
		private static EffectsController _instance;
		private bool _canHitStop = true;
		private new void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			base.Awake();
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
	};
};
