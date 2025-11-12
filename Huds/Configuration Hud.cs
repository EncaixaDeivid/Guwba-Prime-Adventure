using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using GwambaPrimeAdventure.Data;
namespace GwambaPrimeAdventure.Hud
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
		[SerializeField, Tooltip("User interface element.")] private string _inifinityFPSToggle;
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
		internal VisualElement RootElement { get; private set; }
		internal GroupBox Settings { get; private set; }
		internal GroupBox Confirmation { get; private set; }
		internal DropdownField ScreenResolution { get; private set; }
		internal DropdownField FullScreenModes { get; private set; }
		internal Toggle DialogToggle { get; private set; }
		internal Toggle GeneralVolumeToggle { get; private set; }
		internal Toggle EffectsVolumeToggle { get; private set; }
		internal Toggle MusicVolumeToggle { get; private set; }
		internal Toggle InfinityFPS { get; private set; }
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
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			RootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("RootElement");
			Settings = RootElement.Q<GroupBox>(_settingsGroup);
			Confirmation = RootElement.Q<GroupBox>(_confirmationGroup);
			ScreenResolution = RootElement.Q<DropdownField>(_screenResolution);
			FullScreenModes = RootElement.Q<DropdownField>(_fullScreenModes);
			DialogToggle = RootElement.Q<Toggle>(_dialogToggle);
			GeneralVolumeToggle = RootElement.Q<Toggle>(_generalVolumeToggle);
			EffectsVolumeToggle = RootElement.Q<Toggle>(_effectsVolumeToggle);
			MusicVolumeToggle = RootElement.Q<Toggle>(_musicVolumeToggle);
			InfinityFPS = RootElement.Q<Toggle>(_inifinityFPSToggle);
			DialogSpeed = RootElement.Q<Slider>(_dialogSpeed);
			ScreenBrightness = RootElement.Q<Slider>(_screenBrightness);
			FrameRate = RootElement.Q<SliderInt>(_frameRate);
			GeneralVolume = RootElement.Q<SliderInt>(_generalVolume);
			EffectsVolume = RootElement.Q<SliderInt>(_effectsVolume);
			MusicVolume = RootElement.Q<SliderInt>(_musicVolume);
			Close = RootElement.Q<Button>(_closeButton);
			OutLevel = RootElement.Q<Button>(_outLevelButton);
			SaveGame = RootElement.Q<Button>(_saveGameButton);
			Yes = RootElement.Q<Button>(_yesButton);
			No = RootElement.Q<Button>(_noButton);
			FrameRateText =  RootElement.Q<Label>(_frameRateText);
		}
		private IEnumerator Start()
		{
			if (!_instance || _instance != this)
				yield break;
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			SettingsController.Load(out Settings settings);
			if (!SettingsController.FileExists())
				SettingsController.WriteSave(settings);
			DialogSpeed.highValue = .1f;
			ScreenBrightness.highValue = 1f;
			FrameRate.highValue = 240;
			GeneralVolume.highValue = 100;
			EffectsVolume.highValue = 100;
			MusicVolume.highValue = 100;
			DialogSpeed.lowValue = 0f;
			ScreenBrightness.lowValue = 0f;
			FrameRate.lowValue = 10;
			GeneralVolume.lowValue = 0;
			EffectsVolume.lowValue = 0;
			MusicVolume.lowValue = 0;
			foreach (Resolution resolution in SettingsController.PixelPerfectResolutions())
				ScreenResolution.choices.Add($@"{resolution.width} x {resolution.height}");
			foreach (FullScreenMode mode in Enum.GetValues(typeof(FullScreenMode)))
				FullScreenModes.choices.Add(mode.ToString());
			ScreenResolution.value = $@"{settings.ScreenResolution.x} x {settings.ScreenResolution.y}";
			FullScreenModes.value = settings.FullScreenMode.ToString();
			DialogToggle.value = settings.DialogToggle;
			GeneralVolumeToggle.value = settings.GeneralVolumeToggle;
			EffectsVolumeToggle.value = settings.EffectsVolumeToggle;
			MusicVolumeToggle.value = settings.MusicVolumeToggle;
			InfinityFPS.value = settings.InfinityFPS;
			DialogSpeed.value = settings.DialogSpeed;
			ScreenBrightness.value = settings.ScreenBrightness;
			FrameRate.value = settings.FrameRate;
			GeneralVolume.value = settings.GeneralVolume;
			EffectsVolume.value = settings.EffectsVolume;
			MusicVolume.value = settings.MusicVolume;
			FrameRateText.text = settings.FrameRate.ToString();
		}
	};
};
