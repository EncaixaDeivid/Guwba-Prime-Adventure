using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Story
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class InteractiveDialog : MonoBehaviour, IInteractable
    {
		private DialogHud _dialogHud;
		private StoryTeller _storyTeller;
		private Animator _animator;
		private readonly Sender _sender = Sender.Create();
		private string _text = "";
		private ushort _speachIndex = 0;
		private float _dialogTime = 0f;
		private bool _nextSlide = false;
		[Header("Interaction Objects")]
		[SerializeField, Tooltip("The object that handles the hud of the dialog.")] private DialogHud _dialogHudObject;
		[SerializeField, Tooltip("The collection of the object that contais the dialog.")] private DialogObject _dialogObject;
		private void Awake()
		{
			this._sender.SetToWhereConnection(PathConnection.Item);
			this._sender.SetStateForm(StateForm.Action);
			this._sender.SetAdditionalData(this.transform);
		}
		public void Interaction()
		{
			if (this._dialogObject && this._dialogHudObject)
			{
				this._sender.SetToggle(false);
				this._sender.Send();
				SettingsController.Load(out Settings settings);
				StateController.SetState(false);
				this._storyTeller = this.GetComponent<StoryTeller>();
				this._animator = this.GetComponent<Animator>();
				this._dialogHud = Instantiate(this._dialogHudObject);
				this._dialogTime = settings.dialogSpeed;
				this._dialogHud.AdvanceSpeach.clicked += this.AdvanceSpeach;
				this.StartCoroutine(this.TextDigitation());
				if (this._storyTeller)
					this._storyTeller.ShowScene();
			}
		}
		private IEnumerator TextDigitation()
		{
			StyleBackground image = new(this._dialogObject.Speachs[this._speachIndex].Model);
			this._dialogHud.CharacterIcon.style.backgroundImage = image;
			this._dialogHud.CharacterName.text = this._dialogObject.Speachs[this._speachIndex].CharacterName;
			this._text = this._dialogObject.Speachs[this._speachIndex].SpeachText;
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
		private void AdvanceSpeach()
		{
			if (this._dialogHud.CharacterSpeach.text.Length == this._text.Length && this._dialogHud.CharacterSpeach.text == this._text)
			{
				SettingsController.Load(out Settings settings);
				this._dialogTime = settings.dialogSpeed;
				if (this._speachIndex < this._dialogObject.Speachs.Length - 1f)
				{
					if (this._storyTeller && this._dialogObject.Speachs[this._speachIndex].NextSlide)
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
					this._sender.SetToggle(true);
					this._sender.Send();
					SaveController.Load(out SaveFile saveFile);
					if (this._dialogObject.SaveOnEspecific && !saveFile.generalObjects.Contains(this.gameObject.name))
					{
						saveFile.generalObjects.Add(this.gameObject.name);
						SaveController.WriteSave(saveFile);
					}
					if (this._dialogObject.ActivateTransition)
						this.GetComponent<Transitioner>().Transicion(this._dialogObject.SceneToTransition);
					else if (this._dialogObject.ActivateAnimation)
						this._animator.SetTrigger(this._dialogObject.Animation);
					if (this._dialogObject.DesactiveInteraction)
						this.enabled = false;
					if (!this._dialogObject.ActivateTransition && this._dialogObject.ActivateDestroy)
						Destroy(this.gameObject, this._dialogObject.TimeToDestroy);
				}
			}
			else
				this._dialogTime = 0f;
		}
	};
};
