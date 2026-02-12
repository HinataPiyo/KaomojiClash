using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Constants;
using ENUM;

public class AreaDataCreator : EditorWindow
{
    // 基本情報
    private string areaName = "";
    private int cultureLevel = 1;
    private string fileName = "";
    
    // 敵出現設定
    private GenerationMode generationMode = GenerationMode.SemiAuto;
    private List<EnemyData> fixedEnemies = new List<EnemyData>();
    private bool[] excludeTypes = new bool[5]; // KaomojiPartTypeの数
    
    // 難易度別出現数
    private int easyAmount = 2;
    private int normalAmount = 3;
    private int hardAmount = 2;
    private int extremeAmount = 1;
    
    // 詳細設定
    private float kaomojiDensity = 0.5f;
    private bool useAutoKaomojiDensity = true;
    private string savePath = "Assets/Resources/Areas";
    
    // プレビュー
    private Vector2 scrollPosition;
    private bool showPreview = true;
    private bool showAdvanced = false;
    
    // 生成された敵プレビュー
    private List<(EnemyData enemy, Difficulty difficulty)> previewEnemies = new List<(EnemyData, Difficulty)>();

    [MenuItem("Tools/Area/エリアデータ作成ツール")]
    public static void ShowWindow()
    {
        var window = GetWindow<AreaDataCreator>("エリアデータ作成");
        window.minSize = new Vector2(650, 750);
    }

