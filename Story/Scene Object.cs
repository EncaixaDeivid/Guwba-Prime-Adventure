using UnityEngine;
using UnityEditor;
using System;
namespace GuwbaPrimeAdventure.Story
{
	[CreateAssetMenu(fileName = "Dialog", menuName = "Story/Dialog", order = 0)]
	internal class DialogObject : ScriptableObject
    {
		[Header("Dialog Components")]
		[SerializeField, Tooltip("The scene to trancision to after the speach.")] private SceneAsset _sceneToTransition;
		[SerializeField, Tooltip("The collection of the object that contais the speach.")] private Speach[] _speach;
		[SerializeField, Tooltip("The animation to play after the speach.")] private string _animation;
		[SerializeField, Tooltip("If the trancision have to be activated after the speach.")] private bool _activateTransition;
		[SerializeField, Tooltip("If the interaction have to be inactivated after the speach.")] private bool _desactiveInteraction;
		[SerializeField, Tooltip("If the animation will play.")] private bool _activateAnimation;
		[SerializeField, Tooltip("If the object will destruct.")] private bool _activateDestroy;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		[SerializeField, Tooltip("The amount of time to wait to destroy after the speach.")] private float _timeToDestroy;
		internal SceneAsset SceneToTransition => this._sceneToTransition;
		internal Speach[] Speachs => this._speach;
		internal string Animation => this._animation;
		internal bool ActivateTransition => this._activateTransition;
		internal bool DesactiveInteraction => this._desactiveInteraction;
		internal bool ActivateAnimation => this._activateAnimation;
		internal bool ActivateDestroy => this._activateDestroy;
		internal bool SaveOnEspecific => this._saveOnSpecifics;
		internal float TimeToDestroy => this._timeToDestroy;
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
