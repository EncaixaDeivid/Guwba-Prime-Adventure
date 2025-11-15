using UnityEngine;
namespace GwambaPrimeAdventure
{
	public static class WorldBuild
	{
		public const float SNAP = 1f / 16f;
		private static float _localScaleX;
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
