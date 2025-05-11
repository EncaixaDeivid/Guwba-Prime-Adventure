using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Data
{
	public struct SaveFile
	{
		public ushort lifes;
		public List<string> lifesAcquired;
		public ushort coins;
		public Dictionary<string, bool> books;
		public List<string> booksName;
		public List<bool> booksValue;
		public List<string> generalObjects;
		public string lastLevelEntered;
		public bool[] levelsCompleted;
		public bool[] deafetedBosses;
	};
	public static class SaveController
	{
		private static SaveFile _saveFile = LoadFile();
		private static ushort _actualSaveFile = 0;
		public static bool FileExists() => File.Exists($@"{Application.persistentDataPath}\{FilesController.Select(_actualSaveFile)}.txt");
		public static void Load(out SaveFile saveFile) => saveFile = _saveFile;
		private static SaveFile LoadFile()
		{
			SaveFile saveFile = new()
			{
				lifes = 10,
				lifesAcquired = new List<string>(),
				coins = 0,
				books = new Dictionary<string, bool>(),
				booksName = new List<string>(),
				booksValue = new List<bool>(),
				generalObjects = new List<string>(),
				lastLevelEntered = "",
				levelsCompleted = new bool[4],
				deafetedBosses = new bool[4]
			};
			string actualSaveFile = FilesController.Select(_actualSaveFile);
			if (string.IsNullOrEmpty(actualSaveFile))
				return saveFile;
			string actualPath = $@"{Application.persistentDataPath}\{actualSaveFile}.txt";
			if (File.Exists(actualPath))
			{
				bool isDataEmpty1 = actualSaveFile != FilesController.Select(1) && actualSaveFile != FilesController.Select(2);
				bool isDataEmpty2 = actualSaveFile != FilesController.Select(3) && actualSaveFile != FilesController.Select(4);
				if (isDataEmpty1 && isDataEmpty2)
				{
					File.Delete(actualPath);
					return saveFile;
				}
				saveFile = ArchiveEncoder.ReadData<SaveFile>(actualPath);
				saveFile.books = new Dictionary<string, bool>();
				for (ushort i = 0; i < saveFile.booksName.Count; i++)
					saveFile.books.Add(saveFile.booksName[i], saveFile.booksValue[i]);
			}
			return saveFile;
		}
		public static void WriteSave(SaveFile saveFile) => _saveFile = saveFile;
		public static void RefreshData() => _saveFile = LoadFile();
		public static void SetActualSaveFile(ushort actualSaveFile)
		{
			_actualSaveFile = actualSaveFile;
			RefreshData();
		}
		public static void RenameData(ushort actualSave, string newName)
		{
			if (string.IsNullOrEmpty(newName))
				return;
			FilesController.SaveData((actualSave, newName));
			string actualSaveFile = FilesController.Select(actualSave);
			string actualPath = $@"{Application.persistentDataPath}\{actualSaveFile}.txt";
			string newSaveName = $@"{Application.persistentDataPath}\{newName}.txt";
			if (File.Exists(actualPath))
			{
				SaveFile loadedData = ArchiveEncoder.ReadData<SaveFile>(actualPath);
				File.Delete(actualPath);
				ArchiveEncoder.WriteData(loadedData, newSaveName);
			}
		}
		public static string DeleteData(ushort actualSave)
		{
			string actualSaveFile = FilesController.Select(actualSave);
			string actualPath = $@"{Application.persistentDataPath}\{actualSaveFile}.txt";
			if (File.Exists(actualPath))
				File.Delete(actualPath);
			return FilesController.SaveData((actualSave, $"Data File {actualSave}"));
		}
		public static void SaveData()
		{
			FilesController.SaveData();
			string actualSaveFile = FilesController.Select(_actualSaveFile);
			string actualPath = $@"{Application.persistentDataPath}\{actualSaveFile}.txt";
			SaveFile newSaveFile = new()
			{
				lifes = _saveFile.lifes,
				lifesAcquired = _saveFile.lifesAcquired,
				coins = _saveFile.coins,
				booksName = new List<string>(_saveFile.books?.Count > 0f ? _saveFile.books.Keys : new List<string>()),
				booksValue = new List<bool>(_saveFile.books?.Count > 0f ? _saveFile.books.Values : new List<bool>()),
				generalObjects = _saveFile.generalObjects,
				lastLevelEntered = _saveFile.lastLevelEntered,
				levelsCompleted = _saveFile.levelsCompleted,
				deafetedBosses = _saveFile.deafetedBosses
			};
			ArchiveEncoder.WriteData(newSaveFile, actualPath);
		}
	};
};
