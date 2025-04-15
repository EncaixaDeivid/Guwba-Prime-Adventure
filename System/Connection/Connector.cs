using System;
namespace GuwbaPrimeAdventure.Connection
{
	public enum PathConnection
	{
		None,
		Enemy,
		Boss,
		Item,
		EventItem,
		Hud,
		Controller,
		Dialog
	};
	public enum ConnectionState
	{
		None,
		Enable,
		Disable,
		Action
	};
	[Flags]
	public enum BossType
	{
		None,
		All,
		Runner,
		Jumper,
		Summoner
	};
	public interface IConnector
	{
		public PathConnection PathConnection { get; }
		public void Receive(DataConnection data);
	};
};
