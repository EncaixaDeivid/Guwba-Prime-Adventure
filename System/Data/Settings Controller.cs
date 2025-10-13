using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Data
{
	public struct Settings
	{
		public Vector2Int ScreenResolution;
		public FullScreenMode FullScreenMode;
		public bool DialogToggle;
		public bool GeneralVolumeToggle;
		public bool EffectsVolumeToggle;
		public bool MusicVolumeToggle;
		public float DialogSpeed;
		public float ScreenBrightness;
		public ushort FrameRate;
		public ushort GeneralVolume;
		public ushort EffectsVolume;
		public ushort MusicVolume;
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
		public static Resolution[] PixelPerfectResolutions()
		{
			List<Resolution> resolutions = new();
			foreach (Resolution resolution in Screen.resolutions)
				if (resolution.width % 320 == 0f && resolution.height % 180 == 0f)
					resolutions.Add(resolution);
			return resolutions.ToArray();
		}
		public static bool FileExists() => File.Exists(SettingsPath);
		public static void Load(out Settings settings)
		{
			settings = new Settings()
			{
				ScreenResolution = new Vector2Int(PixelPerfectResolutions()[^1].width, PixelPerfectResolutions()[^1].height),
				FullScreenMode = Screen.fullScreenMode,
				DialogToggle = true,
				GeneralVolumeToggle = true,
				EffectsVolumeToggle = true,
				MusicVolumeToggle = true,
				DialogSpeed = .05f,
				ScreenBrightness = 1f,
				FrameRate = 60,
				GeneralVolume = 100,
				EffectsVolume = 100,
				MusicVolume = 100
			};
			if (File.Exists(SettingsPath))
				settings = ArchiveEncoder.ReadData<Settings>(SettingsPath);
		}
		public static void WriteSave(Settings settings) => _settings = settings;
		public static void SaveSettings() => ArchiveEncoder.WriteData(_settings, SettingsPath);
	};
};
