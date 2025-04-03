using UnityEngine;
using System.IO;
namespace GuwbaPrimeAdventure
{
	internal struct FilesNames
	{
		private string _dataFile1;
		private string _dataFile2;
		private string _dataFile3;
		private string _dataFile4;
		private static readonly string SaveFilePath = Application.persistentDataPath + "/Files Names.txt";
		private static FilesNames LoadFilesNames()
		{
			if (File.Exists(SaveFilePath))
				return DataController.ReadData<FilesNames>(SaveFilePath);
			return new FilesNames()
			{
				_dataFile1 = "Data File 1",
				_dataFile2 = "Data File 2",
				_dataFile3 = "Data File 3",
				_dataFile4 = "Data File 4"
			};
		}
		internal static string SelectDataFile(ushort actualSaveFile)
		{
			return actualSaveFile switch
			{
				1 => LoadFilesNames()._dataFile1,
				2 => LoadFilesNames()._dataFile2,
				3 => LoadFilesNames()._dataFile3,
				4 => LoadFilesNames()._dataFile4,
				_ => null
			};
		}
		internal static string SaveData(ushort actualDataFile, string newName)
		{
			FilesNames newFilesNames = LoadFilesNames();
			string newFileName = actualDataFile switch
			{
				1 => newFilesNames._dataFile1 = newName,
				2 => newFilesNames._dataFile2 = newName,
				3 => newFilesNames._dataFile3 = newName,
				4 => newFilesNames._dataFile4 = newName,
				_ => null
			};
			DataController.WriteData(newFilesNames, SaveFilePath);
			return newFileName;
		}
	};
};
