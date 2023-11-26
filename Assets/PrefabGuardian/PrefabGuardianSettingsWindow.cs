using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PrefabGuardianSettingsWindow : EditorWindow
{
    private PrefabGuardianConfig config;
    private Vector2 scrollPosition;
    private string userName = "";

    [MenuItem("Tools/PrefabGuardian/Settings")]
    private static void ShowWindow()
    {
        var window = GetWindow<PrefabGuardianSettingsWindow>();
        window.titleContent = new GUIContent("PrefabGuardian Settings");
        window.Show();
    }

    private void OnEnable()
    {
        config = PrefabGuardianConfig.LoadConfig();
        // 保存されたユーザー名を読み込む
        userName = EditorPrefs.GetString("PrefabGuardian_UserName", "");
    }

    private void OnGUI()
    {
        GUILayout.Label("User Settings", EditorStyles.boldLabel);
        // ユーザー名の入力フィールド
        userName = EditorGUILayout.TextField("User Name", userName);

        // ユーザー名を保存
        if (GUILayout.Button("Save User Name"))
        {
            EditorPrefs.SetString("PrefabGuardian_UserName", userName);
            EditorUtility.DisplayDialog("User Name Saved", "Your user name has been saved successfully.", "OK");
        }

        GUILayout.Space(20);

        GUILayout.Label("Protected Prefabs", EditorStyles.boldLabel);

        // 設定内容のスクロールビュー
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (config.prefabGUIDs == null || config.userNames == null)
        {
            EditorGUILayout.HelpBox("There are no prefabs or usernames defined in the config.", MessageType.Info);
            if (GUILayout.Button("Create New Config"))
            {
                config.prefabGUIDs = new string[0];
                config.userNames = new string[0];
            }
        }
        else
        {
            for (int i = 0; i < config.prefabGUIDs.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                string assetPath = AssetDatabase.GUIDToAssetPath(config.prefabGUIDs[i]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

                config.userNames[i] = EditorGUILayout.TextField("User Name", config.userNames[i]);

                if (GUILayout.Button("Remove"))
                {
                    RemoveEntry(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();

                if (prefab != null)
                {
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab));
                    config.prefabGUIDs[i] = guid;
                }
            }

            if (GUILayout.Button("Add Protected Prefab"))
            {
                AddEntry();
            }
        }

        EditorGUILayout.EndScrollView();

        // 変更を保存するボタン
        if (GUILayout.Button("Save"))
        {
            SaveConfig();
        }
    }

    private void RemoveEntry(int index)
    {
        var guidsList = new List<string>(config.prefabGUIDs);
        var namesList = new List<string>(config.userNames);
        guidsList.RemoveAt(index);
        namesList.RemoveAt(index);
        config.prefabGUIDs = guidsList.ToArray();
        config.userNames = namesList.ToArray();
    }

    private void AddEntry()
    {
        var guidsList = new List<string>(config.prefabGUIDs) { string.Empty };
        var namesList = new List<string>(config.userNames) { string.Empty };
        config.prefabGUIDs = guidsList.ToArray();
        config.userNames = namesList.ToArray();
    }

    private void SaveConfig()
    {
        // 新しい設定を作成して保存
        var newConfig = new PrefabGuardianConfig { prefabGUIDs = config.prefabGUIDs, userNames = config.userNames };
        PrefabGuardianConfig.SaveConfig(newConfig);
        EditorUtility.DisplayDialog("Config Saved", "Your changes have been saved successfully.", "OK");
    }
}
