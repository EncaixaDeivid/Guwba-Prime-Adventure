using UnityEngine;
using UnityEngine.Events;
namespace GwambaPrimeAdventure
{
	public abstract class StateController : MonoBehaviour
	{
		private static UnityAction<bool> _state;
		protected void Awake() => _state += InstanceState;
		protected void OnDestroy() => _state -= InstanceState;
		private UnityAction<bool> InstanceState => newState => enabled = newState;
		public static void SetState(bool newState) => _state?.Invoke(newState);
		protected sealed class WaitTime : CustomYieldInstruction
		{
			private readonly StateController _instance;
			private readonly bool _unscaled;
			private float _time;
			public override bool keepWaiting
			{
				get
				{
					if (_time > 0f && _instance.isActiveAndEnabled)
						_time -= Time.deltaTime / (_unscaled ? Time.timeScale : 1f);
					return _time > 0f;
				}
			}
			public WaitTime(StateController instance, float time, bool unscaled = false)
			{
				_instance = instance;
				_time = time;
				_unscaled = unscaled;
			}
		};
	};
};
