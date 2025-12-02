using UnityEngine;
namespace GwambaPrimeAdventure
{
	public static class WorldBuild
	{
		public const ushort PIXEL_PERFECT_WIDTH = 320;
		public const ushort PIXEL_PERFECT_HEIGHT = 180;
		public const ushort LEVELS_COUNT = 10;
		public const float FIELD_SPACE_LENGTH = 8F;
		public const float SNAP_LENGTH = 1F / 16F;
		public enum Layers
		{
			System,
			Character,
			Scene,
			Item,
			Enemy,
			Boss
		};
		public static readonly LayerMask SystemMask = GetMask(Layers.System);
		public static readonly LayerMask CharacterMask = GetMask(Layers.Character);
		public static readonly LayerMask SceneMask = GetMask(Layers.Scene);
		public static readonly LayerMask ItemMask = GetMask(Layers.Item);
		public static readonly LayerMask EnemyMask = GetMask(Layers.Enemy);
		public static readonly LayerMask BossMask = GetMask(Layers.Boss);
		private static float _localScaleX;
		private static LayerMask GetMask(Layers layerName)
		{
			for (int i = 0; i < 32; i++)
				if (LayerMask.LayerToName(i) != string.Empty && LayerMask.LayerToName(i).ToUpper() == layerName.ToString().ToUpper())
					return 1 << i;
			return 0;
		}
		public static void TurnScaleX(this Transform transform, float valueChanger)
		{
			_localScaleX = Mathf.Abs(transform.localScale.x) * valueChanger;
			transform.localScale = new Vector3(_localScaleX, transform.localScale.y, transform.localScale.z);
		}
		public static void TurnScaleX(this Transform transform, bool conditionChanger)
		{
			_localScaleX = Mathf.Abs(transform.localScale.x) * (conditionChanger ? -1f : 1f);
			transform.localScale = new Vector3(_localScaleX, transform.localScale.y, transform.localScale.z);
		}
	};
};
