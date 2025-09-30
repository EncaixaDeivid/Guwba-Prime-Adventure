using UnityEngine;
using System.IO;
namespace GuwbaPrimeAdventure.Data
{
	public struct Settings
	{
		public Vector2Int screenResolution;
		public FullScreenMode fullScreenMode;
		public bool dialogToggle;
		public float dialogSpeed;
		public float screenBrightness;
		public bool generalVolumeToggle;
		public ushort generalVolume;
		public bool effectsVolumeToggle;
		public ushort effectsVolume;
		public bool musicVolumeToggle;
		public ushort musicVolume;
	};
	public static class SettingsController
	{
		private static Settings _settings = LoadFile();
		private static readonly string SettingsPath = $@"{Application.persistentDataPath}\Settings.txt";
		private static Settings LoadFile()
		{
			Load(out Settings settings);
			return settings;
		}
		public static bool FileExists() => File.Exists(SettingsPath);
		public static void Load(out Settings settings)
		{
			settings = new Settings()
			{
				screenResolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height),
				fullScreenMode = FullScreenMode.MaximizedWindow,
				dialogToggle = true,
				dialogSpeed = .05f,
				screenBrightness = 1f,
				generalVolumeToggle = true,
				generalVolume = 100,
				effectsVolumeToggle = true,
				effectsVolume = 100,
				musicVolumeToggle = true,
				musicVolume = 100
			};
			if (File.Exists(SettingsPath))
				settings = ArchiveEncoder.ReadData<Settings>(SettingsPath);
		}
		public static void WriteSave(Settings settings) => _settings = settings;
		public static void SaveSettings() =>
			ArchiveEncoder.WriteData(new Settings()
			{
				screenResolution = _settings.screenResolution,
				fullScreenMode = _settings.fullScreenMode,
				dialogToggle = _settings.dialogToggle,
				dialogSpeed = _settings.dialogSpeed,
				screenBrightness = _settings.screenBrightness,
				generalVolumeToggle = _settings.generalVolumeToggle,
				generalVolume = _settings.generalVolume,
				effectsVolumeToggle = _settings.effectsVolumeToggle,
				effectsVolume = _settings.effectsVolume,
				musicVolumeToggle = _settings.musicVolumeToggle,
				musicVolume = _settings.musicVolume
			}, SettingsPath);
	};
};
