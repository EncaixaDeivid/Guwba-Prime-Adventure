using UnityEngine;
using UnityEngine.UIElements;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	public sealed class ConfigurationHud : MonoBehaviour
	{
		private static ConfigurationHud _instance;
		private GroupBox _confirmation, _settings;
		private Button _close, _outLevel, _yes, _no, _saveGame;
		private Slider _generalVolume, _effectsVolume, _musicVolume, _dialogSpeed;
		private Toggle _fullScreen, _generalVolumeToggle, _effectsVolumeToggle, _musicVolumeToggle, _dialogToggle;
		[SerializeField] private string[] _volumes, _toggles;
		[SerializeField] private string _confirmationGroup, _outLevelButton, _yesButton, _noButton, _settingsGroup, _saveGameButton, _closeButton;
		public GroupBox Settings => this._settings;
		public Button Close => this._close;
		public Button OutLevel => this._outLevel;
		public Button SaveGame => this._saveGame;
		public (Slider GeneralVolume, Slider EffectsVolume) Volumes1 => (this._generalVolume, this._effectsVolume);
		public (Slider MusicVolume, Slider DialogSpeed) Volumes2 => (this._musicVolume, this._dialogSpeed);
		public (Toggle FullScreen, Toggle GeneralVolumeToggle) Toggles1 => (this._fullScreen, this._generalVolumeToggle);
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
			this._settings = root.Q<GroupBox>(this._settingsGroup);
			this._close = root.Q<Button>(this._closeButton);
			this._outLevel = root.Q<Button>(this._outLevelButton);
			this._saveGame = root.Q<Button>(this._saveGameButton);
			this._generalVolume = root.Q<Slider>(this._volumes[0]);
			this._effectsVolume = root.Q<Slider>(this._volumes[1]);
			this._musicVolume = root.Q<Slider>(this._volumes[2]);
			this._dialogSpeed = root.Q<Slider>(this._volumes[3]);
			this._fullScreen = root.Q<Toggle>(this._toggles[0]);
			this._generalVolumeToggle = root.Q<Toggle>(this._toggles[1]);
			this._effectsVolumeToggle = root.Q<Toggle>(this._toggles[2]);
			this._musicVolumeToggle = root.Q<Toggle>(this._toggles[3]);
			this._dialogToggle = root.Q<Toggle>(this._toggles[4]);
			this._confirmation = root.Q<GroupBox>(this._confirmationGroup);
			this._yes = root.Q<Button>(this._yesButton);
			this._no = root.Q<Button>(this._noButton);
			if (SettingsController.FileExists())
				SettingsController.SaveSettings();
			SettingsController.Load(out Settings settings);
			this._generalVolume.highValue = 100;
			this._effectsVolume.highValue = 100;
			this._musicVolume.highValue = 100;
			this._dialogSpeed.highValue = .1f;
			this._generalVolume.lowValue = 0;
			this._effectsVolume.lowValue = 0;
			this._musicVolume.lowValue = 0;
			this._dialogSpeed.lowValue = 0f;
			this._generalVolume.value = settings.generalVolume;
			this._effectsVolume.value = settings.effectsVolume;
			this._musicVolume.value = settings.musicVolume;
			this._dialogSpeed.value = settings.dialogSpeed;
			this._fullScreen.value = settings.fullScreen;
			this._generalVolumeToggle.value = settings.generalVolumeToggle;
			this._effectsVolumeToggle.value = settings.effectsVolumeToggle;
			this._musicVolumeToggle.value = settings.musicVolumeToggle;
			this._dialogToggle.value = settings.dialogToggle;
		}
	};
};
