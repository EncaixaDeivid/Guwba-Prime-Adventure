using UnityEngine;
using System;
namespace GuwbaPrimeAdventure.OffEnviroment
{
	[CreateAssetMenu(fileName = "Scene", menuName = "Scriptable Objects/Scene", order = 2)]
	internal sealed class SceneObject : ScriptableObject
	{
		[SerializeField, Tooltip("The collection of objects that carry the background settings.")] private BackgroundImage[] _backgroundImages;
		internal BackgroundImage[] BackgroundImages => this._backgroundImages;
		[Serializable]
		internal struct BackgroundImage
		{
			[SerializeField, Tooltip("The main imgae that is placed in the hud.")] private Texture2D _image;
			[SerializeField, Tooltip("If the dialog will go turned off during the scene.")] private bool _offDialog;
			[SerializeField, Tooltip("The amount of time that the dialog will be turned off.")] private float _timeToDesapear;
			internal readonly Texture2D Image => this._image;
			internal readonly bool OffDialog => this._offDialog;
			internal readonly float TimeToDesapear => this._timeToDesapear;
		};
	};
};
