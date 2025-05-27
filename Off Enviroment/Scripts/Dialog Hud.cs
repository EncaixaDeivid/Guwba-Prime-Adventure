using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.OffEnviroment
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class DialogHud : MonoBehaviour
	{
		static private DialogHud _instance;
		[SerializeField] private string _rootElementVisual;
		[SerializeField] private string _characterIcon;
		[SerializeField] private string _characterName;
		[SerializeField] private string _characterSpeach;
		[SerializeField] private string _advanceSpeach;
		internal VisualElement RootElement { get; private set; }
		internal VisualElement CharacterIcon { get; private set; }
		internal Label CharacterName { get; private set; }
		internal Label CharacterSpeach { get; private set; }
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
			this.RootElement = root.Q<VisualElement>(this._rootElementVisual);
			this.CharacterIcon = root.Q<VisualElement>(this._characterIcon);
			this.CharacterName = root.Q<Label>(this._characterName);
			this.CharacterSpeach = root.Q<Label>(this._characterSpeach);
			this.AdvanceSpeach = root.Q<Button>(this._advanceSpeach);
		}
	};
};