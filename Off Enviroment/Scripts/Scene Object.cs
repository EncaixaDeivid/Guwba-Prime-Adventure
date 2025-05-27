using UnityEngine;
namespace GuwbaPrimeAdventure.OffEnviroment
{
	[CreateAssetMenu(fileName = "Scene", menuName = "Scriptable Objects/Scene", order = 2)]
	internal sealed class SceneObject : ScriptableObject
	{
		[SerializeField] private (Texture2D, bool)[] _backgroundImages;
		[SerializeField] private float _timeToDesapear = 0f;
		internal (Texture2D image, bool offDialog)[] BackgroundImages => this._backgroundImages;
		internal float TimeToDesapear => this._timeToDesapear;
	};
};