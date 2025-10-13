using UnityEngine;
using System.IO;
using System.Text;
namespace GuwbaPrimeAdventure.Data
{
	internal static class ArchiveEncoder
	{
		private const string ScriptPassword = "BoLo%De%CeNoUrA%cOm%CoBeRtUrA%dE%cHoCoLaTe%AmArGo%!";
		private static string ScriptData(string data)
		{
			string scriptedData = "";
			for (ushort i = 0; i < data.Length; i++)
				scriptedData += (char)(data[i] ^ ScriptPassword[i % ScriptPassword.Length]);
			return scriptedData;
		}
		internal static StructData ReadData<StructData>(string path) where StructData : struct
		{
			string scriptedData = File.ReadAllText(path, Encoding.UTF8);
			string dataJSON = ScriptData(scriptedData);
			return JsonUtility.FromJson<StructData>(dataJSON);
		}
		internal static void WriteData<StructData>(StructData structData, string path) where StructData : struct
		{
			string dataJSON = JsonUtility.ToJson(structData);
			string scriptedData = ScriptData(dataJSON);
			if (File.Exists(path))
				File.Delete(path);
			File.WriteAllText(path, scriptedData, Encoding.UTF8);
		}
	};
};
