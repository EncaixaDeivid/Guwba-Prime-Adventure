using UnityEngine;
using UnityEngine.UIElements;
using System;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class ConfigurationHud : MonoBehaviour
	{
		private static ConfigurationHud _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _settingsGroup;
		[SerializeField, Tooltip("User interface element.")] private string _confirmationGroup;
		[SerializeField, Tooltip("User interface element.")] private string _screenResolution;
		[SerializeField, Tooltip("User interface element.")] private string _fullScreenModes;
		[SerializeField, Tooltip("User interface element.")] private string _dialogToggle;
		[SerializeField, Tooltip("User interface element.")] private string _generalVolumeToggle;
		[SerializeField, Tooltip("User interface element.")] private string _effectsVolumeToggle;
		[SerializeField, Tooltip("User interface element.")] private string _musicVolumeToggle;
		[SerializeField, Tooltip("User interface element.")] private string _dialogSpeed;
		[SerializeField, Tooltip("User interface element.")] private string _screenBrightness;
		[SerializeField, Tooltip("User interface element.")] private string _frameRate;
		[SerializeField, Tooltip("User interface element.")] private string _generalVolume;
		[SerializeField, Tooltip("User interface element.")] private string _effectsVolume;
		[SerializeField, Tooltip("User interface element.")] private string _musicVolume;
		[SerializeField, Tooltip("User interface element.")] private string _closeButton;
		[SerializeField, Tooltip("User interface element.")] private string _outLevelButton;
		[SerializeField, Tooltip("User interface element.")] private string _saveGameButton;
		[SerializeField, Tooltip("User interface element.")] private string _yesButton;
		[SerializeField, Tooltip("User interface element.")] private string _noButton;
		[SerializeField, Tooltip("User interface element.")] private string _frameRateText;
		internal GroupBox Settings { get; private set; }
		internal GroupBox Confirmation { get; private set; }
		internal DropdownField ScreenResolution { get; private set; }
		internal DropdownField FullScreenModes { get; private set; }
		internal Toggle DialogToggle { get; private set; }
		internal Toggle GeneralVolumeToggle { get; private set; }
		internal Toggle EffectsVolumeToggle { get; private set; }
		internal Toggle MusicVolumeToggle { get; private set; }
		internal Slider DialogSpeed { get; private set; }
		internal Slider ScreenBrightness { get; private set; }
		internal SliderInt FrameRate { get; private set; }
		internal SliderInt GeneralVolume { get; private set; }
		internal SliderInt EffectsVolume { get; private set; }
		internal SliderInt MusicVolume { get; private set; }
		internal Button Close { get; private set; }
		internal Button OutLevel { get; private set; }
		internal Button SaveGame { get; private set; }
		internal Button Yes { get; private set; }
		internal Button No { get; private set; }
		internal Label FrameRateText { get; private set; }
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
			this.Confirmation = root.Q<GroupBox>(this._confirmationGroup);
			this.ScreenResolution = root.Q<DropdownField>(this._screenResolution);
			this.FullScreenModes = root.Q<DropdownField>(this._fullScreenModes);
			this.DialogToggle = root.Q<Toggle>(this._dialogToggle);
			this.GeneralVolumeToggle = root.Q<Toggle>(this._generalVolumeToggle);
			this.EffectsVolumeToggle = root.Q<Toggle>(this._effectsVolumeToggle);
			this.MusicVolumeToggle = root.Q<Toggle>(this._musicVolumeToggle);
			this.FrameRate = root.Q<SliderInt>(this._frameRate);
			this.DialogSpeed = root.Q<Slider>(this._dialogSpeed);
			this.ScreenBrightness = root.Q<Slider>(this._screenBrightness);
			this.GeneralVolume = root.Q<SliderInt>(this._generalVolume);
			this.EffectsVolume = root.Q<SliderInt>(this._effectsVolume);
			this.MusicVolume = root.Q<SliderInt>(this._musicVolume);
			this.Close = root.Q<Button>(this._closeButton);
			this.OutLevel = root.Q<Button>(this._outLevelButton);
			this.SaveGame = root.Q<Button>(this._saveGameButton);
			this.Yes = root.Q<Button>(this._yesButton);
			this.No = root.Q<Button>(this._noButton);
			this.FrameRateText =  root.Q<Label>(this._frameRateText);
			if (!SettingsController.FileExists())
			{
				SettingsController.Load(out Settings saveSettings);
				SettingsController.WriteSave(saveSettings);
			}
			SettingsController.Load(out Settings settings);
			this.FrameRate.highValue = 120;
			this.DialogSpeed.highValue = .1f;
			this.ScreenBrightness.highValue = 1f;
			this.GeneralVolume.highValue = 100;
			this.EffectsVolume.highValue = 100;
			this.MusicVolume.highValue = 100;
			this.FrameRate.lowValue = 30;
			this.DialogSpeed.lowValue = 0f;
			this.ScreenBrightness.lowValue = 0f;
			this.GeneralVolume.lowValue = 0;
			this.EffectsVolume.lowValue = 0;
			this.MusicVolume.lowValue = 0;
			foreach (Resolution resolution in SettingsController.PixelPerfectResolutions())
				this.ScreenResolution.choices.Add($@"{resolution.width} x {resolution.height}");
			foreach (FullScreenMode mode in Enum.GetValues(typeof(FullScreenMode)))
				this.FullScreenModes.choices.Add(mode.ToString());
			this.ScreenResolution.value = $@"{settings.ScreenResolution.x} x {settings.ScreenResolution.y}";
			this.FullScreenModes.value = settings.FullScreenMode.ToString();
			this.DialogToggle.value = settings.DialogToggle;
			this.GeneralVolumeToggle.value = settings.GeneralVolumeToggle;
			this.EffectsVolumeToggle.value = settings.EffectsVolumeToggle;
			this.MusicVolumeToggle.value = settings.MusicVolumeToggle;
			this.DialogSpeed.value = settings.DialogSpeed;
			this.ScreenBrightness.value = settings.ScreenBrightness;
			this.FrameRate.value = settings.FrameRate;
			this.GeneralVolume.value = settings.GeneralVolume;
			this.EffectsVolume.value = settings.EffectsVolume;
			this.MusicVolume.value = settings.MusicVolume;
			this.FrameRateText.text = settings.FrameRate.ToString();
		}
	};
};
