using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.Android;
using Newtonsoft.Json;

public static class SaveSystem
{
    public static void Save(PlayerData data)
    {
        try
        {
            string jsonData = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(GetPath(), FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(jsonData);
                }

                stream.Close();
            }

            Debug.Log("Saved Game");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError("Error occured when trying to save the data to a file: " + GetPath());
        }
    }

    public static PlayerData Load()
    {
        PlayerData data = null;

        try
        {
            if (File.Exists(GetPath()))
            {
                string jsonData = "";
                using (FileStream stream = new FileStream(GetPath(), FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonData = reader.ReadToEnd();
                    }
                }

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
            Debug.LogError("Error occured when trying to load the data from file: " + GetPath());
        }

        return data;
    }


    public static void CheckPermissions()
    {
        // Request permission to write to external storage
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            while (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }

    public static PlayerData NewSave()
    {
        PlayerData emptyData = new PlayerData();
        GameEvents.instance.NewGame();
        Debug.Log("New Save Created");
        return emptyData;
    }

    public static bool SaveExists()
    {
        return File.Exists(GetPath());
    }

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

    public static string SerializeSOList<T>(List<T> SO_List)
    {
        string result = "";
        foreach (T item in SO_List)
        {
            result += JsonUtility.ToJson(item) + ", ";
        }
        return result;
    }

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
