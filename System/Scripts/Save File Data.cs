using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure
{
	public struct DataFile
	{
		private ushort _lifes;
		private List<string> _lifesAcquired;
		private ushort _coins;
		private Dictionary<string, bool> _books;
		private List<string> _booksName;
		private List<bool> _booksValue;
		private List<string> _generalObjects;
		private string _lastLevelEntered;
		private bool[] _levelsCompleted;
		private bool[] _deafetedBosses;
		private static ushort ActualSaveFile = 0;
		public static ushort Lifes = LoadFile()._lifes;
		public static ushort Coins = LoadFile()._coins;
		internal static string InternalLastLevelEntered = LoadFile()._lastLevelEntered;
		private static List<string> _privateLifesAcquired = LoadFile()._lifesAcquired;
		private static Dictionary<string, bool> _privateBooks = LoadFile()._books;
		private static List<string> _privateGeneralObjects = LoadFile()._generalObjects;
		private static bool[] _privateLevelsCompleted = LoadFile()._levelsCompleted;
		private static bool[] _privateDeafetedBosses = LoadFile()._deafetedBosses;
		public static string LastLevelEntered => InternalLastLevelEntered;
		public static List<string> LifesAcquired => _privateLifesAcquired;
		public static Dictionary<string, bool> Books => _privateBooks;
		public static List<string> GeneralObjects => _privateGeneralObjects;
		public static bool[] LevelsCompleted => _privateLevelsCompleted;
		public static bool[] DeafetedBosses => _privateDeafetedBosses;
		internal static void RefreshData()
		{
			Lifes = LoadFile()._lifes;
			_privateLifesAcquired = LoadFile()._lifesAcquired;
			Coins = LoadFile()._coins;
			_privateBooks = LoadFile()._books;
			_privateGeneralObjects = LoadFile()._generalObjects;
			InternalLastLevelEntered = LoadFile()._lastLevelEntered;
			_privateLevelsCompleted = LoadFile()._levelsCompleted;
			_privateDeafetedBosses = LoadFile()._deafetedBosses;
		}
		internal static bool FileExists() => File.Exists($"{Application.persistentDataPath}/{FilesNames.SelectDataFile(ActualSaveFile)}.txt");
		private static DataFile LoadFile()
		{
			string actualSaveFile = FilesNames.SelectDataFile(ActualSaveFile);
			if (actualSaveFile == null)
				return new DataFile();
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			if (File.Exists(actualPath))
			{
				bool[] filesExists = new bool[4];
				for (ushort i = 0; i < 4; i++)
					if (FilesNames.SelectDataFile(i) != actualSaveFile)
						filesExists[i] = true;
				if (filesExists[0] == true && filesExists[1] == true && filesExists[2] == true && filesExists[3] == true)
				{
					File.Delete(actualPath);
					return new DataFile();
				}
				DataFile loadedData = DataController.ReadData<DataFile>(actualPath);
				loadedData._books = new Dictionary<string, bool>();
				for (ushort i = 0; i < loadedData._booksName.Count; i++)
					loadedData._books.Add(loadedData._booksName[i], loadedData._booksValue[i]);
				return loadedData;
			}
			return new DataFile()
			{
				_lifes = 10,
				_lifesAcquired = new List<string>(),
				_coins = 0,
				_books = new Dictionary<string, bool>(),
				_booksName = new List<string>(),
				_booksValue = new List<bool>(),
				_generalObjects = new List<string>(),
				_lastLevelEntered = "",
				_levelsCompleted = new bool[2],
				_deafetedBosses = new bool[1]
			};
		}
		internal static void SetActualSaveFile(ushort actualSaveFile)
		{
			ActualSaveFile = actualSaveFile;
			RefreshData();
		}
		internal static void RenameData(ushort actualSave, string newName)
		{
			string actualSaveFile = FilesNames.SelectDataFile(actualSave);
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			string newSaveName = Application.persistentDataPath + $"/{newName}.txt";
			if (File.Exists(actualPath))
			{
				DataFile loadedData = DataController.ReadData<DataFile>(actualPath);
				File.Delete(actualPath);
				DataController.WriteData(loadedData, newSaveName);
			}
			FilesNames.SaveData(actualSave, newName);
		}
		internal static string DeleteData(ushort actualSave)
		{
			string actualSaveFile = FilesNames.SelectDataFile(actualSave);
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			if (File.Exists(actualPath))
				File.Delete(actualPath);
			FilesNames.SaveData(actualSave, $"Data File {actualSave}");
			return FilesNames.SelectDataFile(actualSave);
		}
		internal static void SaveData()
		{
			string actualSaveFile = FilesNames.SelectDataFile(ActualSaveFile);
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			DataFile newDataFile = LoadFile();
			newDataFile._lifes = Lifes;
			newDataFile._lifesAcquired = LifesAcquired;
			newDataFile._coins = Coins;
			if (Books.Count > 0f)
			{
				newDataFile._booksName = new List<string>();
				newDataFile._booksValue = new List<bool>();
				newDataFile._booksName.AddRange(Books.Keys);
				newDataFile._booksValue.AddRange(Books.Values);
			}
			newDataFile._generalObjects = GeneralObjects;
			newDataFile._lastLevelEntered = LastLevelEntered;
			newDataFile._levelsCompleted = LevelsCompleted;
			newDataFile._deafetedBosses = DeafetedBosses;
			DataController.WriteData(newDataFile, actualPath);
		}
	};
};
