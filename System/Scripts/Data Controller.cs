using UnityEngine;
using System.IO;
using System.Text;
namespace GuwbaPrimeAdventure
{
	internal static class DataController
	{
		private const string ScriptPassword = "BeToNeIrAdEqUaTrOÈUmAgRaNdEiDeIa!";
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
			File.WriteAllText(path, scriptedData, Encoding.UTF8);
		}
	};
};