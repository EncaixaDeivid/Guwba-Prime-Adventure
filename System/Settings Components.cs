using Unity.Cinemachine;
using UnityEngine;
using System;
namespace GuwbaPrimeAdventure
{
	[Serializable]
	public struct ScreenShakeProfile
	{
		[Header("Impulse Listener")]
		[SerializeField, Tooltip("The amplitude gain of the impulse listener.")] private float _amplitudeGain;
		[SerializeField, Tooltip("The frequency gain of the impulse listener.")] private float _frequencyGain;
		[SerializeField, Tooltip("The duration of the impulse listener.")] private float _duration;
		[Header("Impulse Source")]
		[SerializeField, Tooltip("The impulse created by the editor.")] private AnimationCurve _customImpulse;
		[SerializeField, Tooltip("The type of the impulse will get.")] private CinemachineImpulseDefinition.ImpulseTypes _impulseType;
		[SerializeField, Tooltip("The the shape of the impulse.")] private CinemachineImpulseDefinition.ImpulseShapes _impulseShape;
		[SerializeField, Tooltip("The mode of dissipation of the impulse.")]
		private CinemachineImpulseManager.ImpulseEvent.DissipationModes _dissipationMode;
		[SerializeField, Tooltip("The velocity of the impulse.")] private Vector2 _velocity;
		[SerializeField, Tooltip("The force of the impulse.")] private float _force;
		[SerializeField, Tooltip("The time's duration of the impulse.")] private float _time;
		[SerializeField, Tooltip("The propagation's speed of the impulse.")] private float _propagationSpeed;
		[SerializeField, Tooltip("The dissiopation's distance of the impulse.")] private float _dissipationDistance;
		[SerializeField, Tooltip("The dissiopation's rate of the impulse.")] private float _dissipationRate;
		public readonly float AmplitudeGain => this._amplitudeGain;
		public readonly float FrequencyGain => this._frequencyGain;
		public readonly float Duration => this._duration;
		public readonly AnimationCurve CustomImpulse => this._customImpulse;
		public readonly CinemachineImpulseDefinition.ImpulseTypes ImpulseType => this._impulseType;
		public readonly CinemachineImpulseDefinition.ImpulseShapes ImpulseShape => this._impulseShape;
		public readonly CinemachineImpulseManager.ImpulseEvent.DissipationModes DissipationMode => this._dissipationMode;
		public readonly Vector2 Velocity => this._velocity;
		public readonly float Force => this._force;
		public readonly float Time => this._time;
		public readonly float PropagationSpeed => this._propagationSpeed;
		public readonly float DissipationDistance => this._dissipationDistance;
		public readonly float DissipationRate => this._dissipationRate;
	};
};