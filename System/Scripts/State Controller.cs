using UnityEngine;
using UnityEngine.Events;
namespace GuwbaPrimeAdventure
{
	public abstract class StateController : MonoBehaviour
	{
		private static UnityAction<bool> _state;
		private UnityAction<bool> InstanceState => (bool state) => this.enabled = state;
		protected void Awake() => _state += this.InstanceState;
		protected void OnDestroy() => _state -= this.InstanceState;
		public static void SetState(bool newState) => _state?.Invoke(newState);
		protected sealed class WaitTime : CustomYieldInstruction
		{
			private readonly StateController _instance;
			private float _time;
			public override bool keepWaiting
			{
				get
				{
					if (this._time > 0f && _instance.enabled)
						this._time -= Time.deltaTime;
					return this._time > 0f;
				}
			}
			public WaitTime(StateController instance, float time)
			{
				this._instance = instance;
				this._time = time;
			}
		}
	};
};
