using UnityEngine;
using System.IO;
namespace GuwbaPrimeAdventure
{
	public struct Settings
	{
		private bool _fullScreen;
		private bool _generalVolumeToggle;
		[Range(0f, 1f)] private float _generalVolume;
		private bool _effectsVolumeToggle;
		[Range(0f, 1f)] private float _effectsVolume;
		private bool _musicVolumeToggle;
		[Range(0f, 1f)] private float _musicVolume;
		[Range(0f, .1f)] private float _dialogSpeed;
		private static readonly string SettingsPath = Application.persistentDataPath + "/Settings.txt";
		private static Settings LoadSettings()
		{
			if (File.Exists(SettingsPath))
				return DataController.ReadData<Settings>(SettingsPath);
			return new Settings()
			{
				_fullScreen = true,
				_generalVolumeToggle = true,
				_generalVolume = 1f,
				_effectsVolumeToggle = true,
				_effectsVolume = 1f,
				_musicVolumeToggle = true,
				_musicVolume = 1f,
				_dialogSpeed = .05f
			};
		}
		internal static void SaveSettings()
		{
			Settings settings = LoadSettings();
			settings._fullScreen = FullScreen;
			settings._generalVolumeToggle = GeneralVolumeToggle;
			settings._generalVolume = GeneralVolume;
			settings._effectsVolumeToggle = EffectsVolumeToggle;
			settings._effectsVolume = EffectsVolume;
			settings._musicVolumeToggle = MusicVolumeToggle;
			settings._musicVolume = MusicVolume;
			settings._dialogSpeed = DialogSpeed;
			DataController.WriteData(settings, SettingsPath);
		}
		public static bool FullScreen = LoadSettings()._fullScreen;
		public static bool GeneralVolumeToggle = LoadSettings()._generalVolumeToggle;
		public static float GeneralVolume = LoadSettings()._generalVolume;
		public static bool EffectsVolumeToggle = LoadSettings()._effectsVolumeToggle;
		public static float EffectsVolume = LoadSettings()._effectsVolume;
		public static bool MusicVolumeToggle = LoadSettings()._musicVolumeToggle;
		public static float MusicVolume = LoadSettings()._musicVolume;
		public static float DialogSpeed = LoadSettings()._dialogSpeed;
	};
};
