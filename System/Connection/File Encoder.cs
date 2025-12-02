using UnityEngine;
using System.IO;
using System.Text;
namespace GwambaPrimeAdventure.Connection
{
	internal static class FileEncoder
	{
		private const string ScriptPassword = "BoLo%De%CeNoUrA%cOm%CoBeRtUrA%dE%cHoCoLaTe%AmArGo%!";
		private static string ScriptData(string data)
		{
			string scriptedData = "";
			for (ushort i = 0; i < data.Length; i++)
				scriptedData += (char)(data[i] ^ ScriptPassword[i % ScriptPassword.Length]);
			return scriptedData;
		}
		internal static Data ReadData<Data>(string path) where Data : struct => JsonUtility.FromJson<Data>(ScriptData(File.ReadAllText(path, Encoding.UTF8)));
		internal static void WriteData<Data>(Data structData, string path) where Data : struct
		{
			if (File.Exists(path))
				File.Delete(path);
			File.WriteAllText(path, ScriptData(JsonUtility.ToJson(structData)), Encoding.UTF8);
		}
	};
};