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

    // 顔文字パーツ（ENUM.KaomojiPartTypeの順番）
    private KaomojiPartData mouth = null;
    private KaomojiPartData eyes = null;
    private KaomojiPartData hands = null;
    private KaomojiPartData decorationFirst = null;
    private KaomojiPartData decorationSecond = null;

    // MentalData
    private MentalData mentalData = null;

    // ステータス定数
    private const float SPEED_MIN = 1f; private const float SPEED_MAX = 10f;
    private const float HEALTH_MIN = 10; private const float HEALTH_MAX = 100;
    private const float POWER_MIN = 5; private const float POWER_MAX = 20;
    private const float GUARD_MIN = 0f; private const float GUARD_MAX = 10f;
    private const float MAX_DRAG_MIN = 1f; private const float MAX_DRAG_MAX = 10f;
    private const float MIN_LAUNCH_MIN = 0.1f; private const float MIN_LAUNCH_MAX = 1f;
    private const float MAX_REFLECT_MIN = 1f; private const float MAX_REFLECT_MAX = 10f;
    private const float LAUNCH_WAIT_MIN = 0.1f; private const float LAUNCH_WAIT_MAX = 5f;
    private const float LAUNCH_COOLDOWN_MIN = 0.1f; private const float LAUNCH_COOLDOWN_MAX = 3f;
    private const float LAUNCH_INTERVAL_MIN = 0.5f; private const float LAUNCH_INTERVAL_MAX = 10f;

    // 基礎ステータス
    private float speed = 2f;
    private float health = 10f;
    private float power = 5f;
    private float guard = 1f;

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
            string kaomoji = BuildKaomojiPreview();

            if (!string.IsNullOrEmpty(kaomoji))
            {
                GUIStyle kaomojiStyle = new GUIStyle(EditorStyles.label);
                kaomojiStyle.fontSize = 28;
                kaomojiStyle.alignment = TextAnchor.MiddleCenter;
                kaomojiStyle.normal.textColor = Color.white;

                EditorGUILayout.LabelField(kaomoji, kaomojiStyle, GUILayout.Height(35));
            }
            else
            {
                GUIStyle emptyStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                emptyStyle.fontSize = 12;
                EditorGUILayout.LabelField("（パーツを設定してください）", emptyStyle, GUILayout.Height(35));
            }
        }
        catch (System.Exception)
        {
            EditorGUILayout.LabelField($"プレビューエラー", EditorStyles.centeredGreyMiniLabel, GUILayout.Height(35));
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

    // <summary>
    /// 顔文字プレビューを構築
    /// </summary>
    private string BuildKaomojiPreview()
    {
        string result = "";

        try
        {
            // MentalData（括弧）
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

            result += leftFaceline;

            // Hands（手）- 左側
            if (hands != null && hands.Data != null && !string.IsNullOrEmpty(hands.Data.part))
            {
                if (hands.Data.part.Length >= 1)
                {
                    result += hands.Data.part[0];
                }
            }

            // DecorationFirst（装飾1）- 左側
            if (decorationFirst != null && decorationFirst.Data != null && !string.IsNullOrEmpty(decorationFirst.Data.part))
            {
                if (decorationFirst.Data.part.Length >= 1)
                {
                    result += decorationFirst.Data.part[0];
                }
            }

            // Eyes（目）- 左目
            if (eyes != null && eyes.Data != null && !string.IsNullOrEmpty(eyes.Data.part))
            {
                if (eyes.Data.part.Length >= 1)
                {
                    result += eyes.Data.part[0];
                }
            }

            // Mouth（口）
            if (mouth != null && mouth.Data != null && !string.IsNullOrEmpty(mouth.Data.part))
            {
                result += mouth.Data.part;
            }

            // Eyes（目）- 右目
            if (eyes != null && eyes.Data != null && !string.IsNullOrEmpty(eyes.Data.part))
            {
                if (eyes.Data.part.Length >= 2)
                {
                    result += eyes.Data.part[1];
                }
                else if (eyes.Data.part.Length >= 1)
                {
                    result += eyes.Data.part[0];
                }
            }

            // DecorationFirst（装飾1）- 右側
            if (decorationFirst != null && decorationFirst.Data != null && !string.IsNullOrEmpty(decorationFirst.Data.part))
            {
                if (decorationFirst.Data.part.Length >= 2)
                {
                    result += decorationFirst.Data.part[1];
                }
                else if (decorationFirst.Data.part.Length >= 1)
                {
                    result += decorationFirst.Data.part[0];
                }
            }

            // Hands（手）- 右側
            if (hands != null && hands.Data != null && !string.IsNullOrEmpty(hands.Data.part))
            {
                if (hands.Data.part.Length >= 2)
                {
                    result += hands.Data.part[1];
                }
                else if (hands.Data.part.Length >= 1)
                {
                    result += hands.Data.part[0];
                }
            }

            // DecorationSecond（装飾2）
            if (decorationSecond != null && decorationSecond.Data != null && !string.IsNullOrEmpty(decorationSecond.Data.part))
            {
                result += decorationSecond.Data.part;
            }

            result += rightFaceline;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"顔文字プレビュー構築エラー: {e.Message}");
            return "";
        }

        return result;
    }

    /// <summary>
    /// パーツタイプが正しいかチェック
    /// </summary>
    private bool ValidatePartType(KaomojiPartData part, KaomojiPartType expectedType, string label)
    {
        if (part == null) return true; // nullは許容

        // KaomojiPartDataのPartTypeプロパティがnullまたはアクセスできない場合のチェック
        try
        {
            if (part.PartType != expectedType)
            {
                GUIStyle errorStyle = new GUIStyle(EditorStyles.miniLabel);
                errorStyle.normal.textColor = Color.red;
                errorStyle.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField($"⚠ {label}: 正しいタイプは {expectedType} です", errorStyle);
                return false;
            }
        }
        catch (System.Exception)
        {
            // PartTypeにアクセスできない場合もエラーとして扱う
            GUIStyle errorStyle = new GUIStyle(EditorStyles.miniLabel);
            errorStyle.normal.textColor = Color.red;
            errorStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField($"⚠ {label}: パーツデータが無効です", errorStyle);
            return false;
        }

        return true;
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

        // ��文字パーツ（ENUM順）
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("😀 顔文字パーツ", EditorStyles.boldLabel);

        // Mouth
        EditorGUI.BeginChangeCheck();
        mouth = (KaomojiPartData)EditorGUILayout.ObjectField("口（Mouth）", mouth, typeof(KaomojiPartData), false);
        if (EditorGUI.EndChangeCheck())
        {
            ValidatePartType(mouth, KaomojiPartType.Mouth, "口");
            Repaint();
        }

        // Eyes
        EditorGUI.BeginChangeCheck();
        eyes = (KaomojiPartData)EditorGUILayout.ObjectField("目（Eyes）", eyes, typeof(KaomojiPartData), false);
        if (EditorGUI.EndChangeCheck())
        {
            ValidatePartType(eyes, KaomojiPartType.Eyes, "目");
            Repaint();
        }

        // Hands
        EditorGUI.BeginChangeCheck();
        hands = (KaomojiPartData)EditorGUILayout.ObjectField("手（Hands）", hands, typeof(KaomojiPartData), false);
        if (EditorGUI.EndChangeCheck())
        {
            ValidatePartType(hands, KaomojiPartType.Hands, "手");
            Repaint();
        }

        // Decoration_First
        EditorGUI.BeginChangeCheck();
        decorationFirst = (KaomojiPartData)EditorGUILayout.ObjectField("装飾1", decorationFirst, typeof(KaomojiPartData), false);
        if (EditorGUI.EndChangeCheck())
        {
            ValidatePartType(decorationFirst, KaomojiPartType.Decoration_First, "装飾1");
            Repaint();
        }

        // Decoration_Second
        EditorGUI.BeginChangeCheck();
        decorationSecond = (KaomojiPartData)EditorGUILayout.ObjectField("装飾2", decorationSecond, typeof(KaomojiPartData), false);
        if (EditorGUI.EndChangeCheck())
        {
            ValidatePartType(decorationSecond, KaomojiPartType.Decoration_Second, "装飾2");
            Repaint();
        }

        int partsCount = 0;
        if (mouth != null) partsCount++;
        if (eyes != null) partsCount++;
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

        launchWaitTime = EditorGUILayout.Slider("待機", launchWaitTime, LAUNCH_WAIT_MIN, LAUNCH_WAIT_MAX);
        launchCooldown = EditorGUILayout.Slider("硬直", launchCooldown, LAUNCH_COOLDOWN_MIN, LAUNCH_COOLDOWN_MAX);
        launchInterval = EditorGUILayout.Slider("間隔", launchInterval, LAUNCH_INTERVAL_MIN, LAUNCH_INTERVAL_MAX);

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

        speed = EditorGUILayout.Slider("速度", speed, SPEED_MIN, SPEED_MAX);
        health = EditorGUILayout.Slider("体力", health, HEALTH_MIN, HEALTH_MAX);
        power = EditorGUILayout.Slider("攻撃力", power, POWER_MIN, POWER_MAX);
        guard = EditorGUILayout.Slider("防御力", guard, GUARD_MIN, GUARD_MAX);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(5);

        // Movement設定
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("🎯 Movement設定", EditorStyles.boldLabel);

        maxDragDistance = EditorGUILayout.Slider("ドラッグ最大", maxDragDistance, MAX_DRAG_MIN, MAX_DRAG_MAX);
        minLaunchDistance = EditorGUILayout.Slider("発射最小", minLaunchDistance, MIN_LAUNCH_MIN, MIN_LAUNCH_MAX);
        maxReflectCount = EditorGUILayout.Slider("反射回数", maxReflectCount, MAX_REFLECT_MIN, MAX_REFLECT_MAX);

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

        if (mouth == null && eyes == null)
        {
            EditorUtility.DisplayDialog("エラー", "最低でも目か口のパーツを設定してください", "OK");
            return;
        }

        // パーツタイプの検証（エディタ表示中はスキップ）
        bool hasError = false;
        if (mouth != null && mouth.PartType != KaomojiPartType.Mouth) hasError = true;
        if (eyes != null && eyes.PartType != KaomojiPartType.Eyes) hasError = true;
        if (hands != null && hands.PartType != KaomojiPartType.Hands) hasError = true;
        if (decorationFirst != null && decorationFirst.PartType != KaomojiPartType.Decoration_First) hasError = true;
        if (decorationSecond != null && decorationSecond.PartType != KaomojiPartType.Decoration_Second) hasError = true;

        if (hasError)
        {
            EditorUtility.DisplayDialog("エラー", "正しいタイプのパーツを設定してください", "OK");
            return;
        }

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        // EnemyData作成
        EnemyData newEnemy = CreateInstance<EnemyData>();

        if (newEnemy == null)
        {
            EditorUtility.DisplayDialog("エラー", "EnemyDataの作成に失敗しました", "OK");
            return;
        }

        // SerializedObjectを使用して設定
        SerializedObject serializedEnemy = new SerializedObject(newEnemy);

        // Kaomoji設定
        SerializedProperty kaomojiProp = serializedEnemy.FindProperty("kaomoji");
        if (kaomojiProp != null)
        {
            SerializedProperty mouthProp = kaomojiProp.FindPropertyRelative("mouth");
            if (mouth != null && mouthProp != null)
                mouthProp.objectReferenceValue = mouth;

            SerializedProperty eyesProp = kaomojiProp.FindPropertyRelative("eyes");
            if (eyes != null && eyesProp != null)
                eyesProp.objectReferenceValue = eyes;

            SerializedProperty handsProp = kaomojiProp.FindPropertyRelative("hands");
            if (hands != null && handsProp != null)
                handsProp.objectReferenceValue = hands;

            SerializedProperty decorationFirstProp = kaomojiProp.FindPropertyRelative("decorationFirst");
            if (decorationFirst != null && decorationFirstProp != null)
                decorationFirstProp.objectReferenceValue = decorationFirst;

            SerializedProperty decorationSecondProp = kaomojiProp.FindPropertyRelative("decorationSecond");
            if (decorationSecond != null && decorationSecondProp != null)
                decorationSecondProp.objectReferenceValue = decorationSecond;
        }
        else
        {
            Debug.LogWarning("'kaomoji' property not found in EnemyData!");
        }

        // Status設定
        SerializedProperty statusProp = serializedEnemy.FindProperty("status");
        if (statusProp != null)
        {
            SerializedProperty speedProp = statusProp.FindPropertyRelative("speed");
            if (speedProp != null) speedProp.floatValue = speed;

            SerializedProperty healthProp = statusProp.FindPropertyRelative("health");
            if (healthProp != null) healthProp.floatValue = health;

            SerializedProperty powerProp = statusProp.FindPropertyRelative("power");
            if (powerProp != null) powerProp.floatValue = power;

            SerializedProperty guardProp = statusProp.FindPropertyRelative("guard");
            if (guardProp != null) guardProp.floatValue = guard;

            SerializedProperty maxDragProp = statusProp.FindPropertyRelative("maxDragDistance");
            if (maxDragProp != null) maxDragProp.floatValue = maxDragDistance;

            SerializedProperty minLaunchProp = statusProp.FindPropertyRelative("minLaunchDistance");
            if (minLaunchProp != null) minLaunchProp.floatValue = minLaunchDistance;

            SerializedProperty maxReflectProp = statusProp.FindPropertyRelative("maxReflectCount");
            if (maxReflectProp != null) maxReflectProp.floatValue = maxReflectCount;

            if (mentalData != null)
            {
                SerializedProperty mentalDataProp = statusProp.FindPropertyRelative("mentalData");
                if (mentalDataProp != null)
                    mentalDataProp.objectReferenceValue = mentalData;
            }
        }
        else
        {
            Debug.LogWarning("'status' property not found in EnemyData!");
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
        mouth = null;
        eyes = null;
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