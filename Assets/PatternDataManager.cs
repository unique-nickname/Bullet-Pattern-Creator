using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PatternInfo
{
    public string pattern_name;
    public string creator;
    public string date_of_creation;
    public string description;
    public int end_time;
}

[Serializable]
public class BulletEvent
{
    public string type;
    public float direction;
    public float speed;
    public float scale;
    public Vector2 start_position;
    public int spawn_time;
    public int life_time;
}

[Serializable]
public class PatternData
{
    public PatternInfo info = new PatternInfo();
    public List<BulletEvent> bullets = new List<BulletEvent>();
}

public class PatternDataManager : MonoBehaviour
{
    public static PatternDataManager Instance { get; private set; }

    [SerializeField] private PatternData currentPattern = new PatternData();

    public PatternData CurrentPattern => currentPattern;
    public PatternInfo Info => currentPattern.info;
    public List<BulletEvent> Bullets => currentPattern.bullets;

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetDefaultData();
    }

    private void SetDefaultData()
    {
        currentPattern.info.pattern_name = "Example Pattern";
        currentPattern.info.creator = "Example Creator";
        currentPattern.info.date_of_creation = "2026-04-19";
        currentPattern.info.description = "This is an example pattern.";
        currentPattern.info.end_time = 0;
    }

    public string ToJson(bool prettyPrint = true)
    {
        return JsonUtility.ToJson(currentPattern, prettyPrint);
    }

    public bool LoadFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) {
            Debug.LogError("JSON string is null or empty.");
            return false;
        }

        try {
            PatternData loadedData = JsonUtility.FromJson<PatternData>(json);

            if (loadedData == null) {
                Debug.LogError("Failed to parse pattern JSON.");
                return false;
            }

            if (loadedData.info == null)
                loadedData.info = new PatternInfo();

            if (loadedData.bullets == null)
                loadedData.bullets = new List<BulletEvent>();

            currentPattern = loadedData;
            return true;
        }
        catch (Exception e) {
            Debug.LogError($"Failed to parse JSON: {e.Message}");
            return false;
        }
    }

    public void ClearPattern()
    {
        currentPattern = new PatternData();
        SetDefaultData();
    }
}