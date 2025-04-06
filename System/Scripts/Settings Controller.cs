using UnityEngine;
using System.IO;
namespace GuwbaPrimeAdventure
{
	public static class SettingsController
	{
		private struct Settings
		{
			public bool FullScreen;
			public bool GeneralVolumeToggle;
			[Range(0f, 1f)] public float GeneralVolume;
			public bool EffectsVolumeToggle;
			[Range(0f, 1f)] public float EffectsVolume;
			public bool MusicVolumeToggle;
			[Range(0f, 1f)] public float MusicVolume;
			public bool DialogToggle;
			[Range(0f, .1f)] public float DialogSpeed;
		}
		private static readonly string SettingsPath = Application.persistentDataPath + "/Settings.txt";
		private static Settings LoadSettings()
		{
			if (File.Exists(SettingsPath))
				return ArchiveEncoder.ReadData<Settings>(SettingsPath);
			return new Settings()
			{
				FullScreen = true,
				GeneralVolumeToggle = true,
				GeneralVolume = 1f,
				EffectsVolumeToggle = true,
				EffectsVolume = 1f,
				MusicVolumeToggle = true,
				MusicVolume = 1f,
				DialogToggle = true,
				DialogSpeed = .05f
			};
		}
		internal static void SaveSettings()
		{
			Settings settings = new()
			{
				FullScreen = FullScreen,
				GeneralVolumeToggle = GeneralVolumeToggle,
				GeneralVolume = GeneralVolume,
				EffectsVolumeToggle = EffectsVolumeToggle,
				EffectsVolume = EffectsVolume,
				MusicVolumeToggle = MusicVolumeToggle,
				MusicVolume = MusicVolume,
				DialogToggle = DialogToggle,
				DialogSpeed = DialogSpeed
			};
			ArchiveEncoder.WriteData(settings, SettingsPath);
		}
		public static bool FullScreen = LoadSettings().FullScreen;
		public static bool GeneralVolumeToggle = LoadSettings().GeneralVolumeToggle;
		public static float GeneralVolume = LoadSettings().GeneralVolume;
		public static bool EffectsVolumeToggle = LoadSettings().EffectsVolumeToggle;
		public static float EffectsVolume = LoadSettings().EffectsVolume;
		public static bool MusicVolumeToggle = LoadSettings().MusicVolumeToggle;
		public static float MusicVolume = LoadSettings().MusicVolume;
		public static bool DialogToggle = LoadSettings().DialogToggle;
		public static float DialogSpeed = LoadSettings().DialogSpeed;
	};
};
