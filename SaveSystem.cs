using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.Android;
using Newtonsoft.Json;

// Static class for handling saving and loading game data
public static class SaveSystem
{
    // Save the player data to a file
    public static void Save(PlayerData data)
    {
        try
        {
            // Convert PlayerData to JSON format
            string jsonData = JsonUtility.ToJson(data, true);

            // Write JSON data to a file
            using (FileStream stream = new FileStream(GetPath(), FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(jsonData);
                }
            }

            Debug.Log("Saved Game");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError("Error occurred when trying to save the data to a file: " + GetPath());
        }
    }

    // Load player data from a file
    public static PlayerData Load()
    {
        PlayerData data = null;

        try
        {
            // Check if the save file exists
            if (File.Exists(GetPath()))
            {
                // Read JSON data from the file
                string jsonData = "";
                using (FileStream stream = new FileStream(GetPath(), FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonData = reader.ReadToEnd();
                    }
                }

                // Convert JSON data to PlayerData object
                data = JsonUtility.FromJson<PlayerData>(jsonData);
                Debug.Log(GetPath());
                Debug.Log("Save Loaded");
            }
            else
            {
                Debug.LogWarning("Save file does not exist: " + GetPath());
                return NewSave();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError("Error occurred when trying to load the data from file: " + GetPath());
        }

        return data;
    }

    // Check and request necessary permissions for writing to external storage
    public static void CheckPermissions()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            while (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }

    // Create a new save with default data
    public static PlayerData NewSave()
    {
        //PlayerData emptyData = new PlayerData();  Create new data
        //GameEvents.instance.NewGame();  //Start new game
        Debug.Log("New Save Created");
        return emptyData;
    }

    // Check if a save file exists
    public static bool SaveExists()
    {
        return File.Exists(GetPath());
    }

    // Get the path for saving and loading data
    private static string GetPath()
    {
        if (Application.isEditor)
        {
            return Path.Combine(Application.dataPath, "data.json");
        }
        else
        {
            return Path.Combine(Application.persistentDataPath, "data.json");
        }
    }

    // Serialize a list of ScriptableObjects to a JSON string
    public static string SerializeSOList<T>(List<T> SO_List)
    {
        string result = "";
        foreach (T item in SO_List)
        {
            result += JsonUtility.ToJson(item) + ", ";
        }
        return result;
    }

    // Deserialize a JSON string to a list of ScriptableObjects
    public static List<T> DeserializeSOList<T>(string json_string)
    {
        string[] stringSeparators = new string[] { "}," };
        List<T> result = new List<T>();
        string[] splitted = json_string.Split(stringSeparators, System.StringSplitOptions.None);
        for (int i = 0; i < splitted.Length - 1; i++)
        {
            string SO_string = splitted[i] + "}";
            Debug.Log(SO_string);
            T itemBasic = (T)System.Activator.CreateInstance(typeof(T));
            JsonUtility.FromJsonOverwrite(SO_string, itemBasic);
            result.Add(itemBasic);
        }
        return result;
    }
}
