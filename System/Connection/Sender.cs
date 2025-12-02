using UnityEngine;
using System.Collections.Generic;
namespace GwambaPrimeAdventure.Connection
{
	public interface IConnector
	{
		public MessagePath Path { get; }
		public void Receive(MessageData data);
	};
	public sealed class Sender
	{
		private Sender()
		{
			_messageData = new MessageData()
			{
				Format = MessageFormat.None,
				AdditionalData = null,
				ToggleValue = null,
				NumberValue = null
			};
		}
		private static readonly Dictionary<MessagePath, List<IConnector>> _connectors = new();
		private MessageData _messageData;
		public static void Include(IConnector connector)
		{
			if (!_connectors.ContainsKey(connector.Path))
				_connectors.Add(connector.Path, new List<IConnector>() { connector });
			else if (!_connectors[connector.Path].Contains(connector))
				_connectors[connector.Path].Add(connector);
		}
		public static void Exclude(IConnector connector)
		{
			if (_connectors.ContainsKey(connector.Path))
			{
				if (_connectors[connector.Path].Contains(connector))
					_connectors[connector.Path].Remove(connector);
				if (_connectors[connector.Path].Count <= 0)
					_connectors.Remove(connector.Path);
			}
		}
		public static Sender Create() => new();
		public void SetFormat(MessageFormat format) => _messageData.Format = format;
		public void SetAdditionalData(object additionalData) => _messageData.AdditionalData = additionalData;
		public void SetToggle(bool value) => _messageData.ToggleValue = value;
		public void SetNumber(ushort value) => _messageData.NumberValue = (ushort)(Mathf.Abs(value));
		public void Send(MessagePath path)
		{
			if (_connectors.ContainsKey(path))
				foreach (IConnector connector in _connectors[path].ToArray())
					if (connector != null && connector.Path == path)
						connector.Receive(_messageData);
		}
	};
};
