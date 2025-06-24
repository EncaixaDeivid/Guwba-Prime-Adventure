using UnityEngine;
using System;
namespace GuwbaPrimeAdventure.Story
{
	[CreateAssetMenu(fileName = "Dialog", menuName = "Scriptable Objects/Dialog", order = 1)]
	internal class DialogObject : ScriptableObject
    {
		[Header("Dialog Collection")]
		[SerializeField, Tooltip("The collection of the object that contais the entire dialog.")] private Dialog[] _dialog;
		internal Dialog[] Dialogs => this._dialog;
	};
	[Serializable]
	internal struct Dialog
	{
		[Header("Dialog Components")]
		[SerializeField, Tooltip("The collection of the object that contais the speach.")] private Speach[] _speach;
		[SerializeField, Tooltip("The scene to trancision to after the speach.")] private string _sceneToTransition;
		[SerializeField, Tooltip("The animation to play after the speach.")] private string _animation;
		[SerializeField, Tooltip("If the trancision have to be activated after the speach.")] private bool _activateTransition;
		[SerializeField, Tooltip("If the interaction have to be inactivated after the speach.")] private bool _desactiveInteraction;
		[SerializeField, Tooltip("If the animation will play.")] private bool _activateAnimation;
		[SerializeField, Tooltip("If the object will destruct.")] private bool _activateDestroy;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnEspecific;
		[SerializeField, Tooltip("The amount of time to wait to destroy after the speach.")] private float _timeToDestroy;
		internal readonly Speach[] Speachs => this._speach;
		internal readonly string SceneToTransition => this._sceneToTransition;
		internal readonly string Animation => this._animation;
		internal readonly bool ActivateTransition => this._activateTransition;
		internal readonly bool DesactiveInteraction => this._desactiveInteraction;
		internal readonly bool ActivateAnimation => this._activateAnimation;
		internal readonly bool ActivateDestroy => this._activateDestroy;
		internal readonly bool SaveOnEspecific => this._saveOnEspecific;
		internal readonly float TimeToDestroy => this._timeToDestroy;
		[Serializable]
		internal struct Speach
		{
			[Header("Speach Components")]
			[SerializeField, Tooltip("The image icon of the character that is speaking.")] private Sprite _model;
			[SerializeField, Tooltip("The name of the character that is speaking.")] private string _characterName;
			[SerializeField, TextArea(1, 12), Tooltip("The speach of the character that is speaking.")] private string _speachText;
			[SerializeField, Tooltip("If after the speach the next slide of story scene have to come.")] private bool _nextSlide;
			internal readonly Sprite Model => this._model;
			internal readonly string CharacterName => this._characterName;
			internal readonly string SpeachText => this._speachText;
			internal readonly bool NextSlide => this._nextSlide;
		};
	};
};