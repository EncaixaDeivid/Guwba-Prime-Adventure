using UnityEngine;
using System.IO;
namespace GuwbaPrimeAdventure.Data
{
	public struct Settings
	{
		public bool fullScreen;
		public bool generalVolumeToggle;
		public ushort generalVolume;
		public bool effectsVolumeToggle;
		public ushort effectsVolume;
		public bool musicVolumeToggle;
		public ushort musicVolume;
		public bool dialogToggle;
		public float dialogSpeed;
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
				fullScreen = true,
				generalVolumeToggle = true,
				generalVolume = 100,
				effectsVolumeToggle = true,
				effectsVolume = 100,
				musicVolumeToggle = true,
				musicVolume = 100,
				dialogToggle = true,
				dialogSpeed = .05f
			};
			if (File.Exists(SettingsPath))
				settings = ArchiveEncoder.ReadData<Settings>(SettingsPath);
		}
		public static void WriteSave(Settings settings) => _settings = settings;
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
