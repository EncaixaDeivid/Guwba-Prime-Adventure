using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class MenuHud : MonoBehaviour
	{
		private static MenuHud _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _buttonsGroup;
		[SerializeField, Tooltip("User interface element.")] private string _savesGroup;
		[SerializeField, Tooltip("User interface element.")] private string _playButton;
		[SerializeField, Tooltip("User interface element.")] private string _configurationsButton;
		[SerializeField, Tooltip("User interface element.")] private string _quitButton;
		[SerializeField, Tooltip("User interface element.")] private string _backButton;
		[SerializeField, Tooltip("User interface element.")] private string _saveNameTextField;
		[SerializeField, Tooltip("User interface element.")] private string _loadButton;
		[SerializeField, Tooltip("User interface element.")] private string _deleteButton;
		internal GroupBox Buttons { get; private set; }
		internal GroupBox Saves { get; private set; }
		internal Button Play { get; private set; }
		internal Button Configurations { get; private set; }
		internal Button Quit { get; private set; }
		internal Button Back { get; private set; }
		internal TextField[] SaveName { get; private set; }
		internal Button[] Load { get; private set; }
		internal Button[] Delete { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			VisualElement root = GetComponent<UIDocument>().rootVisualElement;
			Buttons = root.Q<GroupBox>(_buttonsGroup);
			Play = root.Q<Button>(_playButton);
			Configurations = root.Q<Button>(_configurationsButton);
			Quit = root.Q<Button>(_quitButton);
			Saves = root.Q<GroupBox>(_savesGroup);
			Back = root.Q<Button>(_backButton);
			SaveName = new TextField[4];
			for (ushort i = 0; i < SaveName.Length; i++)
				SaveName[i] = root.Q<TextField>(_saveNameTextField + $"{i + 1}");
			Load = new Button[4];
			for (ushort i = 0; i < Load.Length; i++)
				Load[i] = root.Q<Button>(_loadButton + $"{i + 1}");
			Delete = new Button[4];
			for (ushort i = 0; i < Delete.Length; i++)
				Delete[i] = root.Q<Button>(_deleteButton + $"{i + 1}");
		}
	};
};