    private void OnEnable()
    {
        // 初期化
        UpdateAutoValues();
        GeneratePreview();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.LabelField("エリアデータ作成ツール", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);

        // ========== 基本情報 ==========
        DrawBasicInfo();

        // ========== 敵出現設定 ==========
        DrawEnemySpawnSettings();

        // ========== プレビュー ==========
        DrawPreview();

        // ========== 詳細設定 ==========
        DrawAdvancedSettings();

        EditorGUILayout.Space(10);

        // ========== アクションボタン ==========
        DrawActionButtons();

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 基本情報セクション
    /// </summary>
    private void DrawBasicInfo()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("📍 基本情報", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        // 左列
        EditorGUILayout.BeginVertical(GUILayout.Width(300));
        areaName = EditorGUILayout.TextField("エリア名", areaName);
        
        EditorGUI.BeginChangeCheck();
        cultureLevel = EditorGUILayout.IntSlider("文化圏レベル", cultureLevel, 1, 100);
        if (EditorGUI.EndChangeCheck())
        {
            // 文化圏レベル変更時に自動計算
            UpdateAutoValues();
            GeneratePreview();
        }
        
        EditorGUILayout.EndVertical();
        
        // 右列
        EditorGUILayout.BeginVertical();
        
        GUIStyle infoStyle = new GUIStyle(EditorStyles.helpBox);
        infoStyle.normal.textColor = Color.cyan;
        infoStyle.fontStyle = FontStyle.Bold;
        infoStyle.alignment = TextAnchor.MiddleLeft;
        infoStyle.padding = new RectOffset(8, 8, 4, 4);
        
        EditorGUILayout.LabelField("保存ファイル名", EditorStyles.miniLabel);
        fileName = $"Area_Lv{cultureLevel:D2}";
        EditorGUILayout.LabelField($"{fileName}.asset", infoStyle, GUILayout.Height(18));
        
        EditorGUILayout.LabelField("敵平均レベル", EditorStyles.miniLabel);
        int avgLevel = AreaBuild.GetEnemyAverageLevel(cultureLevel);
        EditorGUILayout.LabelField($"Lv {avgLevel}", infoStyle, GUILayout.Height(18));
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 敵出現設定セクション
    /// </summary>
    private void DrawEnemySpawnSettings()
    {
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("🎲 敵出現設定", EditorStyles.boldLabel);
        
        GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
        if (GUILayout.Button("🎲 再生成", GUILayout.Width(100), GUILayout.Height(20)))
        {
            GeneratePreview();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        // 生成モード選択
        EditorGUILayout.LabelField("生成モード", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        
        generationMode = (GenerationMode)EditorGUILayout.EnumPopup(generationMode);
        
        if (EditorGUI.EndChangeCheck())
        {
            GeneratePreview();
        }
        
        EditorGUILayout.Space(5);
        
        // モード別設定
        switch (generationMode)
        {
            case GenerationMode.FullAuto:
                EditorGUILayout.HelpBox("完全自動モード: 文化圏レベルに基づいて全て自動生成されます", MessageType.Info);
                break;
                
            case GenerationMode.SemiAuto:
                DrawSemiAutoSettings();
                break;
                
            case GenerationMode.Manual:
                DrawManualSettings();
                break;
        }
        
        EditorGUILayout.Space(5);
        
        // 難易度別出現数
        DrawDifficultyAmounts();
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 半自動モード設定
    /// </summary>
    private void DrawSemiAutoSettings()
    {
        EditorGUILayout.LabelField("【半自動モード設定】", EditorStyles.boldLabel);
        
        // 固定敵リスト
        EditorGUILayout.LabelField("固定敵リスト（オプション）", EditorStyles.miniLabel);
        
        for (int i = 0; i < fixedEnemies.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            fixedEnemies[i] = (EnemyData)EditorGUILayout.ObjectField($"敵 {i + 1}", fixedEnemies[i], typeof(EnemyData), false);
            
            GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("✕", GUILayout.Width(25)))
            {
                fixedEnemies.RemoveAt(i);
                GeneratePreview();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
        }
        
        GUI.backgroundColor = new Color(0.7f, 0.7f, 1f);
        if (GUILayout.Button("+ 固定敵を追加"))
        {
            fixedEnemies.Add(null);
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space(5);
        
        // 除外タイプ
        EditorGUILayout.LabelField("除外する部位タイプ", EditorStyles.miniLabel);
        EditorGUILayout.BeginHorizontal();
        
        EditorGUI.BeginChangeCheck();
        excludeTypes[0] = EditorGUILayout.ToggleLeft("口", excludeTypes[0], GUILayout.Width(60));
        excludeTypes[1] = EditorGUILayout.ToggleLeft("目", excludeTypes[1], GUILayout.Width(60));
        excludeTypes[2] = EditorGUILayout.ToggleLeft("手", excludeTypes[2], GUILayout.Width(60));
        excludeTypes[3] = EditorGUILayout.ToggleLeft("装飾1", excludeTypes[3], GUILayout.Width(70));
        excludeTypes[4] = EditorGUILayout.ToggleLeft("装飾2", excludeTypes[4], GUILayout.Width(70));
        
        if (EditorGUI.EndChangeCheck())
        {
            GeneratePreview();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 手動モード設定
    /// </summary>
    private void DrawManualSettings()
    {
        EditorGUILayout.HelpBox("手動モード: 全ての敵を手動で指定してください", MessageType.Warning);
        
        // 手動モード用の固定敵リスト
        EditorGUILayout.LabelField("敵リスト", EditorStyles.boldLabel);
        
        for (int i = 0; i < fixedEnemies.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            fixedEnemies[i] = (EnemyData)EditorGUILayout.ObjectField($"敵 {i + 1}", fixedEnemies[i], typeof(EnemyData), false);
            
            GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("✕", GUILayout.Width(25)))
            {
                fixedEnemies.RemoveAt(i);
                GeneratePreview();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
        }
        
        GUI.backgroundColor = new Color(0.7f, 0.7f, 1f);
        if (GUILayout.Button("+ 敵を追加"))
        {
            fixedEnemies.Add(null);
        }
        GUI.backgroundColor = Color.white;
    }

    /// <summary>
    /// 難易度別出現数
    /// </summary>
    private void DrawDifficultyAmounts()
    {
        EditorGUILayout.LabelField("【難易度別出現数】", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Easy:", GUILayout.Width(80));
        easyAmount = EditorGUILayout.IntSlider(easyAmount, 0, 10);
        EditorGUILayout.LabelField("体", GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Normal:", GUILayout.Width(80));
        normalAmount = EditorGUILayout.IntSlider(normalAmount, 0, 10);
        EditorGUILayout.LabelField("体", GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Hard:", GUILayout.Width(80));
        hardAmount = EditorGUILayout.IntSlider(hardAmount, 0, 10);
        EditorGUILayout.LabelField("体", GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Extreme:", GUILayout.Width(80));
        extremeAmount = EditorGUILayout.IntSlider(extremeAmount, 0, 10);
        EditorGUILayout.LabelField("体", GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck())
        {
            GeneratePreview();
        }
        
        int total = easyAmount + normalAmount + hardAmount + extremeAmount;
        EditorGUILayout.LabelField($"合計: {total} 体", EditorStyles.boldLabel);
    }

    /// <summary>
    /// プレビューセクション
    /// </summary>
    private void DrawPreview()
    {
        EditorGUILayout.BeginVertical("box");
        showPreview = EditorGUILayout.Foldout(showPreview, "📊 プレビュー", true, EditorStyles.foldoutHeader);
        
        if (showPreview)
        {
            EditorGUILayout.LabelField("出現敵一覧", EditorStyles.boldLabel);
            
            if (previewEnemies.Count == 0)
            {
                EditorGUILayout.HelpBox("敵が生成されていません。「再生成」ボタンをクリックしてください。", MessageType.Info);
            }
            else
            {
                int displayCount = 0;
                foreach (var entry in previewEnemies)
                {
                    if (entry.enemy != null)
                    {
                        string kaomoji = entry.enemy.Kaomoji.BuildKaomoji(entry.enemy.Status.mentalData);
                        int avgLevel = AreaBuild.GetEnemyAverageLevelByWaveDifficulty(cultureLevel, entry.difficulty);
                        
                        GUIStyle enemyStyle = new GUIStyle(EditorStyles.helpBox);
                        enemyStyle.normal.textColor = GetDifficultyColor(entry.difficulty);
                        enemyStyle.padding = new RectOffset(5, 5, 3, 3);
                        
                        EditorGUILayout.LabelField($"[{entry.difficulty}] {kaomoji} Lv{avgLevel}", enemyStyle);
                        displayCount++;
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"[{entry.difficulty}] (生成失敗)", EditorStyles.helpBox);
                    }
                }
                
                if (displayCount == 0)
                {
                    EditorGUILayout.HelpBox("有効な敵が生成されませんでした。KaomojiPartsフォルダにパーツが存在するか確認してください。", MessageType.Warning);
                }
            }
            
            EditorGUILayout.Space(5);
            
            // 予想報酬
            EditorGUILayout.LabelField("【予想報酬】", EditorStyles.boldLabel);
            int estimatedExp = EstimateExp();
            int estimatedMoney = EstimateMoney();
            
            EditorGUILayout.LabelField($"・獲得経験値: ~{estimatedExp} Exp");
            EditorGUILayout.LabelField($"・獲得金額: ~{estimatedMoney} G");
            EditorGUILayout.LabelField($"・ドロップ: {previewEnemies.Count * 1}-{previewEnemies.Count * 2} 個のパーツ");
            
            EditorGUILayout.Space(5);
            
            // 推定難易度
            int difficulty = Mathf.Clamp(cultureLevel / 20 + 1, 1, 5);
            string stars = new string('★', difficulty) + new string('☆', 5 - difficulty);
            EditorGUILayout.LabelField($"【推定難易度】{stars}");
        }
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 詳細設定セクション
    /// </summary>
    private void DrawAdvancedSettings()
    {
        EditorGUILayout.BeginVertical("box");
        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "⚙ 詳細設定", true, EditorStyles.foldoutHeader);
        
        if (showAdvanced)
        {
            EditorGUILayout.BeginHorizontal();
            useAutoKaomojiDensity = EditorGUILayout.Toggle("顔文字密度を自動計算", useAutoKaomojiDensity);
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.BeginDisabledGroup(useAutoKaomojiDensity);
            kaomojiDensity = EditorGUILayout.Slider("顔文字密度", kaomojiDensity, 0.1f, 1f);
            EditorGUI.EndDisabledGroup();
            
            if (useAutoKaomojiDensity)
            {
                float autoDensity = AreaBuild.CalculateKaomojiDensity(cultureLevel);
                EditorGUILayout.LabelField($"自動計算値: {autoDensity:F2}", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            savePath = EditorGUILayout.TextField("保存先", savePath);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string path = EditorUtility.OpenFolderPanel("保存先を選択", "Assets", "");
                if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
                {
                    savePath = "Assets" + path.Substring(Application.dataPath.Length);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// アクションボタン
    /// </summary>
    private void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("✓ 作成", GUILayout.Height(40)))
        {
            CreateAreaData();
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
        if (GUILayout.Button("🎲 再生成", GUILayout.Height(40), GUILayout.Width(100)))
        {
            GeneratePreview();
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = new Color(1f, 0.8f, 0.6f);
        if (GUILayout.Button("↻ リセット", GUILayout.Height(40), GUILayout.Width(100)))
        {
            if (EditorUtility.DisplayDialog("確認", "入力をリセットしますか？", "はい", "いいえ"))
            {
                ResetFields();
            }
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 自動値を更新
    /// </summary>
    private void UpdateAutoValues()
    {
        if (useAutoKaomojiDensity)
        {
            kaomojiDensity = AreaBuild.CalculateKaomojiDensity(cultureLevel);
        }
        
        // 文化圏レベルに応じて難易度別出現数を自動調整
        int baseCount = Mathf.CeilToInt(cultureLevel / 5f);
        easyAmount = baseCount + 2;
        normalAmount = baseCount + 3;
        hardAmount = baseCount + 2;
        extremeAmount = baseCount + 1;
    }

    /// <summary>
    /// プレビュー生成
    /// </summary>
    private void GeneratePreview()
    {
        previewEnemies.Clear();
        
        // Easy
        for (int i = 0; i < easyAmount; i++)
        {
            EnemyData enemy = GenerateRandomEnemy(Difficulty.Easy);
            previewEnemies.Add((enemy, Difficulty.Easy));
        }
        
        // Normal
        for (int i = 0; i < normalAmount; i++)
        {
            EnemyData enemy = GenerateRandomEnemy(Difficulty.Normal);
            previewEnemies.Add((enemy, Difficulty.Normal));
        }
        
        // Hard
        for (int i = 0; i < hardAmount; i++)
        {
            EnemyData enemy = GenerateRandomEnemy(Difficulty.Hard);
            previewEnemies.Add((enemy, Difficulty.Hard));
        }
        
        // Extreme
        for (int i = 0; i < extremeAmount; i++)
        {
            EnemyData enemy = GenerateRandomEnemy(Difficulty.Extreme);
            previewEnemies.Add((enemy, Difficulty.Extreme));
        }
        
        Repaint();
    }

    /// <summary>
    /// ランダムに敵を生成（プレビュー用）
    /// </summary>
    private EnemyData GenerateRandomEnemy(Difficulty difficulty)
    {
        List<KaomojiPartType> excludeList = new List<KaomojiPartType>();
        for (int i = 0; i < excludeTypes.Length; i++)
        {
            if (excludeTypes[i])
            {
                excludeList.Add((KaomojiPartType)i);
            }
        }

        // 生成モードに応じた処理
        switch (generationMode)
        {
            case GenerationMode.FullAuto:
                return EnemyDataGenerator.GenerateRandomEnemy(cultureLevel, difficulty, null);
                
            case GenerationMode.SemiAuto:
                // 固定敵がある場合は優先
                if (fixedEnemies.Count > 0)
                {
                    var validFixed = fixedEnemies.Where(e => e != null).ToList();
                    if (validFixed.Count > 0 && Random.value < 0.3f) // 30%の確率で固定敵を使用
                    {
                        return validFixed[Random.Range(0, validFixed.Count)];
                    }
                }
                return EnemyDataGenerator.GenerateRandomEnemy(cultureLevel, difficulty, excludeList);
                
            case GenerationMode.Manual:
                // 手動モードでは固定敵のみ
                if (fixedEnemies.Count > 0)
                {
                    var validFixed = fixedEnemies.Where(e => e != null).ToList();
                    if (validFixed.Count > 0)
                    {
                        return validFixed[Random.Range(0, validFixed.Count)];
                    }
                }
                return null;
                
            default:
                return null;
        }
    }

    /// <summary>
    /// 経験値の推定
    /// </summary>
    private int EstimateExp()
    {
        return cultureLevel * 100 + previewEnemies.Count * 50;
    }

    /// <summary>
    /// お金の推定
    /// </summary>
    private int EstimateMoney()
    {
        return cultureLevel * 30 + previewEnemies.Count * 20;
    }

    /// <summary>
    /// 難易度の色を取得
    /// </summary>
    private Color GetDifficultyColor(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy: return Color.green;
            case Difficulty.Normal: return Color.yellow;
            case Difficulty.Hard: return new Color(1f, 0.5f, 0f);
            case Difficulty.Extreme: return Color.red;
            default: return Color.white;
        }
    }

    /// <summary>
    /// AreaData作成
    /// </summary>
    private void CreateAreaData()
    {
        if (string.IsNullOrEmpty(areaName))
        {
            EditorUtility.DisplayDialog("エラー", "エリア名を入力してください", "OK");
            return;
        }
        
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        
        AreaData newArea = CreateInstance<AreaData>();
        
        // SerializedObjectを使用して安全に設定
        SerializedObject serializedArea = new SerializedObject(newArea);
        
        // エリア名設定
        SerializedProperty areaNameProp = serializedArea.FindProperty("areaName");
        if (areaNameProp != null)
        {
            areaNameProp.stringValue = areaName;
        }
        
        // AreaBuild設定
        SerializedProperty areaProp = serializedArea.FindProperty("area");
        if (areaProp == null)
        {
            Debug.LogError("'area' property not found in AreaData!");
            DestroyImmediate(newArea);
            return;
        }
        
        // 文化圏レベル設定
        SerializedProperty cultureLevelProp = areaProp.FindPropertyRelative("cultureLevel");
        if (cultureLevelProp != null)
        {
            cultureLevelProp.intValue = cultureLevel;
        }
        
        // 顔文字密度設定
        SerializedProperty densityProp = areaProp.FindPropertyRelative("kaomojiDensity");
        if (densityProp != null)
        {
            densityProp.floatValue = useAutoKaomojiDensity ? AreaBuild.CalculateKaomojiDensity(cultureLevel) : kaomojiDensity;
        }
        
        // EnemySpawnConfig設定
        SerializedProperty spawnConfigProp = areaProp.FindPropertyRelative("spawnConfig");
        if (spawnConfigProp == null)
        {
            Debug.LogError("'spawnConfig' property not found in AreaBuild!");
            serializedArea.ApplyModifiedProperties();
            DestroyImmediate(newArea);
            return;
        }
        
        // 生成モード
        SerializedProperty modeProp = spawnConfigProp.FindPropertyRelative("mode");
        if (modeProp != null)
        {
            modeProp.enumValueIndex = (int)generationMode;
        }
        
        // 固定敵リスト
        SerializedProperty fixedEnemiesProp = spawnConfigProp.FindPropertyRelative("fixedEnemies");
        if (fixedEnemiesProp != null)
        {
            fixedEnemiesProp.ClearArray();
            for (int i = 0; i < fixedEnemies.Count; i++)
            {
                if (fixedEnemies[i] != null)
                {
                    fixedEnemiesProp.InsertArrayElementAtIndex(i);
                    fixedEnemiesProp.GetArrayElementAtIndex(i).objectReferenceValue = fixedEnemies[i];
                }
            }
        }
        
        // 除外タイプ設定
        SerializedProperty excludeTypesProp = spawnConfigProp.FindPropertyRelative("excludeTypes");
        if (excludeTypesProp != null)
        {
            excludeTypesProp.ClearArray();
            int excludeIndex = 0;
            for (int i = 0; i < excludeTypes.Length; i++)
            {
                if (excludeTypes[i])
                {
                    excludeTypesProp.InsertArrayElementAtIndex(excludeIndex);
                    excludeTypesProp.GetArrayElementAtIndex(excludeIndex).enumValueIndex = i;
                    excludeIndex++;
                }
            }
        }
        
        // 難易度別出現数
        SerializedProperty spawnAmountsProp = spawnConfigProp.FindPropertyRelative("spawnAmounts");
        if (spawnAmountsProp != null)
        {
            spawnAmountsProp.ClearArray();
            
            // Easy
            spawnAmountsProp.InsertArrayElementAtIndex(0);
            SerializedProperty easy = spawnAmountsProp.GetArrayElementAtIndex(0);
            easy.FindPropertyRelative("difficulty").enumValueIndex = (int)Difficulty.Easy;
            easy.FindPropertyRelative("amount").intValue = easyAmount;
            
            // Normal
            spawnAmountsProp.InsertArrayElementAtIndex(1);
            SerializedProperty normal = spawnAmountsProp.GetArrayElementAtIndex(1);
            normal.FindPropertyRelative("difficulty").enumValueIndex = (int)Difficulty.Normal;
            normal.FindPropertyRelative("amount").intValue = normalAmount;
            
            // Hard
            spawnAmountsProp.InsertArrayElementAtIndex(2);
            SerializedProperty hard = spawnAmountsProp.GetArrayElementAtIndex(2);
            hard.FindPropertyRelative("difficulty").enumValueIndex = (int)Difficulty.Hard;
            hard.FindPropertyRelative("amount").intValue = hardAmount;
            
            // Extreme
            spawnAmountsProp.InsertArrayElementAtIndex(3);
            SerializedProperty extreme = spawnAmountsProp.GetArrayElementAtIndex(3);
            extreme.FindPropertyRelative("difficulty").enumValueIndex = (int)Difficulty.Extreme;
            extreme.FindPropertyRelative("amount").intValue = extremeAmount;
        }
        
        // 変更を適用
        serializedArea.ApplyModifiedProperties();
        
        string assetPath = $"{savePath}/{fileName}.asset";
        
        if (AssetDatabase.LoadAssetAtPath<AreaData>(assetPath) != null)
        {
            if (!EditorUtility.DisplayDialog("警告", $"'{fileName}.asset' を上書きしますか？", "上書き", "キャンセル"))
            {
                DestroyImmediate(newArea);
                return;
            }
        }
        
        AssetDatabase.CreateAsset(newArea, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("成功", $"エリアデータを作成しました！\n{fileName}.asset", "OK");
        
        Selection.activeObject = newArea;
        EditorGUIUtility.PingObject(newArea);
    }

    /// <summary>
    /// フィールドリセット
    /// </summary>
    private void ResetFields()
    {
        areaName = "";
        cultureLevel = 1;
        generationMode = GenerationMode.SemiAuto;
        fixedEnemies.Clear();
        excludeTypes = new bool[5];
        useAutoKaomojiDensity = true;
        UpdateAutoValues();
        GeneratePreview();
    }
}