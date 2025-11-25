using System;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public float speed;
}

[Serializable]
public class PulpitData
{
    public float min_pulpit_destroy_time;
    public float max_pulpit_destroy_time;
    public float pulpit_spawn_time;
}

[Serializable]
public class DoofusDiaryRoot
{
    public PlayerData player_data;
    public PulpitData pulpit_data;
}

public class DiaryLoader : MonoBehaviour
{
    public static DoofusDiaryRoot Diary;

    void Awake()
    {
        string path = Path.Combine(UnityEngine.Application.streamingAssetsPath, "doofus_diary.json");

        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogError("[DiaryLoader] doofus_diary.json NOT FOUND! Place it in Assets/StreamingAssets/");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            Diary = JsonUtility.FromJson<DoofusDiaryRoot>(json);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[DiaryLoader] Error reading diary: " + e.Message);
        }
    }
}
