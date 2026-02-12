using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Constants;
using ENUM;

public class EnemyDataCreator : EditorWindow
{
    // 基本情報
    private string enemyName = "";
    private string fileName = "";
    
    // 顔文字パーツ
    private KaomojiPartData eyes = null;
    private KaomojiPartData mouth = null;
    private KaomojiPartData hands = null;
    private KaomojiPartData decorationFirst = null;
    private KaomojiPartData decorationSecond = null;
    
    // MentalData
    private MentalData mentalData = null;
    
    // 基礎ステータス
    private float speed = 10f;
    private float health = 10f;
    private float power = 1f;
    private float guard = 1f;

    const float MIN_SPEED = 1f; const float MAX_SPEED = 10f;
    const int MIN_HEALTH = 1; const int MAX_HEALTH = 100;
    const int MIN_POWER = 5; const float MAX_POWER = 20;
    const float MIN_GUARD = 0f; const float MAX_GUARD = 20f;
    
    // Movement設定
    private float maxDragDistance = 3f;
    private float minLaunchDistance = 0.1f;
    private float maxReflectCount = 5f;
    
    // 行動タイミング
    private float launchWaitTime = 1.5f;
    private float launchCooldown = 1.0f;
    private float launchInterval = 2.5f;
    
    // プレビュー
    private Vector2 scrollPosition;
    
    // 保存先
    private string savePath = "Assets/Resources/Enemies";

    [MenuItem("Tools/Enemy/敵データ作成ツール")]
    public static void ShowWindow()
    {
        var window = GetWindow<EnemyDataCreator>("敵データ作成");
        window.minSize = new Vector2(700, 600);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.Space(5);
        
        // ========== タイトルとプレビュー（上部固定） ==========
        DrawHeaderAndPreview();
        
        EditorGUILayout.Space(5);
        
        // ========== 2カラムレイアウト ==========
        EditorGUILayout.BeginHorizontal();
        
        // 左カラム
        EditorGUILayout.BeginVertical(GUILayout.Width(340));
        DrawLeftColumn();
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        // 右カラム
        EditorGUILayout.BeginVertical(GUILayout.Width(340));
        DrawRightColumn();
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);

        // ========== アクションボタン（下部固定） ==========
        DrawActionButtons();
        
