using UnityEngine;
using System.IO;
using System.Text;
namespace GwambaPrimeAdventure.Data
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
		internal static StructData ReadData<StructData>(string path) where StructData : struct => JsonUtility.FromJson<StructData>(ScriptData(File.ReadAllText(path, Encoding.UTF8)));
		internal static void WriteData<StructData>(StructData structData, string path) where StructData : struct
		{
			if (File.Exists(path))
				File.Delete(path);
			File.WriteAllText(path, ScriptData(JsonUtility.ToJson(structData)), Encoding.UTF8);
		}
	};
};
