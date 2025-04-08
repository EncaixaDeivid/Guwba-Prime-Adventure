using UnityEngine;
using System.IO;
namespace GuwbaPrimeAdventure.Data
{
	public struct Settings
	{
		public bool fullScreen;
		public bool generalVolumeToggle;
		[Range(0f, 1f)] public float generalVolume;
		public bool effectsVolumeToggle;
		[Range(0f, 1f)] public float effectsVolume;
		public bool musicVolumeToggle;
		[Range(0f, 1f)] public float musicVolume;
		public bool dialogToggle;
		[Range(0f, .1f)] public float dialogSpeed;
	};
	public enum ToggleSettings
	{
		FullScreen,
		GeneralVolumeToggle,
		EffectsVolumeToggle,
		MusicVolumeToggle,
		DialogToggle,
	};
	public enum RangeSettings
	{
		GeneralVolume,
		EffectsVolume,
		MusicVolume,
		DialogSpeed
	};
	public static class SettingsController
	{
		private static Settings _settings = new();
		private static readonly string SettingsPath = $@"{Application.persistentDataPath}\Settings.txt";
		public static void Load(out Settings settings)
		{
			settings = new Settings()
			{
				fullScreen = true,
				generalVolumeToggle = true,
				generalVolume = 1f,
				effectsVolumeToggle = true,
				effectsVolume = 1f,
				musicVolumeToggle = true,
				musicVolume = 1f,
				dialogToggle = true,
				dialogSpeed = .05f
			};
			if (File.Exists(SettingsPath))
				settings = ArchiveEncoder.ReadData<Settings>(SettingsPath);
		}
		public static void WriteSave(ToggleSettings toggleSettings, bool settingsValue)
		{
			switch (toggleSettings)
			{
				case ToggleSettings.FullScreen:
					_settings.fullScreen = settingsValue;
					break;
				case ToggleSettings.GeneralVolumeToggle:
					_settings.generalVolumeToggle = settingsValue;
					break;
				case ToggleSettings.EffectsVolumeToggle:
					_settings.effectsVolumeToggle = settingsValue;
					break;
				case ToggleSettings.MusicVolumeToggle:
					_settings.musicVolumeToggle = settingsValue;
					break;
				case ToggleSettings.DialogToggle:
					_settings.dialogToggle = settingsValue;
					break;
			}
		}
		public static void WriteSave(RangeSettings rangeSettings, float settingsValue)
		{
			switch (rangeSettings)
			{
				case RangeSettings.GeneralVolume:
					_settings.generalVolume = settingsValue;
					break;
				case RangeSettings.EffectsVolume:
					_settings.effectsVolume = settingsValue;
					break;
				case RangeSettings.MusicVolume:
					_settings.musicVolume = settingsValue;
					break;
				case RangeSettings.DialogSpeed:
					_settings.dialogSpeed = settingsValue;
					break;
			}
		}
		public static void SaveSettings() =>
			ArchiveEncoder.WriteData(new Settings()
			{
				fullScreen = _settings.fullScreen,
				generalVolumeToggle = _settings.generalVolumeToggle,
				generalVolume = _settings.generalVolume,
				effectsVolumeToggle = _settings.effectsVolumeToggle,
				effectsVolume = _settings.effectsVolume,
				musicVolumeToggle = _settings.musicVolumeToggle,
				musicVolume = _settings.musicVolume,
				dialogToggle = _settings.dialogToggle,
				dialogSpeed = _settings.dialogSpeed
			}, SettingsPath);
	};
};
