using UnityEngine;
namespace GwambaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Flying Enemy", menuName = "Enemy Statistics/Flying", order = 4)]
	internal sealed class FlyingStatistics : MovingStatistics
	{
		[Header("Flying Enemy")]
		[SerializeField, Tooltip("The target this enemy have to pursue.")] private GameObject _target;
		[SerializeField, Tooltip("The amount of speed to this enemy rotate towards.")] private float _rotationSpeed;
		[SerializeField, Tooltip("The distance to stay away from the target.")] private float _targetDistance;
		[SerializeField, Tooltip("The multiplication factor of the detection.")] private float _detectionFactor;
		[SerializeField, Tooltip("The amount of speed that this enemy moves to go back to the original point.")] private float _returnSpeed;
		[SerializeField, Tooltip("The amount of time this enemy moves will be stopped after it attack.")] private float _afterTime;
		[SerializeField, Tooltip("If this enemy will pursue the target until fade.")] private bool _endlessPursue;
		internal GameObject Target => _target;
		internal float RotationSpeed => _rotationSpeed;
		internal float TargetDistance => _targetDistance;
		internal float DetectionFactor => _detectionFactor;
		internal float ReturnSpeed => _returnSpeed;
		internal float AfterTime => _afterTime;
		internal bool EndlessPursue => _endlessPursue;
	};
};
