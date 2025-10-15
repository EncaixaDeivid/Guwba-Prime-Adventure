using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Story
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class InteractiveDialog : MonoBehaviour, IInteractable, IConnector
    {
		private DialogHud _dialogHud;
		private StoryTeller _storyTeller;
		private Animator _animator;
		private readonly Sender _sender = Sender.Create();
		private readonly int _isOn = Animator.StringToHash("IsOn");
		private string _text = "";
		private ushort _speachIndex = 0;
		private float _dialogTime = 0f;
		private bool _nextSlide = false;
		[Header("Interaction Objects")]
		[SerializeField, Tooltip("The object that handles the hud of the dialog.")] private DialogHud _dialogHudObject;
		[SerializeField, Tooltip("The collection of the object that contais the dialog.")] private DialogObject _dialogObject;
		public PathConnection PathConnection => PathConnection.Story;
		private void Awake()
		{
			_storyTeller = GetComponent<StoryTeller>();
			_animator = GetComponent<Animator>();
			_sender.SetStateForm(StateForm.State);
			_sender.SetAdditionalData(gameObject);
		}
		private void OnEnable()
		{
			if (_animator)
				_animator.SetFloat(_isOn, 1f);
		}
		private void OnDisable()
		{
			if (_animator)
				_animator.SetFloat(_isOn, 0f);
		}
		private IEnumerator TextDigitation()
		{
			StyleBackground image = new(_dialogObject.Speachs[_speachIndex].Model);
			_dialogHud.CharacterIcon.style.backgroundImage = image;
			_dialogHud.CharacterName.text = _dialogObject.Speachs[_speachIndex].CharacterName;
			_text = _dialogObject.Speachs[_speachIndex].SpeachText;
			_dialogHud.CharacterSpeach.text = "";
			if (_nextSlide)
			{
				_nextSlide = false;
				yield return _storyTeller.NextSlide();
				_dialogHud.RootElement.style.display = DisplayStyle.Flex;
			}
			foreach (char letter in _text.ToCharArray())
			{
				_dialogHud.CharacterSpeach.text += letter;
				yield return new WaitForSeconds(_dialogTime);
			}
		}
		private void AdvanceSpeach()
		{
			if (_dialogHud.CharacterSpeach.text.Length == _text.Length && _dialogHud.CharacterSpeach.text == _text)
			{
				SettingsController.Load(out Settings settings);
				_dialogTime = settings.DialogSpeed;
				if (_speachIndex < _dialogObject.Speachs.Length - 1f)
				{
					if (_storyTeller && _dialogObject.Speachs[_speachIndex].NextSlide)
					{
						_nextSlide = true;
						_dialogHud.RootElement.style.display = DisplayStyle.None;
					}
					_speachIndex += 1;
					StartCoroutine(TextDigitation());
				}
				else
				{
					_text = null;
					_speachIndex = 0;
					_dialogHud.CharacterIcon.style.backgroundImage = null;
					_dialogHud.CharacterName.text = "";
					_dialogHud.CharacterSpeach.text = "";
					_dialogHud.AdvanceSpeach.clicked -= AdvanceSpeach;
					Destroy(_dialogHud.gameObject);
					StateController.SetState(true);
					if (_storyTeller)
						_storyTeller.CloseScene();
					SaveController.Load(out SaveFile saveFile);
					if (_dialogObject.SaveOnEspecific && !saveFile.generalObjects.Contains(gameObject.name))
					{
						saveFile.generalObjects.Add(gameObject.name);
						SaveController.WriteSave(saveFile);
					}
					if (_dialogObject.ActivateTransition)
						GetComponent<Transitioner>().Transicion(_dialogObject.SceneToTransition);
					else if (_dialogObject.ActivateAnimation)
						_animator.SetTrigger(_dialogObject.Animation);
					if (_dialogObject.DesactiveInteraction)
					{
						_sender.SetAdditionalData(null);
						Destroy(this);
					}
					if (!_dialogObject.ActivateTransition && _dialogObject.ActivateDestroy)
						Destroy(gameObject, _dialogObject.TimeToDestroy);
					_sender.SetToggle(true);
					_sender.Send(PathConnection.Hud);
				}
			}
			else
				_dialogTime = 0f;
		}
		public void Interaction()
		{
			_sender.SetToggle(false);
			_sender.Send(PathConnection.Hud);
			StateController.SetState(false);
			SettingsController.Load(out Settings settings);
			if (settings.DialogToggle && _dialogObject && _dialogHudObject)
			{
				_dialogHud = Instantiate(_dialogHudObject, transform);
				_dialogTime = settings.DialogSpeed;
				_dialogHud.AdvanceSpeach.clicked += AdvanceSpeach;
				StartCoroutine(TextDigitation());
				if (_storyTeller)
					_storyTeller.ShowScene();
			}
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Action && (GameObject)additionalData == gameObject)
				Interaction();
		}
	};
};
