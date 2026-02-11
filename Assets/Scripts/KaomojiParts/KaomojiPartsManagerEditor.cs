#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using ENUM;

/// <summary>
/// エディタモードでKaomojiPartDataを検索するユーティリティ
/// </summary>
public static class KaomojiPartsManagerEditor
{
    /// <summary>
    /// エディタで全てのKaomojiPartDataを検索
    /// </summary>
    public static KaomojiPartData[] GetAllPartsInEditor()
    {
        string[] guids = AssetDatabase.FindAssets("t:KaomojiPartData");
        List<KaomojiPartData> parts = new List<KaomojiPartData>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            KaomojiPartData part = AssetDatabase.LoadAssetAtPath<KaomojiPartData>(path);
            if (part != null)
            {
                parts.Add(part);
            }
        }

        return parts.ToArray();
    }

    /// <summary>
    /// エディタで特定の部位のパーツを検索
    /// </summary>
    public static KaomojiPartData[] GetPartsByTypeInEditor(KaomojiPartType type)
    {
        var allParts = GetAllPartsInEditor();
        return allParts.Where(p => p.PartType == type).ToArray();
    }

    /// <summary>
    /// エディタでパーツ名から検索
    /// </summary>
    public static KaomojiPartData GetPartByNameInEditor(string partName)
    {
        var allParts = GetAllPartsInEditor();
        return allParts.FirstOrDefault(p => p.Data.partName == partName);
    }
}

/// <summary>
/// KaomojiPartsManagerのインスペクター拡張
/// </summary>
[CustomEditor(typeof(KaomojiPartsManager))]
public class KaomojiPartsManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        KaomojiPartsManager manager = (KaomojiPartsManager)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("管理ツール", EditorStyles.boldLabel);

        if (GUILayout.Button("キャッシュをリロード"))
        {
            manager.ReloadCache();
            EditorUtility.DisplayDialog("完了", "キャッシュをリロードしました", "OK");
        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("全パーツを表示"))
        {
            var parts = KaomojiPartsManagerEditor.GetAllPartsInEditor();
            Debug.Log($"=== 全パーツ一覧 ({parts.Length}個) ===");
            foreach (var part in parts)
            {
                Debug.Log($"[{part.PartType}] {part.Data.partName} ({part.Data.part})");
            }
        }

        EditorGUILayout.Space(5);

        // 部位別カウント表示
        EditorGUILayout.LabelField("部位別パーツ数", EditorStyles.boldLabel);
        var allParts = KaomojiPartsManagerEditor.GetAllPartsInEditor();
        var grouped = allParts.GroupBy(p => p.PartType);

        foreach (var group in grouped.OrderBy(g => g.Key))
        {
            EditorGUILayout.LabelField($"  {group.Key}: {group.Count()}個");
        }
    }
}
#endif