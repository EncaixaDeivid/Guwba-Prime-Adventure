using UnityEngine;
using UnityEngine.Events;
namespace GuwbaPrimeAdventure
{
	public abstract class StateController : MonoBehaviour
	{
		private static UnityAction<bool> _state;
		private UnityAction<bool> InstanceState => state => enabled = state;
		protected void Awake() => _state += InstanceState;
		protected void OnDestroy() => _state -= InstanceState;
		public static void SetState(bool newState) => _state?.Invoke(newState);
		protected sealed class WaitTime : CustomYieldInstruction
		{
			private readonly StateController _instance;
			private float _time;
			public override bool keepWaiting
			{
				get
				{
					if (_time > 0f && _instance.isActiveAndEnabled)
						_time -= Time.deltaTime;
					return _time > 0f;
				}
			}
			public WaitTime(StateController instance, float time)
			{
				_instance = instance;
				_time = time;
			}
		}
	};
};
