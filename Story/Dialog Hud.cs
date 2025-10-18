using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Story
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class DialogHud : MonoBehaviour
	{
		static private DialogHud _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _characterIcon;
		[SerializeField, Tooltip("User interface element.")] private string _characterName;
		[SerializeField, Tooltip("User interface element.")] private string _characterSpeach;
		[SerializeField, Tooltip("User interface element.")] private string _advanceSpeach;
		internal VisualElement RootElement { get; private set; }
		internal VisualElement CharacterIcon { get; private set; }
		internal Label CharacterName { get; private set; }
		internal Label CharacterSpeach { get; private set; }
		internal Button AdvanceSpeach { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			RootElement = GetComponent<UIDocument>().rootVisualElement;
			CharacterIcon = RootElement.Q<VisualElement>(_characterIcon);
			CharacterName = RootElement.Q<Label>(_characterName);
			CharacterSpeach = RootElement.Q<Label>(_characterSpeach);
			AdvanceSpeach = RootElement.Q<Button>(_advanceSpeach);
		}
	};
};
