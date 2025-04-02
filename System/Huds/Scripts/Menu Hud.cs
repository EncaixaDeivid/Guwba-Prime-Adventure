using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	public sealed class MenuHud : MonoBehaviour
	{
		private static MenuHud _instance;
		private GroupBox _buttons, _saves;
		private Button _play, _configurations, _quit, _back;
		private TextField[] _saveName;
		private Button[] _load, _delete;
		[SerializeField] private string
			_buttonsGroup,
			_savesGroup,
			_playButton,
			_configurationsButton,
			_quitButton,
			_backButton,
			_saveNameTextField,
			_loadButton,
			_deleteButton;
		public GroupBox Buttons => this._buttons;
		public GroupBox Saves => this._saves;
		public Button Play => this._play;
		public Button Configurations => this._configurations;
		public Button Quit => this._quit;
		public Button Back => this._back;
		public TextField[] SaveName => this._saveName;
		public Button[] Load => this._load;
		public Button[] Delete => this._delete;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.0001f);
				return;
			}
			_instance = this;
			UIDocument hudDocument = this.GetComponent<UIDocument>();
			this._buttons = hudDocument.rootVisualElement.Q<GroupBox>(this._buttonsGroup);
			this._play = hudDocument.rootVisualElement.Q<Button>(this._playButton);
			this._configurations = hudDocument.rootVisualElement.Q<Button>(this._configurationsButton);
			this._quit = hudDocument.rootVisualElement.Q<Button>(this._quitButton);
			this._saves = hudDocument.rootVisualElement.Q<GroupBox>(this._savesGroup);
			this._back = hudDocument.rootVisualElement.Q<Button>(this._backButton);
			this._saveName = new TextField[4];
			for (ushort i = 0; i < this._saveName.Length; i++)
				this._saveName[i] = hudDocument.rootVisualElement.Q<TextField>(this._saveNameTextField + $"{i + 1f}");
			this._load = new Button[4];
			for (ushort i = 0; i < this._load.Length; i++)
				this._load[i] = hudDocument.rootVisualElement.Q<Button>(this._loadButton + $"{i + 1f}");
			this._delete = new Button[4];
			for (ushort i = 0; i < this._delete.Length; i++)
				this._delete[i] = hudDocument.rootVisualElement.Q<Button>(this._deleteButton + $"{i + 1f}");
		}
	};
};
