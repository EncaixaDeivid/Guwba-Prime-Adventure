using UnityEngine;
using System;
namespace GuwbaPrimeAdventure.OffEnviroment
{
	[CreateAssetMenu(fileName = "Scene", menuName = "Scriptable Objects/Scene", order = 2)]
	internal sealed class SceneObject : ScriptableObject
	{
		[SerializeField] private BackgroundImage[] _backgroundImages;
		internal BackgroundImage[] BackgroundImages => this._backgroundImages;
		[Serializable]
		internal struct BackgroundImage
		{
			[SerializeField] private Texture2D _image;
			[SerializeField] private bool _offDialog;
			[SerializeField] private float _timeToDesapear;
			internal readonly Texture2D Image => this._image;
			internal readonly bool OffDialog => this._offDialog;
			internal readonly float TimeToDesapear => this._timeToDesapear;
		};
	};
};
