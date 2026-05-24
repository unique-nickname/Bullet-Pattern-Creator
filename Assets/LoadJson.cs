using System.IO;
using UnityEngine;

public class LoadJson : MonoBehaviour
{
    public void LoadJsonFile()
    {
        string fileName = "test1.json";
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (!File.Exists(path)) {
            Debug.LogError($"JSON file not found at: {path}");
            return;
        }

        string json = File.ReadAllText(path);

        if (PatternDataManager.Instance == null) {
            Debug.LogError("PatternDataManager instance not found in the scene.");
            return;
        }

        bool success = PatternDataManager.Instance.LoadFromJson(json);

        GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineRenderer>().totalDurationMs = PatternDataManager.Instance.Info.end_time;
        GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineRenderer>().Refresh();
    }
}
