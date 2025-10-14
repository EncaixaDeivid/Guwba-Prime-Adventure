using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Flying Enemy", menuName = "Enemy Statistics/Flying", order = 4)]
	internal sealed class FlyingStatistics : MovingStatistics
	{
		[Header("Flying Enemy")]
		[SerializeField, Tooltip("The target this enemy have to pursue.")] private GameObject _target;
		[SerializeField, Tooltip("The distance to stay away from the target.")] private float _targetDistance;
		[SerializeField, Tooltip("The time this enemy stay alive during the endless pursue.")] private float _fadeTime;
		[SerializeField, Tooltip("The multiplication factor of the detection.")] private float _detectionFactor;
		[SerializeField, Tooltip("The amount of speed that this enemy moves to go back to the original point.")] private float _returnSpeed;
		[SerializeField, Tooltip("If this enemy will pursue the target until fade.")] private bool _endlessPursue;
		internal GameObject Target => this._target;
		internal float TargetDistance => this._targetDistance;
		internal float FadeTime => this._fadeTime;
		internal float DetectionFactor => this._detectionFactor;
		internal float ReturnSpeed => this._returnSpeed;
		internal bool EndlessPursue => this._endlessPursue;
	};
};
