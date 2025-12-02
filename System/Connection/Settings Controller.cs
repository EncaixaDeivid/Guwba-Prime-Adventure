using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace GwambaPrimeAdventure.Connection
{
	public struct Settings
	{
		public Vector2Int ScreenResolution;
		public FullScreenMode FullScreenMode;
		public bool DialogToggle;
		public bool GeneralVolumeToggle;
		public bool EffectsVolumeToggle;
		public bool MusicVolumeToggle;
		public bool InfinityFPS;
		public float DialogSpeed;
		public float ScreenBrightness;
		public ushort FrameRate;
		public ushort GeneralVolume;
		public ushort EffectsVolume;
		public ushort MusicVolume;
	};
	public static class SettingsController
	{
		private static readonly string SettingsPath = $@"{Application.persistentDataPath}\Settings.txt";
		public static Resolution[] PixelPerfectResolutions()
		{
			List<Resolution> resolutions = new();
			foreach (Resolution resolution in Screen.resolutions)
				if (resolution.width % WorldBuild.PIXEL_PERFECT_WIDTH == 0f && resolution.height % WorldBuild.PIXEL_PERFECT_HEIGHT == 0f)
					resolutions.Add(resolution);
			return resolutions.ToArray();
		}
		public static bool FileExists() => File.Exists(SettingsPath);
		public static void Load(out Settings settings)
		{
			if (File.Exists(SettingsPath))
				settings = FileEncoder.ReadData<Settings>(SettingsPath);
			else
				settings = new Settings()
				{
					ScreenResolution = new Vector2Int(PixelPerfectResolutions()[^1].width, PixelPerfectResolutions()[^1].height),
					FullScreenMode = FullScreenMode.FullScreenWindow,
					DialogToggle = true,
					GeneralVolumeToggle = true,
					EffectsVolumeToggle = true,
					MusicVolumeToggle = true,
					InfinityFPS = false,
					DialogSpeed = .05f,
					ScreenBrightness = 1f,
					FrameRate = 60,
					GeneralVolume = 100,
					EffectsVolume = 100,
					MusicVolume = 100
				};
		}
		public static void WriteSave(Settings settings) => FileEncoder.WriteData(settings, SettingsPath);
	};
};