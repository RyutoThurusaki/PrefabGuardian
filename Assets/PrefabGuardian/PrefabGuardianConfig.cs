using System;
using System.IO;
using UnityEngine;

[Serializable]
public class PrefabGuardianConfig
{
    public string[] prefabGUIDs; // PrefabのGUID配列
    public string[] userNames; // ユーザー名配列

    private static readonly string configFilePath = Path.Combine(Application.dataPath, "PrefabGuardian", "PrefabGuardianConfig.json");

    public static PrefabGuardianConfig LoadConfig()
    {
        if (!File.Exists(configFilePath))
        {
            return new PrefabGuardianConfig()
            {
                prefabGUIDs = new string[0],
                userNames = new string[0]
            };
        }

        string json = File.ReadAllText(configFilePath);
        return JsonUtility.FromJson<PrefabGuardianConfig>(json);
    }

    public static void SaveConfig(PrefabGuardianConfig config)
    {
        string json = JsonUtility.ToJson(config, true);
        Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
        File.WriteAllText(configFilePath, json);
    }
}
