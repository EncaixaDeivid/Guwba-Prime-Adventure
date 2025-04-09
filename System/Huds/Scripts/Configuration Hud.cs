using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	public sealed class ConfigurationHud : MonoBehaviour
	{
		private static ConfigurationHud _instance;
		private GroupBox _confirmation, _buttons;
		private Button _close, _outLevel, _yes, _no, _saveGame;
		private Slider _generalVolume, _effectsVolume, _musicVolume, _dialogSpeed;
		private Toggle _fullscreen, _generalVolumeToggle, _effectsVolumeToggle, _musicVolumeToggle, _dialogToggle;
		[SerializeField] private string[] _volumes, _toggles;
		[SerializeField] private string _confirmationGroup, _outLevelButton, _yesButton, _noButton, _buttonsGroup, _saveGameButton, _closeButton;
		public GroupBox Buttons => this._buttons;
		public Button Close => this._close;
		public Button OutLevel => this._outLevel;
		public Button SaveGame => this._saveGame;
		public (Slider GeneralVolume, Slider EffectsVolume) Volumes1 => (this._generalVolume, this._effectsVolume);
		public (Slider MusicVolume, Slider DialogSpeed) Volumes2 => (this._musicVolume, this._dialogSpeed);
		public (Toggle Fullscreen, Toggle GeneralVolumeToggle) Toggles1 => (this._fullscreen, this._generalVolumeToggle);
		public (Toggle EffectsVolumeToggle, Toggle MusicVolumeToggle) Toggles2 => (this._effectsVolumeToggle, this._musicVolumeToggle);
		public Toggle DialogToggle => this._dialogToggle;
		public GroupBox Confirmation => this._confirmation;
		public Button Yes => this._yes;
		public Button No => this._no;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this._buttons = root.Q<GroupBox>(this._buttonsGroup);
			this._close = root.Q<Button>(this._closeButton);
			this._outLevel = root.Q<Button>(this._outLevelButton);
			this._saveGame = root.Q<Button>(this._saveGameButton);
			this._generalVolume = root.Q<Slider>(this._volumes[0]);
			this._effectsVolume = root.Q<Slider>(this._volumes[1]);
			this._musicVolume = root.Q<Slider>(this._volumes[2]);
			this._dialogSpeed = root.Q<Slider>(this._volumes[3]);
			this._fullscreen = root.Q<Toggle>(this._toggles[0]);
			this._generalVolumeToggle = root.Q<Toggle>(this._toggles[1]);
			this._effectsVolumeToggle = root.Q<Toggle>(this._toggles[2]);
			this._musicVolumeToggle = root.Q<Toggle>(this._toggles[3]);
			this._dialogToggle = root.Q<Toggle>(this._toggles[4]);
			this._confirmation = root.Q<GroupBox>(this._confirmationGroup);
			this._yes = root.Q<Button>(this._yesButton);
			this._no = root.Q<Button>(this._noButton);
		}
	};
};
