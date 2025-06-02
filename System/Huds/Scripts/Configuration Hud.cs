using UnityEngine;
using UnityEngine.UIElements;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class ConfigurationHud : MonoBehaviour
	{
		private static ConfigurationHud _instance;
		[SerializeField, Tooltip("User interface element.")] private string[] _volumes;
		[SerializeField, Tooltip("User interface element.")] private string[] _toggles;
		[SerializeField, Tooltip("User interface element.")] private string _confirmationGroup;
		[SerializeField, Tooltip("User interface element.")] private string _outLevelButton;
		[SerializeField, Tooltip("User interface element.")] private string _yesButton;
		[SerializeField, Tooltip("User interface element.")] private string _noButton;
		[SerializeField, Tooltip("User interface element.")] private string _settingsGroup;
		[SerializeField, Tooltip("User interface element.")] private string _saveGameButton;
		[SerializeField, Tooltip("User interface element.")] private string _closeButton;
		internal GroupBox Settings { get; private set; }
		internal Button Close { get; private set; }
		internal Button OutLevel { get; private set; }
		internal Button SaveGame { get; private set; }
		internal Slider GeneralVolume { get; private set; }
		internal Slider EffectsVolume { get; private set; }
		internal Slider MusicVolume { get; private set; }
		internal Slider DialogSpeed { get; private set; }
		internal Toggle FullScreen { get; private set; }
		internal Toggle GeneralVolumeToggle { get; private set; }
		internal Toggle EffectsVolumeToggle { get; private set; }
		internal Toggle MusicVolumeToggle { get; private set; }
		internal Toggle DialogToggle { get; private set; }
		internal GroupBox Confirmation { get; private set; }
		internal Button Yes { get; private set; }
		internal Button No { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.Settings = root.Q<GroupBox>(this._settingsGroup);
			this.Close = root.Q<Button>(this._closeButton);
			this.OutLevel = root.Q<Button>(this._outLevelButton);
			this.SaveGame = root.Q<Button>(this._saveGameButton);
			this.GeneralVolume = root.Q<Slider>(this._volumes[0]);
			this.EffectsVolume = root.Q<Slider>(this._volumes[1]);
			this.MusicVolume = root.Q<Slider>(this._volumes[2]);
			this.DialogSpeed = root.Q<Slider>(this._volumes[3]);
			this.FullScreen = root.Q<Toggle>(this._toggles[0]);
			this.GeneralVolumeToggle = root.Q<Toggle>(this._toggles[1]);
			this.EffectsVolumeToggle = root.Q<Toggle>(this._toggles[2]);
			this.MusicVolumeToggle = root.Q<Toggle>(this._toggles[3]);
			this.DialogToggle = root.Q<Toggle>(this._toggles[4]);
			this.Confirmation = root.Q<GroupBox>(this._confirmationGroup);
			this.Yes = root.Q<Button>(this._yesButton);
			this.No = root.Q<Button>(this._noButton);
			if (!SettingsController.FileExists())
				SettingsController.SaveSettings();
			SettingsController.Load(out Settings settings);
			this.GeneralVolume.highValue = 100;
			this.EffectsVolume.highValue = 100;
			this.MusicVolume.highValue = 100;
			this.DialogSpeed.highValue = .1f;
			this.GeneralVolume.lowValue = 0;
			this.EffectsVolume.lowValue = 0;
			this.MusicVolume.lowValue = 0;
			this.DialogSpeed.lowValue = 0f;
			this.GeneralVolume.value = settings.generalVolume;
			this.EffectsVolume.value = settings.effectsVolume;
			this.MusicVolume.value = settings.musicVolume;
			this.DialogSpeed.value = settings.dialogSpeed;
			this.FullScreen.value = settings.fullScreen;
			this.GeneralVolumeToggle.value = settings.generalVolumeToggle;
			this.EffectsVolumeToggle.value = settings.effectsVolumeToggle;
			this.MusicVolumeToggle.value = settings.musicVolumeToggle;
			this.DialogToggle.value = settings.dialogToggle;
		}
	};
};
