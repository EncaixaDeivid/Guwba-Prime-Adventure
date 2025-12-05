using UnityEngine;
using System.Collections.Generic;
namespace GwambaPrimeAdventure
{
	public static class WorldBuild
	{
		public const float FIELD_SPACE_LENGTH = 8F;
		public const float SNAP_LENGTH = 1F / 16F;
		public const float MINIMUM_TIME_SPACE_LIMIT = 1E-4F;
		public const ushort PIXEL_PERFECT_WIDTH = 320;
		public const ushort PIXEL_PERFECT_HEIGHT = 180;
		public const ushort LEVELS_COUNT = 10;
		public static readonly LayerMask SystemMask = 1 << 3;
		public static readonly LayerMask UIMask = 1 << 5;
		public static readonly LayerMask CharacterMask = 1 << 6;
		public static readonly LayerMask SceneMask = 1 << 7;
		public static readonly LayerMask ItemMask = 1 << 8;
		public static readonly LayerMask EnemyMask = 1 << 9;
		public static readonly LayerMask BossMask = 1 << 10;
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
			float scaleX = Mathf.Abs(transform.localScale.x) * valueChanger;
			transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
		}
		public static void TurnScaleX(this Transform transform, bool conditionChanger) => TurnScaleX(transform, conditionChanger ? -1F : 1F);
	};
};
