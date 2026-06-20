using System.IO;
using UnityEngine;


public static class SaveSystem
{
    static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static PlayerSave Load()
    {
        if (!File.Exists(FilePath))
        {
            Debug.Log("[SaveSystem] No save found — creating new PlayerSave.");
            return new PlayerSave();
        }

        try
        {
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<PlayerSave>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[SaveSystem] Failed to parse save.json: {ex.Message}. Starting fresh.");
            return new PlayerSave();
        }
    }

    public static void Save(PlayerSave data)
    {
        try
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(data, prettyPrint: true));
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveSystem] Write failed: {ex.Message}");
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(FilePath)) File.Delete(FilePath);
        Debug.Log("[SaveSystem] Save deleted.");
    }

    public static string SavePath => FilePath;
}