        EditorGUILayout.Space(5);

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// ヘッダーとプレビュー
    /// </summary>
    private void DrawHeaderAndPreview()
    {
        EditorGUILayout.BeginVertical("box");
        
        // タイトル
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.LabelField("敵データ作成ツール", titleStyle);
        
        EditorGUILayout.Space(5);
        
        // 顔文字プレビュー
        try
        {
            string leftFaceline = "";
            string rightFaceline = "";
            
            if (mentalData != null && !string.IsNullOrEmpty(mentalData.faceline))
            {
                if (mentalData.faceline.Length >= 2)
                {
                    leftFaceline = mentalData.faceline[0].ToString();
                    rightFaceline = mentalData.faceline[1].ToString();
                }
                else if (mentalData.faceline.Length == 1)
                {
                    leftFaceline = mentalData.faceline[0].ToString();
                    rightFaceline = mentalData.faceline[0].ToString();
                }
            }
            
            string leftEye = eyes != null && eyes.Data.part.Length >= 1 ? eyes.Data.part[0].ToString() : "？";
            string rightEye = eyes != null && eyes.Data.part.Length >= 2 ? eyes.Data.part[1].ToString() : leftEye;
            string mouthStr = mouth != null ? mouth.Data.part : "？";
            
            string kaomoji = leftFaceline + leftEye + mouthStr + rightEye + rightFaceline;
            
            GUIStyle kaomojiStyle = new GUIStyle(EditorStyles.label);
            kaomojiStyle.fontSize = 28;
            kaomojiStyle.alignment = TextAnchor.MiddleCenter;
            kaomojiStyle.normal.textColor = Color.white;
            
            EditorGUILayout.LabelField(kaomoji, kaomojiStyle, GUILayout.Height(35));
        }
        catch (System.Exception )
        {
            EditorGUILayout.LabelField($"プレビューエラー", EditorStyles.centeredGreyMiniLabel);
        }
        
        // 名前とファイル名
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        enemyName = EditorGUILayout.TextField("敵の名前", enemyName, GUILayout.Width(300));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        if (!string.IsNullOrEmpty(enemyName))
        {
            fileName = enemyName.Replace(" ", "_");
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle fileStyle = new GUIStyle(EditorStyles.miniLabel);
            fileStyle.normal.textColor = Color.cyan;
            EditorGUILayout.LabelField($"→ {fileName}.asset", fileStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 左カラム（顔文字・MentalData）
    /// </summary>
    private void DrawLeftColumn()
    {
        // MentalData
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("🧠 MentalData", EditorStyles.boldLabel);
        mentalData = (MentalData)EditorGUILayout.ObjectField(mentalData, typeof(MentalData), false, GUILayout.Height(20));
        
        if (mentalData != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"精神力: {mentalData.maxMental}", GUILayout.Width(100));
            EditorGUILayout.LabelField($"括弧: {mentalData.faceline}");
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(5);
        
        // 顔文字パーツ
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("😀 顔文字パーツ", EditorStyles.boldLabel);
        
        eyes = (KaomojiPartData)EditorGUILayout.ObjectField("目", eyes, typeof(KaomojiPartData), false);
        mouth = (KaomojiPartData)EditorGUILayout.ObjectField("口", mouth, typeof(KaomojiPartData), false);
        hands = (KaomojiPartData)EditorGUILayout.ObjectField("手", hands, typeof(KaomojiPartData), false);
        decorationFirst = (KaomojiPartData)EditorGUILayout.ObjectField("装飾1", decorationFirst, typeof(KaomojiPartData), false);
        decorationSecond = (KaomojiPartData)EditorGUILayout.ObjectField("装飾2", decorationSecond, typeof(KaomojiPartData), false);
        
        int partsCount = 0;
        if (eyes != null) partsCount++;
        if (mouth != null) partsCount++;
        if (hands != null) partsCount++;
        if (decorationFirst != null) partsCount++;
        if (decorationSecond != null) partsCount++;
        
        GUIStyle countStyle = new GUIStyle(EditorStyles.miniLabel);
        countStyle.normal.textColor = partsCount >= 2 ? Color.green : Color.yellow;
        countStyle.alignment = TextAnchor.MiddleRight;
        EditorGUILayout.LabelField($"設定済み: {partsCount}/5", countStyle);
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(5);
        
        // 行動タイミング
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("⏱️ 行動タイミング", EditorStyles.boldLabel);
        
        launchWaitTime = EditorGUILayout.Slider("待機", launchWaitTime, 0.1f, 5f);
        launchCooldown = EditorGUILayout.Slider("硬直", launchCooldown, 0.1f, 3f);
        launchInterval = EditorGUILayout.Slider("間隔", launchInterval, 0.5f, 10f);
        
        float totalCycle = launchWaitTime + launchCooldown + launchInterval;
        GUIStyle cycleStyle = new GUIStyle(EditorStyles.miniLabel);
        cycleStyle.normal.textColor = Color.cyan;
        cycleStyle.alignment = TextAnchor.MiddleRight;
        EditorGUILayout.LabelField($"サイクル: {totalCycle:F2}秒", cycleStyle);
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 右カラム（ステータス）
    /// </summary>
    private void DrawRightColumn()
    {
        // 基礎ステータス
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("💪 基礎ステータス", EditorStyles.boldLabel);
        
        speed = EditorGUILayout.Slider("速度", speed, MIN_SPEED, MAX_SPEED);
        health = EditorGUILayout.Slider("体力", health, MIN_HEALTH, MAX_HEALTH);
        power = EditorGUILayout.Slider("攻撃力", power, MIN_HEALTH, MAX_HEALTH);
        guard = EditorGUILayout.Slider("防御力", guard, MIN_GUARD, MAX_GUARD);
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(5);
        
        // Movement設定
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("🎯 Movement設定", EditorStyles.boldLabel);
        
        maxDragDistance = EditorGUILayout.Slider("ドラッグ最大", maxDragDistance, 1f, 10f);
        minLaunchDistance = EditorGUILayout.Slider("発射最小", minLaunchDistance, 0.1f, 1f);
        maxReflectCount = EditorGUILayout.Slider("反射回数", maxReflectCount, 1f, 10f);
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(5);
        
        // ステータスサマリー
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("📊 サマリー", EditorStyles.boldLabel);
        
        GUIStyle summaryStyle = new GUIStyle(EditorStyles.miniLabel);
        summaryStyle.wordWrap = true;
        
        EditorGUILayout.LabelField($"速度:{speed:F1} 体力:{health:F1}", summaryStyle);
        EditorGUILayout.LabelField($"攻撃:{power:F1} 防御:{guard:F1}", summaryStyle);
        EditorGUILayout.LabelField($"サイクル:{(launchWaitTime + launchCooldown + launchInterval):F2}秒", summaryStyle);
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(5);
        
        // 保存先
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("💾 保存先", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        savePath = EditorGUILayout.TextField(savePath);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string path = EditorUtility.OpenFolderPanel("保存先を選択", "Assets", "");
            if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
            {
                savePath = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// アクションボタン
    /// </summary>
    private void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.FlexibleSpace();
        
        GUI.backgroundColor = new Color(1f, 0.8f, 0.6f);
        if (GUILayout.Button("↻ リセット", GUILayout.Height(35), GUILayout.Width(120)))
        {
            if (EditorUtility.DisplayDialog("確認", "入力をリセットしますか？", "はい", "いいえ"))
            {
                ResetFields();
            }
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(10);
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("✓ 作成", GUILayout.Height(35), GUILayout.Width(200)))
        {
            CreateEnemyData();
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.FlexibleSpace();
        
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// EnemyData作成
    /// </summary>
    private void CreateEnemyData()
    {
        // バリデーション
        if (string.IsNullOrEmpty(enemyName))
        {
            EditorUtility.DisplayDialog("エラー", "敵の名前を入力してください", "OK");
            return;
        }
        
        if (eyes == null && mouth == null)
        {
            EditorUtility.DisplayDialog("エラー", "最低でも目か口のパーツを設定してください", "OK");
            return;
        }
        
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        
        // EnemyData作成
        EnemyData newEnemy = CreateInstance<EnemyData>();
        
        // SerializedObjectを使用して設定
        SerializedObject serializedEnemy = new SerializedObject(newEnemy);
        
        // Kaomoji設定
        SerializedProperty kaomojiProp = serializedEnemy.FindProperty("kaomoji");
        if (kaomojiProp != null)
        {
            if (eyes != null)
                kaomojiProp.FindPropertyRelative("eyes").objectReferenceValue = eyes;
            if (mouth != null)
                kaomojiProp.FindPropertyRelative("mouth").objectReferenceValue = mouth;
            if (hands != null)
                kaomojiProp.FindPropertyRelative("hands").objectReferenceValue = hands;
            if (decorationFirst != null)
                kaomojiProp.FindPropertyRelative("decorationFirst").objectReferenceValue = decorationFirst;
            if (decorationSecond != null)
                kaomojiProp.FindPropertyRelative("decorationSecond").objectReferenceValue = decorationSecond;
        }
        
        // Status設定
        SerializedProperty statusProp = serializedEnemy.FindProperty("status");
        if (statusProp != null)
        {
            statusProp.FindPropertyRelative("speed").floatValue = speed;
            statusProp.FindPropertyRelative("health").floatValue = health;
            statusProp.FindPropertyRelative("power").floatValue = power;
            statusProp.FindPropertyRelative("guard").floatValue = guard;
            statusProp.FindPropertyRelative("maxDragDistance").floatValue = maxDragDistance;
            statusProp.FindPropertyRelative("minLaunchDistance").floatValue = minLaunchDistance;
            statusProp.FindPropertyRelative("maxReflectCount").floatValue = maxReflectCount;
            
            if (mentalData != null)
                statusProp.FindPropertyRelative("mentalData").objectReferenceValue = mentalData;
        }
        
        // Timing設定
        SerializedProperty launchWaitTimeProp = serializedEnemy.FindProperty("launchWaitTime");
        if (launchWaitTimeProp != null)
            launchWaitTimeProp.floatValue = launchWaitTime;
        
        SerializedProperty launchCooldownProp = serializedEnemy.FindProperty("launchCooldown");
        if (launchCooldownProp != null)
            launchCooldownProp.floatValue = launchCooldown;
        
        SerializedProperty launchIntervalProp = serializedEnemy.FindProperty("launchInterval");
        if (launchIntervalProp != null)
            launchIntervalProp.floatValue = launchInterval;
        
        serializedEnemy.ApplyModifiedProperties();
        
        // 保存
        string assetPath = $"{savePath}/{fileName}.asset";
        
        if (AssetDatabase.LoadAssetAtPath<EnemyData>(assetPath) != null)
        {
            if (!EditorUtility.DisplayDialog("警告", $"'{fileName}.asset' を上書きしますか？", "上書き", "キャンセル"))
            {
                DestroyImmediate(newEnemy);
                return;
            }
        }
        
        AssetDatabase.CreateAsset(newEnemy, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("成功", $"敵データを作成しました！\n{fileName}.asset", "OK");
        
        Selection.activeObject = newEnemy;
        EditorGUIUtility.PingObject(newEnemy);
    }

    /// <summary>
    /// フィールドリセット
    /// </summary>
    private void ResetFields()
    {
        enemyName = "";
        fileName = "";
        eyes = null;
        mouth = null;
        hands = null;
        decorationFirst = null;
        decorationSecond = null;
        mentalData = null;
        speed = 10f;
        health = 10f;
        power = 1f;
        guard = 1f;
        maxDragDistance = 3f;
        minLaunchDistance = 0.1f;
        maxReflectCount = 5f;
        launchWaitTime = 1.5f;
        launchCooldown = 1.0f;
        launchInterval = 2.5f;
        
        Repaint();
    }
}