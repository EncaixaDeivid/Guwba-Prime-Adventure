using UnityEngine;
using System.IO;
namespace GuwbaPrimeAdventure
{
	public static class SettingsData
	{
		private static readonly string SettingsPath = Application.persistentDataPath + "/Settings.txt";
		private struct Settings
		{
			public bool FullScreen;
			public bool GeneralVolumeToggle;
			[Range(0f, 1f)] public float GeneralVolume;
			public bool EffectsVolumeToggle;
			[Range(0f, 1f)] public float EffectsVolume;
			public bool MusicVolumeToggle;
			[Range(0f, 1f)] public float MusicVolume;
			[Range(0f, .1f)] public float DialogSpeed;
		};
		public static bool FullScreen = LoadSettings().FullScreen;
		public static bool GeneralVolumeToggle = LoadSettings().GeneralVolumeToggle;
		public static float GeneralVolume = LoadSettings().GeneralVolume;
		public static bool EffectsVolumeToggle = LoadSettings().EffectsVolumeToggle;
		public static float EffectsVolume = LoadSettings().EffectsVolume;
		public static bool MusicVolumeToggle = LoadSettings().MusicVolumeToggle;
		public static float MusicVolume = LoadSettings().MusicVolume;
		public static float DialogSpeed = LoadSettings().DialogSpeed;
		private static Settings LoadSettings()
		{
			if (File.Exists(SettingsPath))
				return DataController.ReadData<Settings>(SettingsPath);
			return new Settings()
			{
				FullScreen = true,
				GeneralVolumeToggle = true,
				GeneralVolume = 1f,
				EffectsVolumeToggle = true,
				EffectsVolume = 1f,
				MusicVolumeToggle = true,
				MusicVolume = 1f,
				DialogSpeed = .05f
			};
		}
		internal static void SaveSettings()
		{
			Settings settings = LoadSettings();
			settings.FullScreen = FullScreen;
			settings.GeneralVolumeToggle = GeneralVolumeToggle;
			settings.GeneralVolume = GeneralVolume;
			settings.EffectsVolumeToggle = EffectsVolumeToggle;
			settings.EffectsVolume = EffectsVolume;
			settings.MusicVolumeToggle = MusicVolumeToggle;
			settings.MusicVolume = MusicVolume;
			settings.DialogSpeed = DialogSpeed;
			DataController.WriteData(settings, SettingsPath);
		}
	};
};