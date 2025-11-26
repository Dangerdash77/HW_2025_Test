using System;
using System.IO;
using UnityEngine;

// --- JSON DATA STRUCTURES (Serializable Classes) ---

// Holds configuration data for the player. 
[Serializable]
public class PlayerData
{
    public float speed;
}

// Holds configuration data for the platforms (pulpits).
[Serializable]
public class PulpitData
{
    public float min_pulpit_destroy_time;
    public float max_pulpit_destroy_time;
    public float pulpit_spawn_time;
}

// The root object for the JSON configuration file (doofus_diary.json).
[Serializable]
public class DoofusDiaryRoot
{
    public PlayerData player_data;
    public PulpitData pulpit_data;
}


// --- LOADER SCRIPT ---
// Loads game configuration data from the 'doofus_diary.json' file in StreamingAssets.
public class DiaryLoader : MonoBehaviour
{
    // Static property to access the loaded configuration data from anywhere
    public static DoofusDiaryRoot Diary;

    // UNITY LIFECYCLE METHODS

    void Awake()
    {
        // Construct the full path to the JSON file in the StreamingAssets folder
        string path = Path.Combine(UnityEngine.Application.streamingAssetsPath, "doofus_diary.json");

        // Check if the file exists
        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogError("[DiaryLoader] doofus_diary.json NOT FOUND! Place it in Assets/StreamingAssets/");
            return;
        }

        // Attempt to read and parse the JSON file
        try
        {
            string json = File.ReadAllText(path);
            // Deserialize the JSON string into the static data structure
            Diary = JsonUtility.FromJson<DoofusDiaryRoot>(json);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[DiaryLoader] Error reading diary: " + e.Message);
        }
    }
}