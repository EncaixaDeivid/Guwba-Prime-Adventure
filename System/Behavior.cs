using UnityEngine;
using System.IO;
using System.Runtime.CompilerServices;
namespace GuwbaPrimeAdventure
{
	public interface IDestructible
	{
		public short Health { get; }
		public bool Hurt(ushort damage);
		public void Stun(ushort stunStength, float stunTime);
	};
	public interface IInteractable
	{
		public void Interaction();
	};
	public interface ICollectable
	{
		public void Collect();
	};
#if UNITY_EDITOR
	public interface ILogger
	{
		public void LogInfo(string message,
			[CallerMemberName] string member = "",
			[CallerFilePath] string file = "",
			[CallerLineNumber] int line = 0)
		{
			Debug.Log($"[{Path.GetFileName(file)} : {line} - {member}] {message}");
		}
	};
	public sealed class Logger : ILogger
	{
		public static readonly ILogger Informer = new Logger();
		private Logger() { }
	};
#endif
};
