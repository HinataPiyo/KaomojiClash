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
    
    // 敵設定
    private List<EnemyData> fixedEnemies = new List<EnemyData>();
    
    // 難易度別出現数
    private int easyAmount = 2;
    private int normalAmount = 3;
    private int hardAmount = 2;
    private int extremeAmount = 1;
    
    // 部位解放設定
    private List<PartUnlockConfig> partUnlockConfigs = new List<PartUnlockConfig>();
    private bool showPartUnlockSettings = true;
    
    // MentalDataリスト
    private List<MentalData> mentalDataList = new List<MentalData>();
    private bool showMentalDataSettings = true;
    
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
        window.minSize = new Vector2(650, 800);
    }

    private void OnEnable()
    {
        // 初期化
        UpdateAutoValues();
        InitializePartUnlockConfigs();
        GeneratePreview();
    }
    
    /// <summary>
    /// 部位解放設定の初期化
    /// </summary>
    private void InitializePartUnlockConfigs()
    {
        partUnlockConfigs = new List<PartUnlockConfig>()
        {
            new PartUnlockConfig { unlockCultureLevel = 1, partType = KaomojiPartType.Mouth, description = "口（基本）" },
            new PartUnlockConfig { unlockCultureLevel = 6, partType = KaomojiPartType.Eyes, description = "目" },
            new PartUnlockConfig { unlockCultureLevel = 11, partType = KaomojiPartType.Hands, description = "手" },
            new PartUnlockConfig { unlockCultureLevel = 16, partType = KaomojiPartType.Decoration_First, description = "装飾1" },
            new PartUnlockConfig { unlockCultureLevel = 21, partType = KaomojiPartType.Decoration_Second, description = "装飾2" }
        };
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.LabelField("エリアデータ作成ツール", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);

        // ========== 基本情報 ==========
        DrawBasicInfo();

        // ========== MentalData設定 ==========
        DrawMentalDataSettings();

        // ========== 敵リスト設定 ==========
        DrawEnemyListSettings();

        // ========== 部位解放設定 ==========
        DrawPartUnlockSettings();

        // ========== 難易度別出現数 ==========
        DrawDifficultyAmounts();

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
    /// MentalData設定セクション
    /// </summary>
    private void DrawMentalDataSettings()
    {
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.BeginHorizontal();
        showMentalDataSettings = EditorGUILayout.Foldout(showMentalDataSettings, "🧠 MentalData設定", true, EditorStyles.foldoutHeader);
        
        GUI.backgroundColor = new Color(1f, 0.9f, 0.5f);
        if (GUILayout.Button("全てクリア", GUILayout.Width(100), GUILayout.Height(20)))
        {
            if (EditorUtility.DisplayDialog("確認", "全てのMentalDataをクリアしますか？", "はい", "いいえ"))
            {
                mentalDataList.Clear();
                GeneratePreview();
            }
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
        
        if (showMentalDataSettings)
        {
            EditorGUILayout.HelpBox("敵に使用するMentalData（精神力・顔の輪郭）を設定します。\nリストからランダムに選択されます。", MessageType.Info);
            EditorGUILayout.Space(3);
            
            if (mentalDataList.Count == 0)
            {
                EditorGUILayout.HelpBox("MentalDataが設定されていません。少なくとも1つは設定してください。", MessageType.Warning);
            }
            else
            {
                GUIStyle countStyle = new GUIStyle(EditorStyles.helpBox);
                countStyle.normal.textColor = Color.cyan;
                countStyle.fontStyle = FontStyle.Bold;
                countStyle.alignment = TextAnchor.MiddleCenter;
                
                EditorGUILayout.LabelField($"登録数: {mentalDataList.Count} 個", countStyle);
            }
            
            EditorGUILayout.Space(5);
            
            // MentalDataリスト
            for (int i = 0; i < mentalDataList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal("box");
                
                EditorGUI.BeginChangeCheck();
                
                EditorGUILayout.LabelField($"{i + 1}.", GUILayout.Width(30));
                mentalDataList[i] = (MentalData)EditorGUILayout.ObjectField(mentalDataList[i], typeof(MentalData), false, GUILayout.Width(250));
                
                if (mentalDataList[i] != null)
                {
                    GUIStyle previewStyle = new GUIStyle(EditorStyles.label);
                    previewStyle.normal.textColor = Color.green;
                    previewStyle.fontStyle = FontStyle.Bold;
                    
                    string preview = $"精神力:{mentalDataList[i].maxMental} 括弧:{mentalDataList[i].faceline}";
                    EditorGUILayout.LabelField(preview, previewStyle);
                }
                else
                {
                    EditorGUILayout.LabelField("(未設定)", EditorStyles.miniLabel);
                }
                
                if (EditorGUI.EndChangeCheck())
                {
                    GeneratePreview();
                }
                
                GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                if (GUILayout.Button("✕", GUILayout.Width(25)))
                {
                    mentalDataList.RemoveAt(i);
                    GeneratePreview();
                }
                GUI.backgroundColor = Color.white;
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space(3);
            
            GUI.backgroundColor = new Color(0.7f, 0.7f, 1f);
            if (GUILayout.Button("+ MentalDataを追加"))
            {
                mentalDataList.Add(null);
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(3);
            
            // 統計情報
            if (mentalDataList.Count > 0)
            {
                var validMentals = mentalDataList.Where(m => m != null).ToList();
                if (validMentals.Count > 0)
                {
                    float avgMental = (float)validMentals.Average(m => m.maxMental);
                    var uniqueFacelines = validMentals.Select(m => m.faceline).Distinct().Count();
                    
                    EditorGUILayout.LabelField("【統計】", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"  有効データ: {validMentals.Count} / {mentalDataList.Count}");
                    EditorGUILayout.LabelField($"  平均精神力: {avgMental:F1}");
                    EditorGUILayout.LabelField($"  括弧の種類: {uniqueFacelines} 種類");
                }
            }
        }
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 敵リスト設定セクション
    /// </summary>
    private void DrawEnemyListSettings()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("👾 敵リスト設定", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox("設定された敵リストからランダムに選択されます。\n敵のステータスはEnemyDataで設定してください。", MessageType.Info);
        
        int validEnemyCount = fixedEnemies.Count(e => e != null);
        
        if (validEnemyCount == 0)
        {
            GUIStyle warningStyle = new GUIStyle(EditorStyles.helpBox);
            warningStyle.normal.textColor = Color.red;
            warningStyle.fontStyle = FontStyle.Bold;
            
            EditorGUILayout.LabelField("警告: 敵が1体も設定されていません", warningStyle);
        }
        else
        {
            GUIStyle infoStyle = new GUIStyle(EditorStyles.helpBox);
            infoStyle.normal.textColor = Color.green;
            infoStyle.fontStyle = FontStyle.Bold;
            
            EditorGUILayout.LabelField($"設定済み: {validEnemyCount}体（この中からランダムに選択されます）", infoStyle);
        }
        
        EditorGUILayout.Space(3);
        
        for (int i = 0; i < fixedEnemies.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginChangeCheck();
            fixedEnemies[i] = (EnemyData)EditorGUILayout.ObjectField($"敵 {i + 1}", fixedEnemies[i], typeof(EnemyData), false);
            
            if (EditorGUI.EndChangeCheck())
            {
                GeneratePreview();
            }
            
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
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 部位解放設定セクション
    /// </summary>
    private void DrawPartUnlockSettings()
    {
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.BeginHorizontal();
        showPartUnlockSettings = EditorGUILayout.Foldout(showPartUnlockSettings, "🔓 部位解放設定", true, EditorStyles.foldoutHeader);
        
        GUI.backgroundColor = new Color(0.7f, 0.9f, 1f);
        if (GUILayout.Button("デフォルトに戻す", GUILayout.Width(120), GUILayout.Height(20)))
        {
            InitializePartUnlockConfigs();
            GeneratePreview();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
        
        if (showPartUnlockSettings)
        {
            EditorGUILayout.HelpBox("文化圏レベルに応じて解放される部位を設定します", MessageType.Info);
            EditorGUILayout.Space(3);
            
            var tempManager = new PartUnlockManager { unlockConfigs = partUnlockConfigs };
            var availableTypes = tempManager.GetAvailablePartTypes(cultureLevel);
            
            GUIStyle availableStyle = new GUIStyle(EditorStyles.helpBox);
            availableStyle.normal.textColor = Color.green;
            availableStyle.fontStyle = FontStyle.Bold;
            availableStyle.alignment = TextAnchor.MiddleCenter;
            
            EditorGUILayout.LabelField($"文化圏Lv{cultureLevel}で使用可能: {string.Join(", ", availableTypes)}", availableStyle);
            
            EditorGUILayout.Space(5);
            
            for (int i = 0; i < partUnlockConfigs.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField($"設定 {i + 1}", GUILayout.Width(50));
                partUnlockConfigs[i].unlockCultureLevel = EditorGUILayout.IntSlider("Lv", partUnlockConfigs[i].unlockCultureLevel, 1, 100, GUILayout.Width(200));
                partUnlockConfigs[i].partType = (KaomojiPartType)EditorGUILayout.EnumPopup(partUnlockConfigs[i].partType, GUILayout.Width(150));
                
                if (EditorGUI.EndChangeCheck())
                {
                    GeneratePreview();
                }
                
                GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                if (GUILayout.Button("✕", GUILayout.Width(25)))
                {
                    partUnlockConfigs.RemoveAt(i);
                    GeneratePreview();
                }
                GUI.backgroundColor = Color.white;
                
                EditorGUILayout.EndHorizontal();
                
                partUnlockConfigs[i].description = EditorGUILayout.TextField("説明", partUnlockConfigs[i].description);
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space(3);
            
            GUI.backgroundColor = new Color(0.7f, 0.7f, 1f);
            if (GUILayout.Button("+ 解放設定を追加"))
            {
                partUnlockConfigs.Add(new PartUnlockConfig 
                { 
                    unlockCultureLevel = cultureLevel, 
                    partType = KaomojiPartType.Mouth,
                    description = "" 
                });
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(3);
            
            var nextUnlock = tempManager.GetNextUnlock(cultureLevel);
            if (nextUnlock != null)
            {
                GUIStyle nextUnlockStyle = new GUIStyle(EditorStyles.helpBox);
                nextUnlockStyle.normal.textColor = Color.yellow;
                nextUnlockStyle.fontStyle = FontStyle.Bold;
                
                EditorGUILayout.LabelField($"次の解放: Lv{nextUnlock.unlockCultureLevel} で {nextUnlock.partType} ({nextUnlock.description})", nextUnlockStyle);
            }
            else
            {
                EditorGUILayout.LabelField("全ての部位が解放済み", EditorStyles.miniLabel);
            }
        }
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 難易度別出現数セクション
    /// </summary>
    private void DrawDifficultyAmounts()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("🎲 難易度別出現数", EditorStyles.boldLabel);
        
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
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// プレビューセクション
    /// </summary>
    private void DrawPreview()
    {
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.BeginHorizontal();
        showPreview = EditorGUILayout.Foldout(showPreview, "📊 プレビュー", true, EditorStyles.foldoutHeader);
        
        GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
        if (GUILayout.Button("🎲 再生成", GUILayout.Width(100), GUILayout.Height(20)))
        {
            GeneratePreview();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
        
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
                        try
                        {
                            MentalData mental = entry.enemy.Status?.mentalData;
                            string kaomoji = "";
                            
                            if (mental != null)
                            {
                                kaomoji = entry.enemy.Kaomoji.BuildKaomoji(mental);
                            }
                            else
                            {
                                kaomoji = entry.enemy.Kaomoji.BuildKaomoji(null);
                                if (string.IsNullOrEmpty(kaomoji))
                                {
                                    kaomoji = "(顔文字なし)";
                                }
                            }
                            
                            int avgLevel = AreaBuild.GetEnemyAverageLevelByWaveDifficulty(cultureLevel, entry.difficulty);
                            
                            GUIStyle enemyStyle = new GUIStyle(EditorStyles.helpBox);
                            enemyStyle.normal.textColor = GetDifficultyColor(entry.difficulty);
                            enemyStyle.padding = new RectOffset(5, 5, 3, 3);
                            
                            string displayText = $"[{entry.difficulty}] {kaomoji} Lv{avgLevel}";
                            
                            if (mental == null)
                            {
                                displayText += " ⚠️ MentalDataなし";
                            }
                            
                            EditorGUILayout.LabelField(displayText, enemyStyle);
                            displayCount++;
                        }
                        catch (System.Exception e)
                        {
                            GUIStyle errorStyle = new GUIStyle(EditorStyles.helpBox);
                            errorStyle.normal.textColor = Color.red;
                            EditorGUILayout.LabelField($"[{entry.difficulty}] (表示エラー: {e.Message})", errorStyle);
                            Debug.LogWarning($"Preview display error for {entry.difficulty}: {e.Message}");
                        }
                    }
                    else
                    {
                        GUIStyle nullStyle = new GUIStyle(EditorStyles.helpBox);
                        nullStyle.normal.textColor = Color.gray;
                        EditorGUILayout.LabelField($"[{entry.difficulty}] (生成失敗 - null)", nullStyle);
                    }
                }
                
                if (displayCount == 0)
                {
                    EditorGUILayout.HelpBox("有効な敵が生成されませんでした。敵リストに敵を設定してください。", MessageType.Warning);
                }
            }
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("【予想報酬】", EditorStyles.boldLabel);
            int estimatedExp = EstimateExp();
            int estimatedMoney = EstimateMoney();
            
            EditorGUILayout.LabelField($"・獲得経験値: ~{estimatedExp} Exp");
            EditorGUILayout.LabelField($"・獲得金額: ~{estimatedMoney} G");
            
            int validEnemyCount = previewEnemies.Count(e => e.enemy != null);
            EditorGUILayout.LabelField($"・ドロップ: {validEnemyCount * 1}-{validEnemyCount * 2} 個のパーツ");
            
            EditorGUILayout.Space(5);
            
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
        
        for (int i = 0; i < easyAmount; i++)
        {
            EnemyData enemy = GetRandomEnemy();
            previewEnemies.Add((enemy, Difficulty.Easy));
        }
        
        for (int i = 0; i < normalAmount; i++)
        {
            EnemyData enemy = GetRandomEnemy();
            previewEnemies.Add((enemy, Difficulty.Normal));
        }
        
        for (int i = 0; i < hardAmount; i++)
        {
            EnemyData enemy = GetRandomEnemy();
            previewEnemies.Add((enemy, Difficulty.Hard));
        }
        
        for (int i = 0; i < extremeAmount; i++)
        {
            EnemyData enemy = GetRandomEnemy();
            previewEnemies.Add((enemy, Difficulty.Extreme));
        }
        
        Repaint();
    }

    /// <summary>
    /// 敵リストからランダムに取得
    /// </summary>
    private EnemyData GetRandomEnemy()
    {
        var validFixedEnemies = fixedEnemies.Where(e => e != null).ToList();
        if (validFixedEnemies.Count > 0)
        {
            return validFixedEnemies[Random.Range(0, validFixedEnemies.Count)];
        }
        
        return null;
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
        
        int validEnemyCount = fixedEnemies.Count(e => e != null);
        
        if (validEnemyCount == 0)
        {
            EditorUtility.DisplayDialog("エラー", "少���くとも1体の敵を設定する必要があります", "OK");
            return;
        }
        
        int totalSpawn = easyAmount + normalAmount + hardAmount + extremeAmount;
        if (!EditorUtility.DisplayDialog("確認", 
            $"・設定された敵: {validEnemyCount}体\n" +
            $"・出現数合計: {totalSpawn}体\n\n" +
            $"{validEnemyCount}体の中からランダムに{totalSpawn}体が選ばれます。\n" +
            "よろしいですか？", 
            "作成", "キャンセル"))
        {
            return;
        }
        
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        
        AreaData newArea = CreateInstance<AreaData>();
        SerializedObject serializedArea = new SerializedObject(newArea);
        
        SerializedProperty areaNameProp = serializedArea.FindProperty("areaName");
        if (areaNameProp != null)
        {
            areaNameProp.stringValue = areaName;
        }
        
        SerializedProperty areaProp = serializedArea.FindProperty("area");
        if (areaProp == null)
        {
            Debug.LogError("'area' property not found in AreaData!");
            DestroyImmediate(newArea);
            return;
        }
        
        SerializedProperty cultureLevelProp = areaProp.FindPropertyRelative("cultureLevel");
        if (cultureLevelProp != null)
        {
            cultureLevelProp.intValue = cultureLevel;
        }
        
        SerializedProperty densityProp = areaProp.FindPropertyRelative("kaomojiDensity");
        if (densityProp != null)
        {
            densityProp.floatValue = useAutoKaomojiDensity ? AreaBuild.CalculateKaomojiDensity(cultureLevel) : kaomojiDensity;
        }
        
        SerializedProperty mentalDataListProp = areaProp.FindPropertyRelative("mentalDataList");
        if (mentalDataListProp != null)
        {
            mentalDataListProp.ClearArray();
            for (int i = 0; i < mentalDataList.Count; i++)
            {
                if (mentalDataList[i] != null)
                {
                    mentalDataListProp.InsertArrayElementAtIndex(mentalDataListProp.arraySize);
                    mentalDataListProp.GetArrayElementAtIndex(mentalDataListProp.arraySize - 1).objectReferenceValue = mentalDataList[i];
                }
            }
        }
        
        SerializedProperty partUnlockManagerProp = areaProp.FindPropertyRelative("partUnlockManager");
        if (partUnlockManagerProp != null)
        {
            SerializedProperty unlockConfigsProp = partUnlockManagerProp.FindPropertyRelative("unlockConfigs");
            if (unlockConfigsProp != null)
            {
                unlockConfigsProp.ClearArray();
                for (int i = 0; i < partUnlockConfigs.Count; i++)
                {
                    unlockConfigsProp.InsertArrayElementAtIndex(i);
                    SerializedProperty configProp = unlockConfigsProp.GetArrayElementAtIndex(i);
                    configProp.FindPropertyRelative("unlockCultureLevel").intValue = partUnlockConfigs[i].unlockCultureLevel;
                    configProp.FindPropertyRelative("partType").enumValueIndex = (int)partUnlockConfigs[i].partType;
                    configProp.FindPropertyRelative("description").stringValue = partUnlockConfigs[i].description;
                }
            }
        }
        
        SerializedProperty spawnConfigProp = areaProp.FindPropertyRelative("spawnConfig");
        if (spawnConfigProp == null)
        {
            Debug.LogError("'spawnConfig' property not found in AreaBuild!");
            serializedArea.ApplyModifiedProperties();
            DestroyImmediate(newArea);
            return;
        }
        
        SerializedProperty modeProp = spawnConfigProp.FindPropertyRelative("mode");
        if (modeProp != null)
        {
            modeProp.enumValueIndex = (int)GenerationMode.Manual;
        }
        
        SerializedProperty fixedEnemiesProp = spawnConfigProp.FindPropertyRelative("fixedEnemies");
        if (fixedEnemiesProp != null)
        {
            fixedEnemiesProp.ClearArray();
            int index = 0;
            for (int i = 0; i < fixedEnemies.Count; i++)
            {
                if (fixedEnemies[i] != null)
                {
                    fixedEnemiesProp.InsertArrayElementAtIndex(index);
                    fixedEnemiesProp.GetArrayElementAtIndex(index).objectReferenceValue = fixedEnemies[i];
                    index++;
                }
            }
        }
        
        SerializedProperty spawnAmountsProp = spawnConfigProp.FindPropertyRelative("spawnAmounts");
        if (spawnAmountsProp != null)
        {
            spawnAmountsProp.ClearArray();
            
            spawnAmountsProp.InsertArrayElementAtIndex(0);
            SerializedProperty easy = spawnAmountsProp.GetArrayElementAtIndex(0);
            easy.FindPropertyRelative("difficulty").enumValueIndex = (int)Difficulty.Easy;
            easy.FindPropertyRelative("amount").intValue = easyAmount;
            
            spawnAmountsProp.InsertArrayElementAtIndex(1);
            SerializedProperty normal = spawnAmountsProp.GetArrayElementAtIndex(1);
            normal.FindPropertyRelative("difficulty").enumValueIndex = (int)Difficulty.Normal;
            normal.FindPropertyRelative("amount").intValue = normalAmount;
            
            spawnAmountsProp.InsertArrayElementAtIndex(2);
            SerializedProperty hard = spawnAmountsProp.GetArrayElementAtIndex(2);
            hard.FindPropertyRelative("difficulty").enumValueIndex = (int)Difficulty.Hard;
            hard.FindPropertyRelative("amount").intValue = hardAmount;
            
            spawnAmountsProp.InsertArrayElementAtIndex(3);
            SerializedProperty extreme = spawnAmountsProp.GetArrayElementAtIndex(3);
            extreme.FindPropertyRelative("difficulty").enumValueIndex = (int)Difficulty.Extreme;
            extreme.FindPropertyRelative("amount").intValue = extremeAmount;
        }
        
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
        fixedEnemies.Clear();
        mentalDataList.Clear();
        useAutoKaomojiDensity = true;
        InitializePartUnlockConfigs();
        UpdateAutoValues();
        GeneratePreview();
    }
}