using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	internal enum BossType
	{
		None,
		All,
		Runner,
		Jumper,
		Summoner
	};
	internal static class BossSenderExtension
	{
		public static Sender SetBossType(this Sender sender, BossType bossType)
		{
			sender.additionalData = bossType;
			return sender;
		}
	};
};
