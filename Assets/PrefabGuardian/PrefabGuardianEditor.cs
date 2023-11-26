using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PrefabGuardianEditor
{
    static PrefabGuardianEditor()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnSelectionChanged()
    {
        // Sceneウィンドウの選択をチェック
        GameObject selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject != null)
        {
            CheckPrefab(selectedGameObject);
        }

        // Projectウィンドウの選択をチェック
        CheckProjectWindowSelection();
    }

    private static void CheckPrefab(GameObject gameObject)
    {
        var config = PrefabGuardianConfig.LoadConfig();
        var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);

        if (prefabStatus != PrefabInstanceStatus.NotAPrefab)
        {
            string prefabGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject)));
            int index = Array.IndexOf(config.prefabGUIDs, prefabGUID);

            if (index != -1)
            {
                string userName = config.userNames[index];
                string savedUserName = EditorPrefs.GetString("PrefabGuardian_UserName", "");

                if (!string.IsNullOrEmpty(savedUserName) && !savedUserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                {
                    ShowWarning(userName, gameObject.name);
                }
            }
        }
    }

    private static void CheckProjectWindowSelection()
    {
        UnityEngine.Object selectedAsset = Selection.activeObject;
        if (selectedAsset != null)
        {
            var config = PrefabGuardianConfig.LoadConfig();
            string assetPath = AssetDatabase.GetAssetPath(selectedAsset);
            GameObject assetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (assetPrefab != null)
            {
                string prefabGUID = AssetDatabase.AssetPathToGUID(assetPath);
                int index = Array.IndexOf(config.prefabGUIDs, prefabGUID);

                if (index != -1)
                {
                    string userName = config.userNames[index];
                    string savedUserName = EditorPrefs.GetString("PrefabGuardian_UserName", "");

                    if (!string.IsNullOrEmpty(savedUserName) && !savedUserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    {
                        ShowWarning(userName, assetPrefab.name);
                    }
                }
            }
        }
    }

    private static void ShowWarning(string userName, string prefabName)
    {
        string message = $"Warning: '{prefabName}' is under the management of '{userName}'. " +
                         $"Please contact '{userName}' to avoid conflicts.";
        EditorUtility.DisplayDialog("PrefabGuardian Warning", message, "OK");
    }
}
