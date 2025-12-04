using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class ConfigurationHud : MonoBehaviour
	{
		private static ConfigurationHud _instance;
		internal VisualElement RootElement { get; private set; }
		internal GroupBox Settings { get; private set; }
		internal GroupBox ConfirmationButtons { get; private set; }
		internal DropdownField ScreenResolution { get; private set; }
		internal DropdownField FullScreenModes { get; private set; }
		internal Toggle DialogToggle { get; private set; }
		internal Toggle GeneralVolumeToggle { get; private set; }
		internal Toggle EffectsVolumeToggle { get; private set; }
		internal Toggle MusicVolumeToggle { get; private set; }
		internal Toggle InfinityFPS { get; private set; }
		internal Slider DialogSpeed { get; private set; }
		internal Slider ScreenBrightness { get; private set; }
		internal Slider GeneralVolume { get; private set; }
		internal Slider EffectsVolume { get; private set; }
		internal Slider MusicVolume { get; private set; }
		internal SliderInt FrameRate { get; private set; }
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
				Destroy(gameObject, WorldBuild.MINIMUM_TIME_SPACE_LIMIT);
				return;
			}
			_instance = this;
			RootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>(nameof(RootElement));
			Settings = RootElement.Q<GroupBox>(nameof(Settings));
			ConfirmationButtons = RootElement.Q<GroupBox>(nameof(ConfirmationButtons));
			ScreenResolution = RootElement.Q<DropdownField>(nameof(ScreenResolution));
			FullScreenModes = RootElement.Q<DropdownField>(nameof(FullScreenModes));
			DialogToggle = RootElement.Q<Toggle>(nameof(DialogToggle));
			GeneralVolumeToggle = RootElement.Q<Toggle>(nameof(GeneralVolumeToggle));
			EffectsVolumeToggle = RootElement.Q<Toggle>(nameof(EffectsVolumeToggle));
			MusicVolumeToggle = RootElement.Q<Toggle>(nameof(MusicVolumeToggle));
			InfinityFPS = RootElement.Q<Toggle>(nameof(InfinityFPS));
			DialogSpeed = RootElement.Q<Slider>(nameof(DialogSpeed));
			ScreenBrightness = RootElement.Q<Slider>(nameof(ScreenBrightness));
			GeneralVolume = RootElement.Q<Slider>(nameof(GeneralVolume));
			EffectsVolume = RootElement.Q<Slider>(nameof(EffectsVolume));
			MusicVolume = RootElement.Q<Slider>(nameof(MusicVolume));
			FrameRate = RootElement.Q<SliderInt>(nameof(FrameRate));
			Close = RootElement.Q<Button>(nameof(Close));
			OutLevel = RootElement.Q<Button>(nameof(OutLevel));
			SaveGame = RootElement.Q<Button>(nameof(SaveGame));
			Yes = RootElement.Q<Button>(nameof(Yes));
			No = RootElement.Q<Button>(nameof(No));
			FrameRateText =  RootElement.Q<Label>(nameof(FrameRateText));
		}
		private IEnumerator Start()
		{
			if (!_instance || _instance != this)
				yield break;
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			SettingsController.Load(out Settings settings);
			if (!SettingsController.FileExists())
				SettingsController.WriteSave(settings);
			DialogSpeed.highValue = 1E-1F;
			ScreenBrightness.highValue = 1F;
			GeneralVolume.highValue = 1F;
			EffectsVolume.highValue = 1F;
			MusicVolume.highValue = 1F;
			FrameRate.highValue = 240;
			DialogSpeed.lowValue = 0F;
			ScreenBrightness.lowValue = 0F;
			GeneralVolume.lowValue = 1E-4F;
			EffectsVolume.lowValue = 1E-4F;
			MusicVolume.lowValue = 1E-4F;
			FrameRate.lowValue = 10;
			foreach (Resolution resolution in WorldBuild.PixelPerfectResolutions())
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
