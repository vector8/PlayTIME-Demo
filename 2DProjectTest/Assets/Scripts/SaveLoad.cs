using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad 
{
	public static List<SaveFile> savedGames = new List<SaveFile>();

	public static void save(string name)
	{
		bool found = false;
		for(int i = 0; i < savedGames.Count; i++)
		{
			if(savedGames[i].name == name)
			{
				savedGames[i].levelManager = LevelManager.instance;
				savedGames[i].name = name;
				savedGames[i].dateTime = DateTime.Now;
				found = true;
				break;
			}
		}
		if(!found)
		{
			SaveFile save = null;
			save = new SaveFile();
			save.levelManager = LevelManager.instance;
			save.name = name;
			save.dateTime = DateTime.Now;
			savedGames.Add (save);
		}
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
		bf.Serialize(file, SaveLoad.savedGames);
		file.Close();
	}

	public static void load()
	{
		if(File.Exists(Application.persistentDataPath + "/savedGames.gd"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			SaveLoad.savedGames = (List<SaveFile>) bf.Deserialize(file);
			file.Close();
		}
	}

	/**
	 * returns true if successful, false otherwise
	 * */
	public static bool loadGame(string name)
	{
		load();

		for(int i = 0; i < savedGames.Count; i++)
		{
			if(savedGames[i].name == name)
			{
				LevelManager.instance = savedGames[i].levelManager;
				return true;
			}
		}

		return false;
	}
}

[System.Serializable]
public class SaveFile
{
	public LevelManager levelManager;
	public string name;
	public DateTime dateTime;
}
