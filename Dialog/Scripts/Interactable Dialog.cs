using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
namespace GuwbaPrimeAdventure.Dialog
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class InteractableDialog : MonoBehaviour, IInteractable
    {
		private DialogHud _dialogHud;
		private Animator _animator;
		private Dialog _dialogTalk;
		private string _text = "";
		private ushort _dialogObjectIndex = 0, _dialogIndex = 0, _speachIndex = 0;
		private float _dialogTime = 0f;
		private bool _dialogClosed = false;
		[SerializeField] private DialogHud _dialogHudObject;
		[SerializeField] private DialogObject[] _dialogObject;
		public void Interaction()
		{
			if (this._dialogObject != null && this._dialogObject.Length > 0f && !this._dialogHudObject)
			{
				this._animator = this.GetComponent<Animator>();
				this._dialogHud = Instantiate(this._dialogHudObject);
				this._dialogTalk = this._dialogObject[this._dialogObjectIndex].Dialogs[this._dialogIndex];
				this._dialogClosed = true;
				this._dialogObjectIndex = (ushort)(this._dialogObjectIndex < this._dialogObject.Length - 1f ? this._dialogObjectIndex + 1f : 0f);
				this._dialogTime = SettingsData.DialogSpeed;
				this._dialogHud.AdvanceSpeach.clicked += this.AdvanceSpeach;
				this._dialogHud.CloseDialog.clicked += this.CloseDialog;
			}
		}
		private IEnumerator TextDigitation()
		{
			StyleBackground image = new(this._dialogTalk.Speachs[this._speachIndex].Model);
			this._dialogHud.CharacterIcon.style.backgroundImage = image;
			this._dialogHud.CharacterName.text = this._dialogTalk.Speachs[this._speachIndex].CharacterName;
			this._text = this._dialogTalk.Speachs[this._speachIndex].SpeachText;
			this._dialogHud.CharacterSpeach.text = null;
			foreach (char letter in this._text.ToCharArray())
			{
				this._dialogHud.CharacterSpeach.text += letter;
				if (this._dialogClosed)
				{
					this._dialogClosed = false;
					break;
				}
				yield return new WaitForSeconds(this._dialogTime);
			}
		}
		private void AdvanceSpeach()
		{
			if (this._dialogHud.CharacterSpeach.text.Length == this._text.Length && this._dialogHud.CharacterSpeach.text == this._text)
			{
				this._dialogTime = SettingsData.DialogSpeed;
				if (this._speachIndex < this._text.Length - 1f)
				{
					this._speachIndex += 1;
					this.StartCoroutine(this.TextDigitation());
				}
				else if (this._dialogIndex < this._dialogObject[this._dialogObjectIndex].Dialogs.Length - 1f)
				{
					this._speachIndex = 0;
					this._dialogIndex += 1;
					this.StartCoroutine(this.TextDigitation());
				}
				else
				{
					if (this._dialogTalk.SaveOnEspecific)
						SaveFileData.GeneralObjects.Add(this.gameObject.name);
					if (this._dialogTalk.ActivateTransition)
						this.GetComponent<TransitionController>().Transicion(this._dialogTalk.SceneToTransition);
					else if (this._dialogTalk.ActivateAnimation)
						this._animator.SetBool(this._dialogTalk.Animation, true);
					if (this._dialogTalk.DesactiveInteraction)
						this.enabled = true;
					if (!this._dialogTalk.ActivateTransition && this._dialogTalk.ActivateDestroy)
						Destroy(this.gameObject, this._dialogTalk.TimeToDestroy);
					this._text = null;
					this._speachIndex = 0;
					this._dialogIndex = 0;
					this._dialogHud.CharacterIcon.style.backgroundImage = null;
					this._dialogHud.CharacterName.text = null;
					this._dialogHud.CharacterSpeach.text = null;
					this._dialogClosed = false;
					this._dialogHud.AdvanceSpeach.clicked -= this.AdvanceSpeach;
					this._dialogHud.CloseDialog.clicked -= this.CloseDialog;
					Destroy(this._dialogHud.gameObject);
					StateController.SetState(true);
				}
			}
			else
				this._dialogTime = 0f;
		}
		private void CloseDialog()
		{
			this._text = null;
			this._speachIndex = 0;
			this._dialogIndex = 0;
			this._dialogHud.CharacterIcon.style.backgroundImage = null;
			this._dialogHud.CharacterName.text = null;
			this._dialogHud.CharacterSpeach.text = null;
			if (this._dialogClosed)
				if (this._dialogObjectIndex > 0)
					this._dialogObjectIndex -= 1;
				else if (this._dialogObjectIndex <= 0)
					this._dialogObjectIndex = (ushort)(this._dialogObject.Length - 1);
			this._dialogClosed = false;
			this._dialogHud.AdvanceSpeach.clicked -= this.AdvanceSpeach;
			this._dialogHud.CloseDialog.clicked -= this.CloseDialog;
			Destroy(this._dialogHud.gameObject);
			StateController.SetState(true);
		}
	};
};
