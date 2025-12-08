using UnityEngine;
using System.Collections.Generic;
namespace GwambaPrimeAdventure
{
	public static class WorldBuild
	{
		public const float FIELD_SPACE_LENGTH = 8F;
		public const float PIXELS_PER_UNIT = 16F;
		public const float SNAP_LENGTH = 1F / PIXELS_PER_UNIT;
		public const float SCALE_SNAP = 0.0625F;
		public const float ROTATE_SNAP = 7.5F;
		public const float MINIMUM_TIME_SPACE_LIMIT = 1E-4F;
		public const float WIDTH_HEIGHt_PROPORTION = 0.5625F;
		public const int SYSTEM_LAYER = 1 << 3;
		public const int UI_LAYER = 1 << 5;
		public const int CHARACTER_LAYER = 1 << 6;
		public const int SCENE_LAYER = 1 << 7;
		public const int ITEM_LAYER = 1 << 8;
		public const int ENEMY_LAYER = 1 << 9;
		public const int BOSS_LAYER = 1 << 10;
		public const ushort PIXEL_PERFECT_WIDTH = 320;
		public const ushort PIXEL_PERFECT_HEIGHT = (ushort)(PIXEL_PERFECT_WIDTH * WIDTH_HEIGHt_PROPORTION);
		public const ushort UI_SCALE_WIDTH = 1920;
		public const ushort UI_SCALE_HEIGHt = (ushort)(UI_SCALE_WIDTH * WIDTH_HEIGHt_PROPORTION);
		public const ushort LEVELS_COUNT = 10;
		public static Resolution[] PixelPerfectResolutions()
		{
			List<Resolution> resolutions = new();
			foreach (Resolution resolution in Screen.resolutions)
				if (resolution.width % PIXEL_PERFECT_WIDTH == 0 && resolution.height % PIXEL_PERFECT_HEIGHT == 0)
					resolutions.Add(resolution);
			return resolutions.ToArray();
		}
		public static void TurnScaleX(this Transform transform, float valueChanger)
		{
			transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * valueChanger, transform.localScale.y, transform.localScale.z);
		}
		public static void TurnScaleX(this Transform transform, bool conditionChanger) => TurnScaleX(transform, conditionChanger ? -1F : 1F);
	};
};
