using UnityEngine;
using System;
namespace GuwbaPrimeAdventure.Dialog
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
		internal readonly Speach[] Speachs => this._speach;
		[Serializable]
		internal struct Speach
		{
			[SerializeField] private string _characterName;
			[SerializeField] private Sprite _model;
			[SerializeField, TextArea(1, 12)] private string _speachText;
			[SerializeField] private string _sceneToTransition;
			[SerializeField] private bool _activateTransition, _saveOnEspecific;
			internal readonly string CharacterName => this._characterName;
			internal readonly Sprite Model => this._model;
			internal readonly string SpeachText => this._speachText;
			internal readonly bool ActivateTransition => this._activateTransition;
			internal readonly bool SaveOnEspecific => this._saveOnEspecific;
			internal readonly string SceneToTransition => this._sceneToTransition;
		};
	};
};
