using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[CreateAssetMenu(fileName = "Flying Enemy", menuName = "Enemy Statistics/Flying", order = 4)]
	internal sealed class FlyingStatistics : MovingStatistics
	{
		[Header("Flying Enemy")]
		[SerializeField, Tooltip("The target this enemy have to pursue.")] private GameObject _target;
		[SerializeField, Tooltip("How far this enemy detect any target.")] private float _radiusDetection;
		[SerializeField, Tooltip("The distance to stay away from the target.")] private float _targetDistance;
		[SerializeField, Tooltip("The time this enemy stay alive during the endless pursue.")] private float _fadeTime;
		[SerializeField, Tooltip("If this enemy will pursue the target until fade.")] private bool _endlessPursue;
		[Header("Trail")]
		[SerializeField, Tooltip("The amount of speed that this enemy moves to go back to the original point.")] private float _returnSpeed;
		[SerializeField, Tooltip("If this enemy will repeat the same way it makes before.")] private bool _repeatWay;
		internal GameObject Target => this._target;
		internal float RadiusDetection => this._radiusDetection;
		internal float TargetDistance => this._targetDistance;
		internal float FadeTime => this._fadeTime;
		internal bool EndlessPursue => this._endlessPursue;
		internal float ReturnSpeed => this._returnSpeed;
		internal bool RepeatWay => this._repeatWay;
	};
};