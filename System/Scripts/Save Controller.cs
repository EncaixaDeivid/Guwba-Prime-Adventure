using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure
{
	public static class SaveController
	{
		private struct SaveFile
		{
			public ushort Lifes;
			public List<string> LifesAcquired;
			public ushort Coins;
			public Dictionary<string, bool> Books;
			public List<string> BooksName;
			public List<bool> BooksValue;
			public List<string> GeneralObjects;
			public string LastLevelEntered;
			public bool[] LevelsCompleted;
			public bool[] DeafetedBosses;
		};
		private static ushort ActualSaveFile = 0;
		public static ushort Lifes = LoadFile().Lifes;
		public static ushort Coins = LoadFile().Coins;
		internal static string InternalLastLevelEntered = LoadFile().LastLevelEntered;
		private static List<string> _privateLifesAcquired = LoadFile().LifesAcquired;
		private static Dictionary<string, bool> _privateBooks = LoadFile().Books;
		private static List<string> _privateGeneralObjects = LoadFile().GeneralObjects;
		private static bool[] _privateLevelsCompleted = LoadFile().LevelsCompleted;
		private static bool[] _privateDeafetedBosses = LoadFile().DeafetedBosses;
		public static string LastLevelEntered => InternalLastLevelEntered;
		public static List<string> LifesAcquired => _privateLifesAcquired;
		public static Dictionary<string, bool> Books => _privateBooks;
		public static List<string> GeneralObjects => _privateGeneralObjects;
		public static bool[] LevelsCompleted => _privateLevelsCompleted;
		public static bool[] DeafetedBosses => _privateDeafetedBosses;
		internal static void RefreshData()
		{
			Lifes = LoadFile().Lifes;
			_privateLifesAcquired = LoadFile().LifesAcquired;
			Coins = LoadFile().Coins;
			_privateBooks = LoadFile().Books;
			_privateGeneralObjects = LoadFile().GeneralObjects;
			InternalLastLevelEntered = LoadFile().LastLevelEntered;
			_privateLevelsCompleted = LoadFile().LevelsCompleted;
			_privateDeafetedBosses = LoadFile().DeafetedBosses;
		}
		internal static bool FileExists() => File.Exists($"{Application.persistentDataPath}/{FilesController.Select(ActualSaveFile)}.txt");
		private static SaveFile LoadFile()
		{
			SaveFile saveFile = new()
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
			string actualSaveFile = FilesController.Select(ActualSaveFile);
			if (string.IsNullOrEmpty(actualSaveFile))
				return saveFile;
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			if (File.Exists(actualPath))
			{
				bool isDataEmpty1 = actualSaveFile != FilesController.Select(1) && actualSaveFile != FilesController.Select(2);
				bool isDataEmpty2 = actualSaveFile != FilesController.Select(3) && actualSaveFile != FilesController.Select(4);
				if (isDataEmpty1 && isDataEmpty2)
				{
					File.Delete(actualPath);
					return saveFile;
				}
				SaveFile loadedData = ArchiveEncoder.ReadData<SaveFile>(actualPath);
				loadedData.Books = new Dictionary<string, bool>();
				for (ushort i = 0; i < loadedData.BooksName.Count; i++)
					loadedData.Books.Add(loadedData.BooksName[i], loadedData.BooksValue[i]);
				return loadedData;
			}
			return saveFile;
		}
		internal static void SetActualSaveFile(ushort actualSaveFile)
		{
			ActualSaveFile = actualSaveFile;
			RefreshData();
		}
		internal static void RenameData(ushort actualSave, string newName)
		{
			if (string.IsNullOrEmpty(newName))
				return;
			FilesController.SaveData((actualSave, newName));
			string actualSaveFile = FilesController.Select(actualSave);
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			string newSaveName = Application.persistentDataPath + $"/{newName}.txt";
			if (File.Exists(actualPath))
			{
				SaveFile loadedData = ArchiveEncoder.ReadData<SaveFile>(actualPath);
				File.Delete(actualPath);
				ArchiveEncoder.WriteData(loadedData, newSaveName);
			}
		}
		internal static string DeleteData(ushort actualSave)
		{
			string actualSaveFile = FilesController.Select(actualSave);
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			if (File.Exists(actualPath))
				File.Delete(actualPath);
			return FilesController.SaveData((actualSave, $"Data File {actualSave}"));
		}
		internal static void SaveData()
		{
			FilesController.SaveData();
			string actualSaveFile = FilesController.Select(ActualSaveFile);
			string actualPath = Application.persistentDataPath + $"/{actualSaveFile}.txt";
			SaveFile newSaveFile = new()
			{
				Lifes = Lifes,
				LifesAcquired = LifesAcquired,
				Coins = Coins,
				BooksName = new List<string>(Books?.Count > 0f ? Books?.Keys : null),
				BooksValue = new List<bool>(Books?.Count > 0f ? Books?.Values : null),
				GeneralObjects = GeneralObjects,
				LastLevelEntered = LastLevelEntered,
				LevelsCompleted = LevelsCompleted,
				DeafetedBosses = DeafetedBosses
			};
			ArchiveEncoder.WriteData(newSaveFile, actualPath);
		}
	};
};
