using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.OffEnviroment
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class InteractableDialog : MonoBehaviour, IInteractable
    {
		private DialogHud _dialogHud;
		private StoryTeller _storyTeller;
		private Animator _animator;
		private Dialog _dialogTalk;
		private string _text = "";
		private ushort _dialogObjectIndex = 0;
		private ushort _dialogIndex = 0;
		private ushort _speachIndex = 0;
		private float _dialogTime = 0f;
		private bool _nextSlide = false;
		[Header("Interaction Objects")]
		[SerializeField, Tooltip("The object that handles the hud of the dialog.")] private DialogHud _dialogHudObject;
		[SerializeField, Tooltip("The collection of the object that contais the dialog.")] private DialogObject[] _dialogObject;
		public void Interaction()
		{
			if (this._dialogObject?.Length > 0f && this._dialogHudObject)
			{
				SettingsController.Load(out Settings settings);
				StateController.SetState(false);
				this._storyTeller = this.GetComponent<StoryTeller>();
				this._animator = this.GetComponent<Animator>();
				this._dialogHud = Instantiate(this._dialogHudObject);
				this._dialogTalk = this._dialogObject[this._dialogObjectIndex].Dialogs[this._dialogIndex];
				bool indexValidation = this._dialogIndex < this._dialogObject[this._dialogObjectIndex].Dialogs.Length - 1f;
				this._dialogIndex = (ushort)(indexValidation ? this._dialogIndex + 1f : 0f);
				this._dialogObjectIndex = (ushort)(this._dialogObjectIndex < this._dialogObject.Length - 1f ? this._dialogObjectIndex + 1f : 0f);
				this._dialogTime = settings.dialogSpeed;
				this._dialogHud.AdvanceSpeach.clicked += this.AdvanceSpeach;
				this.StartCoroutine(this.TextDigitation());
				if (this._storyTeller)
					this._storyTeller.ShowScene();
			}
		}
		private IEnumerator TextDigitation()
		{
			StyleBackground image = new(this._dialogTalk.Speachs[this._speachIndex].Model);
			this._dialogHud.CharacterIcon.style.backgroundImage = image;
			this._dialogHud.CharacterName.text = this._dialogTalk.Speachs[this._speachIndex].CharacterName;
			this._text = this._dialogTalk.Speachs[this._speachIndex].SpeachText;
			this._dialogHud.CharacterSpeach.text = "";
			if (this._nextSlide)
			{
				this._nextSlide = false;
				yield return this._storyTeller.NextSlide();
				this._dialogHud.RootElement.style.display = DisplayStyle.Flex;
			}
			foreach (char letter in this._text.ToCharArray())
			{
				this._dialogHud.CharacterSpeach.text += letter;
				yield return new WaitForSeconds(this._dialogTime);
			}
		}
		private void WorldInteraction()
		{
			SaveController.Load(out SaveFile saveFile);
			if (this._dialogTalk.SaveOnEspecific && !saveFile.generalObjects.Contains(this.gameObject.name))
			{
				saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(saveFile);
			}
			if (this._dialogTalk.ActivateTransition)
				this.GetComponent<Transitioner>().Transicion(this._dialogTalk.SceneToTransition);
			else if (this._dialogTalk.ActivateAnimation)
				this._animator.SetTrigger(this._dialogTalk.Animation);
			if (this._dialogTalk.DesactiveInteraction)
				this.enabled = false;
			if (!this._dialogTalk.ActivateTransition && this._dialogTalk.ActivateDestroy)
				Destroy(this.gameObject, this._dialogTalk.TimeToDestroy);
		}
		private void AdvanceSpeach()
		{
			if (this._dialogHud.CharacterSpeach.text.Length == this._text.Length && this._dialogHud.CharacterSpeach.text == this._text)
			{
				SettingsController.Load(out Settings settings);
				this._dialogTime = settings.dialogSpeed;
				if (this._speachIndex < this._dialogTalk.Speachs.Length - 1f)
				{
					if (this._storyTeller && this._dialogTalk.Speachs[this._speachIndex].NextSlide)
					{
						this._nextSlide = true;
						this._dialogHud.RootElement.style.display = DisplayStyle.None;
					}
					this._speachIndex += 1;
					this.StartCoroutine(this.TextDigitation());
				}
				else
				{
					this._text = null;
					this._speachIndex = 0;
					this._dialogHud.CharacterIcon.style.backgroundImage = null;
					this._dialogHud.CharacterName.text = "";
					this._dialogHud.CharacterSpeach.text = "";
					this._dialogHud.AdvanceSpeach.clicked -= this.AdvanceSpeach;
					Destroy(this._dialogHud.gameObject);
					StateController.SetState(true);
					if (this._storyTeller)
						this._storyTeller.CloseScene();
					this.WorldInteraction();
				}
			}
			else
				this._dialogTime = 0f;
		}
	};
};