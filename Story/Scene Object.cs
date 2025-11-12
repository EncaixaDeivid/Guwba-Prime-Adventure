using UnityEngine;
using System;
namespace GwambaPrimeAdventure.Story
{
	[CreateAssetMenu(fileName = "Scene", menuName = "Story/Scene", order = 1)]
	internal sealed class SceneObject : ScriptableObject
	{
		[Header("Scene Component Collection")]
		[SerializeField, Tooltip("The collection of objects that carry the background settings.")] private SceneComponent[] _sceneComponents;
		internal SceneComponent[] SceneComponents => _sceneComponents;
		[Serializable]
		internal struct SceneComponent
		{
			[Header("Components")]
			[SerializeField, Tooltip("The main imgae that is placed in the hud.")] private Texture2D _image;
			[SerializeField, Tooltip("If the dialog will go turned off during the scene.")] private bool _offDialog;
			[SerializeField, Tooltip("If the dialog will jump to the next slide.")] private bool _jumpToNext;
			[SerializeField, Tooltip("The amount of time that the dialog will be turned off.")] private float _timeToDesapear;
			internal readonly Texture2D Image => _image;
			internal readonly bool OffDialog => _offDialog;
			internal readonly bool JumpToNext => _jumpToNext;
			internal readonly float TimeToDesapear => _timeToDesapear;
		};
	};
};
