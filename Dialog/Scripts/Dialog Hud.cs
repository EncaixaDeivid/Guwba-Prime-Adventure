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
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.CharacterIcon = root.Q<VisualElement>(this._characterIcon);
			this.CharacterName = root.Q<Label>(this._characterName);
			this.CharacterSpeach = root.Q<Label>(this._characterSpeach);
			this.CloseDialog = root.Q<Button>(this._closeDialog);
			this.AdvanceSpeach = root.Q<Button>(this._advanceSpeach);
		}
	};
};
