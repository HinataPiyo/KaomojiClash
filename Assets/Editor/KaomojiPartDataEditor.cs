using UnityEngine;
using UnityEditor;
using Constants;

[CustomEditor(typeof(KaomojiPartData))]
public class KaomojiPartDataEditor : Editor
{
    SerializedProperty dropProbability;
    SerializedProperty partType;
    SerializedProperty data;

    private void OnEnable()
    {
        // SerializedPropertyを取得
        dropProbability = serializedObject.FindProperty("dorpProbability");
        partType = serializedObject.FindProperty("partType");
        data = serializedObject.FindProperty("data");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ヘッダー
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("顔文字パーツデータ設定", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // ドロップ確率
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("ドロップ設定", EditorStyles.boldLabel);
        EditorGUILayout.Slider(dropProbability, 0.01f, 0.8f, new GUIContent("ドロップ確率", "このパーツのドロップ確率 (1% ~ 80%)"));
        EditorGUILayout.HelpBox($"実際の確率: {dropProbability.floatValue * 100:F1}%", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // パーツタイプ
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("パーツタイプ", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(partType, new GUIContent("部位", "顔文字のどの部位か"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // KaomojiPart データ
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("パーツデータ", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(data, new GUIContent("データ", "パーツの詳細データ"), true);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // プレビュー表示（structの場合）
        if (data != null)
        {
            EditorGUILayout.BeginVertical("helpbox");
            EditorGUILayout.LabelField("プレビュー", EditorStyles.boldLabel);
            
            // structの中のフィールドに直接アクセス
            SerializedProperty partName = data.FindPropertyRelative("partName");
            SerializedProperty part = data.FindPropertyRelative("part");
            
            if (partName != null && !string.IsNullOrEmpty(partName.stringValue))
            {
                EditorGUILayout.LabelField("パーツ名", partName.stringValue);
            }
            
            if (part != null && !string.IsNullOrEmpty(part.stringValue))
            {
                GUIStyle largeStyle = new GUIStyle(EditorStyles.label);
                largeStyle.fontSize = 24;
                largeStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField("記号", part.stringValue, largeStyle);
            }
            
            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}