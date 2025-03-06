using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure
{
	public static class SaveFileData
	{
		private static readonly string SaveFilePath = Application.persistentDataPath + "/Files Names.txt";
		private static ushort ActualSaveFile = 0;
		private struct FilesNames
		{
			public string DataFileName1;
			public string DataFileName2;
			public string DataFileName3;
			public string DataFileName4;
		};
		private static string DataFile1 = LoadFilesNames().DataFileName1;
		private static string DataFile2 = LoadFilesNames().DataFileName2;
		private static string DataFile3 = LoadFilesNames().DataFileName3;
		private static string DataFile4 = LoadFilesNames().DataFileName4;
		internal static string DataFileName1 => DataFile1;
		internal static string DataFileName2 => DataFile2;
		internal static string DataFileName3 => DataFile3;
		internal static string DataFileName4 => DataFile4;
		private static FilesNames LoadFilesNames()
		{
			if (File.Exists(SaveFilePath))
				return DataController.ReadData<FilesNames>(SaveFilePath);
			return new FilesNames()
			{
				DataFileName1 = "Data File 1",
				DataFileName2 = "Data File 2",
				DataFileName3 = "Data File 3",
				DataFileName4 = "Data File 4"
			};
		}
		private static string SelectDataFile(ushort actualSaveFile)
		{
			return actualSaveFile switch
			{
				1 => DataFile1,
				2 => DataFile2,
				3 => DataFile3,
				4 => DataFile4,
				_ => null
			};
		}
		private static void SetDataFile(ushort actualSaveFile, string newSaveName)
		{
			_ = actualSaveFile switch
			{
				1 => DataFile1 = newSaveName,
				2 => DataFile2 = newSaveName,
				3 => DataFile3 = newSaveName,
				4 => DataFile4 = newSaveName,
				_ => null
			};
		}
		private struct DataFile
		{
			public ushort Lifes;
			public ushort Coins;
			public List<string> LifesAcquired;
			public Dictionary<string, bool> Books;
			public List<string> BooksName;
			public List<bool> BooksValue;
			public List<string> GeneralObjects;
			public string LastLevelEntered;
			public bool[] LevelsCompleted;
			public bool[] DeafetedBosses;
		};
		public static ushort Lifes = LoadFile().Lifes;
		public static ushort Coins = LoadFile().Coins;
		internal static string InternalLastLevelEntered = LoadFile().LastLevelEntered;
		private static List<string> PrivateLifesAcquired = LoadFile().LifesAcquired;
		private static Dictionary<string, bool> PrivateBooks = LoadFile().Books;
		private static List<string> PrivateGeneralObjects = LoadFile().GeneralObjects;
		private static bool[] PrivateLevelsCompleted = LoadFile().LevelsCompleted;
		private static bool[] PrivateDeafetedBosses = LoadFile().DeafetedBosses;
		public static string LastLevelEntered => InternalLastLevelEntered;
		public static List<string> LifesAcquired => PrivateLifesAcquired;
		public static Dictionary<string, bool> Books => PrivateBooks;
		public static List<string> GeneralObjects => PrivateGeneralObjects;
		public static bool[] LevelsCompleted => PrivateLevelsCompleted;
		public static bool[] DeafetedBosses => PrivateDeafetedBosses;
		internal static void RefreshData()
		{
			Lifes = LoadFile().Lifes;
			Coins = LoadFile().Coins;
			PrivateLifesAcquired = LoadFile().LifesAcquired;
			PrivateBooks = LoadFile().Books;
			PrivateGeneralObjects = LoadFile().GeneralObjects;
			InternalLastLevelEntered = LoadFile().LastLevelEntered;
			PrivateLevelsCompleted = LoadFile().LevelsCompleted;
			PrivateDeafetedBosses = LoadFile().DeafetedBosses;
		}
		internal static bool FileExists() => File.Exists($"{Application.persistentDataPath}/{SelectDataFile(ActualSaveFile)}.txt");
		private static DataFile LoadFile()
		{
			string actualSaveFile = SelectDataFile(ActualSaveFile);
			if (actualSaveFile == null)
				return new DataFile();
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			if (File.Exists(actualPath))
			{
				if (actualSaveFile != DataFile1 && actualSaveFile != DataFile2 && actualSaveFile != DataFile3 && actualSaveFile != DataFile4)
				{
					File.Delete(actualPath);
					return new DataFile();
				}
				DataFile loadedData = DataController.ReadData<DataFile>(actualPath);
				loadedData.Books = new Dictionary<string, bool>();
				for (ushort i = 0; i < loadedData.BooksName.Count; i++)
					loadedData.Books.Add(loadedData.BooksName[i], loadedData.BooksValue[i]);
				return loadedData;
			}
			return new DataFile()
			{
				Lifes = 10,
				LifesAcquired = new List<string>(),
				Coins = 0,
				Books = new Dictionary<string, bool>(),
				BooksName = new List<string>(),
				BooksValue = new List<bool>(),
				GeneralObjects = new List<string>(),
				LastLevelEntered = "",
				LevelsCompleted = new bool[2],
				DeafetedBosses = new bool[1]
			};
		}
		internal static void SetActualSaveFile(ushort actualSaveFile)
		{
			ActualSaveFile = actualSaveFile;
			RefreshData();
		}
		internal static void RenameData(ushort actualSave, string newName)
		{
			string actualSaveFile = SelectDataFile(actualSave);
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			string newSaveName = Application.persistentDataPath + $"/{newName}.txt";
			if (File.Exists(actualPath))
			{
				DataFile loadedData = DataController.ReadData<DataFile>(actualPath);
				File.Delete(actualPath);
				DataController.WriteData(loadedData, newSaveName);
			}
			SetDataFile(actualSave, newName);
			if (File.Exists(SaveFilePath))
			{
				FilesNames newFilesNames = new()
				{
					DataFileName1 = DataFileName1,
					DataFileName2 = DataFileName2,
					DataFileName3 = DataFileName3,
					DataFileName4 = DataFileName4
				};
				DataController.WriteData(newFilesNames, SaveFilePath);
			}
		}
		internal static void DeleteData(ushort actualSave)
		{
			string actualSaveFile = SelectDataFile(actualSave);
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			SetDataFile(actualSave, $"Data File {actualSave}");
			if (File.Exists(actualPath))
				File.Delete(actualPath);
			FilesNames newFilesNames = new()
			{
				DataFileName1 = DataFileName1,
				DataFileName2 = DataFileName2,
				DataFileName3 = DataFileName3,
				DataFileName4 = DataFileName4
			};
			DataController.WriteData(newFilesNames, SaveFilePath);
		}
		internal static void SaveData()
		{
			FilesNames newFilesNames = LoadFilesNames();
			newFilesNames.DataFileName1 = DataFileName1;
			newFilesNames.DataFileName2 = DataFileName2;
			newFilesNames.DataFileName3 = DataFileName3;
			newFilesNames.DataFileName4 = DataFileName4;
			DataController.WriteData(newFilesNames, SaveFilePath);
			string actualSaveFile = SelectDataFile(ActualSaveFile);
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			DataFile newDataFile = LoadFile();
			newDataFile.Lifes = Lifes;
			newDataFile.LifesAcquired = LifesAcquired;
			newDataFile.Coins = Coins;
			if (Books.Count > 0f)
			{
				newDataFile.BooksName = new List<string>();
				newDataFile.BooksValue = new List<bool>();
				newDataFile.BooksName.AddRange(Books.Keys);
				newDataFile.BooksValue.AddRange(Books.Values);
			}
			newDataFile.GeneralObjects = GeneralObjects;
			newDataFile.LastLevelEntered = LastLevelEntered;
			newDataFile.LevelsCompleted = LevelsCompleted;
			newDataFile.DeafetedBosses = DeafetedBosses;
			DataController.WriteData(newDataFile, actualPath);
		}
	};
};