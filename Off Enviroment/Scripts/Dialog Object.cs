using UnityEngine;
using System;
namespace GuwbaPrimeAdventure.OffEnviroment
{
	[CreateAssetMenu(fileName = "Dialog", menuName = "Scriptable Objects/Dialog", order = 0)]
	internal class DialogObject : ScriptableObject
    {
		[SerializeField] private Dialog[] _dialog;
		internal Dialog[] Dialogs => this._dialog;
	};
	[Serializable]
	internal struct Dialog
	{
		[SerializeField] private Speach[] _speach;
		[SerializeField] private string _sceneToTransition;
		[SerializeField] private string _animation;
		[SerializeField] private bool _activateTransition;
		[SerializeField] private bool _desactiveInteraction;
		[SerializeField] private bool _activateAnimation;
		[SerializeField] private bool _activateDestroy;
		[SerializeField] private bool _saveOnEspecific;
		[SerializeField] private float _timeToDestroy;
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
			[SerializeField] private Sprite _model;
			[SerializeField] private string _characterName;
			[SerializeField, TextArea(1, 12)] private string _speachText;
			[SerializeField] private bool _nextScene;
			internal readonly Sprite Model => this._model;
			internal readonly string CharacterName => this._characterName;
			internal readonly string SpeachText => this._speachText;
			internal readonly bool NextScene => this._nextScene;
		};
	};
};