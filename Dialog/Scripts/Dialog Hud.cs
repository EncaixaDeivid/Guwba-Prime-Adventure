using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Dialog
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class DialogHud : MonoBehaviour
	{
		static private DialogHud _instance;
		[SerializeField] private string _characterIcon, _characterName, _characterSpeach, _closeDialog, _advanceSpeach;
		internal VisualElement CharacterIcon { get; private set; }
		internal Label CharacterName { get; private set; }
		internal Label CharacterSpeach { get; private set; }
		internal Button CloseDialog { get; private set; }
		internal Button AdvanceSpeach { get; private set; }
		private void Awake()
		{
			if (_instance)
				Destroy(_instance.gameObject);
			_instance = this;
			UIDocument hudDocument = this.GetComponent<UIDocument>();
			this.CharacterIcon = hudDocument.rootVisualElement.Q<VisualElement>(this._characterIcon);
			this.CharacterName = hudDocument.rootVisualElement.Q<Label>(this._characterName);
			this.CharacterSpeach = hudDocument.rootVisualElement.Q<Label>(this._characterSpeach);
			this.CloseDialog = hudDocument.rootVisualElement.Q<Button>(this._closeDialog);
			this.AdvanceSpeach = hudDocument.rootVisualElement.Q<Button>(this._advanceSpeach);
		}
	};
};