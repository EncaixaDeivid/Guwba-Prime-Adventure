using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Data
{
	public struct SaveFile
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
				Lifes = 10,
				LifesAcquired = new List<string>(),
				Coins = 0,
				Books = new Dictionary<string, bool>(),
				BooksName = new List<string>(),
				BooksValue = new List<bool>(),
				GeneralObjects = new List<string>(),
				LastLevelEntered = "",
				LevelsCompleted = new bool[12],
				DeafetedBosses = new bool[12]
			};
			string actualSaveFile = FilesController.Select(_actualSaveFile);
			if (string.IsNullOrEmpty(actualSaveFile))
				return saveFile;
			string actualPath = $@"{Application.persistentDataPath}\{actualSaveFile}.txt";
			if (File.Exists(actualPath))
			{
				bool isDataEmpty = actualSaveFile != FilesController.Select(3) && actualSaveFile != FilesController.Select(4);
				if (actualSaveFile != FilesController.Select(1) && actualSaveFile != FilesController.Select(2) && isDataEmpty)
				{
					File.Delete(actualPath);
					return saveFile;
				}
				saveFile = FileEncoder.ReadData<SaveFile>(actualPath);
				saveFile.Books = new Dictionary<string, bool>();
				for (ushort i = 0; i < saveFile.BooksName.Count; i++)
					saveFile.Books.Add(saveFile.BooksName[i], saveFile.BooksValue[i]);
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
				SaveFile loadedData = FileEncoder.ReadData<SaveFile>(actualPath);
				File.Delete(actualPath);
				FileEncoder.WriteData(loadedData, newSaveName);
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
			if (string.IsNullOrEmpty(actualSaveFile))
				return;
			string actualPath = $@"{Application.persistentDataPath}\{actualSaveFile}.txt";
			SaveFile newSaveFile = new()
			{
				Lifes = _saveFile.Lifes,
				LifesAcquired = _saveFile.LifesAcquired,
				Coins = _saveFile.Coins,
				BooksName = new List<string>(_saveFile.Books?.Count > 0f ? _saveFile.Books.Keys : new List<string>()),
				BooksValue = new List<bool>(_saveFile.Books?.Count > 0f ? _saveFile.Books.Values : new List<bool>()),
				GeneralObjects = _saveFile.GeneralObjects,
				LastLevelEntered = _saveFile.LastLevelEntered,
				LevelsCompleted = _saveFile.LevelsCompleted,
				DeafetedBosses = _saveFile.DeafetedBosses
			};
			FileEncoder.WriteData(newSaveFile, actualPath);
		}
	};
};
