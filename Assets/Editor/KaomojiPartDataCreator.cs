using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Constants;
using ENUM;
using Constants.Global;

public class KaomojiPartDataCreator : EditorWindow
{
    // 基本情報
    private string fileName = "";
    private string partName = "";
    private string partSymbol = "";
    private KaomojiPartType partType = KaomojiPartType.Eyes;
    private float dropProbability = 0.3f;

    // ステータス設定（自動生成）
    private float speedValue = 0f;
    private GrowthRateType speedGrowth = GrowthRateType.Normal;

    private float powerValue = 0f;
    private GrowthRateType powerGrowth = GrowthRateType.Normal;

    private float guardValue = 0f;
    private GrowthRateType guardGrowth = GrowthRateType.Normal;

    private float staminaValue = 0f;
    private GrowthRateType staminaGrowth = GrowthRateType.Normal;

    // その他設定
    private int maxDup = 50;
    private bool isInitDisplay = false;
    private GrowthRateType levelGrowth = GrowthRateType.Normal;

    private string savePath = "Assets/Resources/KaomojiParts";
    private Vector2 scrollPosition;

    // Foldout状態
    private bool showPreview = true;
    private bool showAdvanced = false;

    // 成長率制限ルール定義
    private static readonly Dictionary<KaomojiPartType, GrowthRateConstraint> GrowthRateRules = new Dictionary<KaomojiPartType, GrowthRateConstraint>
    {
        {
            KaomojiPartType.Mouth, new GrowthRateConstraint
            {
                AllowedCounts = new Dictionary<GrowthRateType, int>
                {
                    { GrowthRateType.VeryHigh, 0 },
                    { GrowthRateType.High, 1 },
                    { GrowthRateType.Normal, 2 },
                    { GrowthRateType.Low, 1 },
                    { GrowthRateType.VeryLow, 0 }
                }
            }
        },
        {
            KaomojiPartType.Eyes, new GrowthRateConstraint
            {
                AllowedCounts = new Dictionary<GrowthRateType, int>
                {
                    { GrowthRateType.VeryHigh, 1 },
                    { GrowthRateType.High, 1 },
                    { GrowthRateType.Normal, 0 },
                    { GrowthRateType.Low, 2 },
                    { GrowthRateType.VeryLow, 0 }
                }
            }
        },
        {
            KaomojiPartType.Hands, new GrowthRateConstraint
            {
                AllowedCounts = new Dictionary<GrowthRateType, int>
                {
                    { GrowthRateType.VeryHigh, 1 },
                    { GrowthRateType.High, 1 },
                    { GrowthRateType.Normal, 1 },
                    { GrowthRateType.Low, 1 },
                    { GrowthRateType.VeryLow, 0 }
                }
            }
        },
        {
            KaomojiPartType.Decoration_First, new GrowthRateConstraint
            {
                AllowedCounts = new Dictionary<GrowthRateType, int>
                {
                    { GrowthRateType.VeryHigh, 2 },
                    { GrowthRateType.High, 0 },
                    { GrowthRateType.Normal, 0 },
                    { GrowthRateType.Low, 0 },
                    { GrowthRateType.VeryLow, 2 }
                }
            }
        },
        {
            KaomojiPartType.Decoration_Second, new GrowthRateConstraint
            {
                AllowedCounts = new Dictionary<GrowthRateType, int>
                {
                    { GrowthRateType.VeryHigh, 2 },
                    { GrowthRateType.High, 0 },
                    { GrowthRateType.Normal, 0 },
                    { GrowthRateType.Low, 0 },
                    { GrowthRateType.VeryLow, 2 }
                }
            }
        }
    };

    // 成長率制限クラス
    private class GrowthRateConstraint
    {
        public Dictionary<GrowthRateType, int> AllowedCounts;

        /// <summary>
        /// ランダムに成長率を割り当て
        /// </summary>
        public List<GrowthRateType> GenerateRandomGrowthRates()
        {
            var result = new List<GrowthRateType>();

            foreach (var kvp in AllowedCounts)
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    result.Add(kvp.Key);
                }
            }

            // シャッフル
            for (int i = result.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = result[i];
                result[i] = result[j];
                result[j] = temp;
            }

            return result;
        }
    }

    [MenuItem("Tools/Kaomoji/パーツデータ作成ツール")]
    public static void ShowWindow()
    {
        var window = GetWindow<KaomojiPartDataCreator>("顔文字パーツ作成");
        window.minSize = new Vector2(600, 680);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("顔文字パーツデータ作成", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);

        // ========== 基本情報（コンパクト版） ==========
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("基本情報", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        // 左列
        EditorGUILayout.BeginVertical(GUILayout.Width(280));

        KaomojiPartType previousType = partType;
        partType = (KaomojiPartType)EditorGUILayout.EnumPopup("部位", partType);
        if (previousType != partType)
        {
            RandomizeAll(); // 部位変更時に全てランダム化
        }

        fileName = EditorGUILayout.TextField("ファイル名", fileName);
        partName = EditorGUILayout.TextField("パーツ名", partName);

        EditorGUILayout.EndVertical();

        // 右列
        EditorGUILayout.BeginVertical();

        GUIStyle prefixStyle = new GUIStyle(EditorStyles.helpBox);
        prefixStyle.normal.textColor = Color.cyan;
        prefixStyle.fontStyle = FontStyle.Bold;
        prefixStyle.padding = new RectOffset(8, 8, 4, 4);

        EditorGUILayout.LabelField("保存名", EditorStyles.miniLabel);
        EditorGUILayout.LabelField($"{GetPrefix(partType)}{fileName}.asset", prefixStyle, GUILayout.Height(18));

        EditorGUILayout.LabelField("倍率", EditorStyles.miniLabel);
        EditorGUILayout.LabelField($"{Calculation.GetPartTypeMultiplier(partType) * 100}%", prefixStyle, GUILayout.Height(18));

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(3);

        // 記号入力と大きなプレビュー
        EditorGUILayout.BeginHorizontal();
        partSymbol = EditorGUILayout.TextField("記号", partSymbol, GUILayout.Width(280));

        if (!string.IsNullOrEmpty(partSymbol))
        {
            GUIStyle symbolStyle = new GUIStyle(EditorStyles.helpBox);
            symbolStyle.fontSize = 24;
            symbolStyle.alignment = TextAnchor.MiddleCenter;
            symbolStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(partSymbol, symbolStyle, GUILayout.Height(26), GUILayout.ExpandWidth(true));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        // ========== 成長率情報（表示のみ） ==========
        DrawGrowthRateInfo();

        // ========== ステータス表示（表示のみ） ==========
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("⚖ ステータス（自動生成）", EditorStyles.boldLabel);

        // 再生成ボタン
        GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
        if (GUILayout.Button("🎲 全て再生成", GUILayout.Width(120), GUILayout.Height(20)))
        {
            RandomizeAll();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();

        // バジェット情報
        float budget = GetStatBudget();
        float currentTotal = GetCurrentTotalNormalized();
        Rect progressRect = EditorGUILayout.GetControlRect(false, 16);
        float percentage = currentTotal / budget;
        Color barColor = percentage > 1f ? Color.red : Color.green;
        EditorGUI.DrawRect(progressRect, new Color(0.2f, 0.2f, 0.2f));
        Rect fillRect = new Rect(progressRect.x, progressRect.y, progressRect.width * Mathf.Clamp01(percentage), progressRect.height);
        EditorGUI.DrawRect(fillRect, barColor);

        GUIStyle centerStyle = new GUIStyle(EditorStyles.miniLabel);
        centerStyle.alignment = TextAnchor.MiddleCenter;
        centerStyle.normal.textColor = Color.white;
        GUI.Label(progressRect, $"使用: {currentTotal:F2} / {budget:F2} ({percentage * 100:F0}%)", centerStyle);

        EditorGUILayout.Space(3);

        // Speed & Power (上段)
        EditorGUILayout.BeginHorizontal();
        DrawStatDisplay("Speed", speedValue, speedGrowth, new Color(0.3f, 0.9f, 0.9f));
        EditorGUILayout.Space(5);
        DrawStatDisplay("Power", powerValue, powerGrowth, new Color(1f, 0.4f, 0.4f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(3);

        // Guard & Stamina (下段)
        EditorGUILayout.BeginHorizontal();
        DrawStatDisplay("Guard", guardValue, guardGrowth, new Color(0.4f, 0.7f, 1f));
        EditorGUILayout.Space(5);
        DrawStatDisplay("Stamina", staminaValue, staminaGrowth, new Color(1f, 0.9f, 0.3f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        // ========== レベル成長プレビュー（折りたたみ） ==========
        EditorGUILayout.BeginVertical("box");
        showPreview = EditorGUILayout.Foldout(showPreview, "📊 レベル成長プレビュー", true, EditorStyles.foldoutHeader);

        if (showPreview)
        {
            DrawCompactLevelPreview();
        }
        EditorGUILayout.EndVertical();

        // ========== 詳細設定（折りたたみ） ==========
        EditorGUILayout.BeginVertical("box");
        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "⚙ 詳細設定", true, EditorStyles.foldoutHeader);

        if (showAdvanced)
        {
            EditorGUILayout.BeginHorizontal();

            // 左列
            EditorGUILayout.BeginVertical(GUILayout.Width(280));
            dropProbability = EditorGUILayout.Slider("ドロップ率", dropProbability, 0.01f, 0.8f);
            EditorGUILayout.LabelField($"{dropProbability * 100:F1}%", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();

            // 右列
            EditorGUILayout.BeginVertical();
            maxDup = EditorGUILayout.IntSlider("最大重複数", maxDup, 1, 999);
            isInitDisplay = EditorGUILayout.Toggle("初期表示", isInitDisplay);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("経験値成長", GUILayout.Width(100));
            levelGrowth = (GrowthRateType)EditorGUILayout.EnumPopup(levelGrowth);
            EditorGUILayout.LabelField(Calculation.GetGrowthRateStar(levelGrowth), GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

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

        EditorGUILayout.Space(5);

        // ========== アクションボタン ==========
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("✓ 作成", GUILayout.Height(40)))
        {
            CreateKaomojiPartData();
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

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// ステータス値と成長率を全てランダム生成
    /// </summary>
    private void RandomizeAll()
    {
        RandomizeGrowthRates();
        RandomizeStatValues();
    }

    /// <summary>
    /// 成長率をランダム割り当て
    /// </summary>
    private void RandomizeGrowthRates()
    {
        if (!GrowthRateRules.ContainsKey(partType)) return;

        var constraint = GrowthRateRules[partType];
        var randomRates = constraint.GenerateRandomGrowthRates();

        if (randomRates.Count >= 4)
        {
            speedGrowth = randomRates[0];
            powerGrowth = randomRates[1];
            guardGrowth = randomRates[2];
            staminaGrowth = randomRates[3];
        }
    }

    /// <summary>
    /// ステータス値をランダム生成（プラス値優先、バランス調整付き）
    /// </summary>
    private void RandomizeStatValues()
    {
        float multiplier = Calculation.GetPartTypeMultiplier(partType);

        // 各ステータスの範囲
        float speedMin = KaomojiPart.Speed.MIN_VALUE * multiplier;
        float speedMax = KaomojiPart.Speed.MAX_VALUE * multiplier;

        float powerMin = KaomojiPart.Power.MIN_VALUE * multiplier;
        float powerMax = KaomojiPart.Power.MAX_VALUE * multiplier;

        float guardMin = KaomojiPart.Guard.MIN_VALUE * multiplier;
        float guardMax = KaomojiPart.Guard.MAX_VALUE * multiplier;

        float staminaMin = KaomojiPart.Stamina.MIN_VALUE * multiplier;
        float staminaMax = KaomojiPart.Stamina.MAX_VALUE * multiplier;

        float budget = GetStatBudget();
        int maxAttempts = 100;
        float bestDiff = float.MaxValue;
        float bestSpeed = 0, bestPower = 0, bestGuard = 0, bestStamina = 0;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // ステップ1: 全てをプラス値で生成（70%の確率範囲）
            float tempSpeed = Random.Range(0f, speedMax * 0.7f);
            float tempPower = Random.Range(0f, powerMax * 0.7f);
            float tempGuard = Random.Range(0f, guardMax * 0.7f);
            float tempStamina = Random.Range(0f, staminaMax * 0.7f);

            // ステップ2: 合計値を計算
            float total =
                Mathf.Abs(NormalizeValue(tempSpeed, speedMin, speedMax)) +
                Mathf.Abs(NormalizeValue(tempPower, powerMin, powerMax)) +
                Mathf.Abs(NormalizeValue(tempGuard, guardMin, guardMax)) +
                Mathf.Abs(NormalizeValue(tempStamina, staminaMin, staminaMax));

            // ステップ3: 予算オーバーの場合のみマイナス値を導入
            if (total > budget)
            {
                float excess = total - budget;

                // 最も高い値を持つステータスをマイナスにする候補を選ぶ
                var stats = new List<(string name, float value, float min, float max)>
            {
                ("speed", tempSpeed, speedMin, speedMax),
                ("power", tempPower, powerMin, powerMax),
                ("guard", tempGuard, guardMin, guardMax),
                ("stamina", tempStamina, staminaMin, staminaMax)
            };

                // 正規化値でソート（高い順）
                stats = stats.OrderByDescending(s => Mathf.Abs(NormalizeValue(s.value, s.min, s.max))).ToList();

                // 上位1-2個をマイナスに調整
                int negativeCount = excess > budget * 0.5f ? 2 : 1;

                for (int i = 0; i < negativeCount && i < stats.Count; i++)
                {
                    var stat = stats[i];
                    float negativeValue = Random.Range(stat.min * 0.3f, stat.min * 0.7f); // マイナス範囲の30-70%

                    switch (stat.name)
                    {
                        case "speed": tempSpeed = negativeValue; break;
                        case "power": tempPower = negativeValue; break;
                        case "guard": tempGuard = negativeValue; break;
                        case "stamina": tempStamina = negativeValue; break;
                    }
                }

                // 再計算
                total =
                    Mathf.Abs(NormalizeValue(tempSpeed, speedMin, speedMax)) +
                    Mathf.Abs(NormalizeValue(tempPower, powerMin, powerMax)) +
                    Mathf.Abs(NormalizeValue(tempGuard, guardMin, guardMax)) +
                    Mathf.Abs(NormalizeValue(tempStamina, staminaMin, staminaMax));
            }

            float diff = Mathf.Abs(total - budget);

            // より予算に近い組み合わせを保存
            if (diff < bestDiff)
            {
                bestDiff = diff;
                bestSpeed = tempSpeed;
                bestPower = tempPower;
                bestGuard = tempGuard;
                bestStamina = tempStamina;

                // 十分予算に近い場合は終了
                if (diff < 0.15f) break;
            }
        }

        // 最適な値を適用（小数点3桁に丸める）
        speedValue = Mathf.Round(bestSpeed * 1000f) / 1000f;
        powerValue = Mathf.Round(bestPower * 1000f) / 1000f;
        guardValue = Mathf.Round(bestGuard * 1000f) / 1000f;
        staminaValue = Mathf.Round(bestStamina * 1000f) / 1000f;
    }

    /// <summary>
    /// 成長率情報を表示（表示のみ）
    /// </summary>
    private void DrawGrowthRateInfo()
    {
        if (!GrowthRateRules.ContainsKey(partType)) return;

        EditorGUILayout.BeginVertical("box");

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.normal.textColor = new Color(1f, 0.8f, 0.2f);
        EditorGUILayout.LabelField("⭐ 成長率パターン", titleStyle);

        var constraint = GrowthRateRules[partType];
        var currentGrowths = new List<GrowthRateType> { speedGrowth, powerGrowth, guardGrowth, staminaGrowth };

        EditorGUILayout.BeginHorizontal();

        // 各成長率の使用状況を表示
        foreach (var rate in new[] { GrowthRateType.VeryHigh, GrowthRateType.High, GrowthRateType.Normal, GrowthRateType.Low, GrowthRateType.VeryLow })
        {
            int allowed = constraint.AllowedCounts[rate];
            if (allowed == 0) continue;

            int used = currentGrowths.Count(g => g == rate);

            GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            boxStyle.alignment = TextAnchor.MiddleCenter;
            boxStyle.normal.textColor = used > 0 ? Color.green : Color.gray;
            boxStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(90));
            EditorGUILayout.LabelField(Calculation.GetGrowthRateStar(rate), new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter });
            EditorGUILayout.LabelField($"{used}/{allowed}", new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = used > 0 ? Color.green : Color.gray }
            });
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// ステータス表示（表示のみ）
    /// </summary>
    private void DrawStatDisplay(string name, float value, GrowthRateType growth, Color color)
    {
        EditorGUILayout.BeginVertical("box");

        // タイトル
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.normal.textColor = color;
        titleStyle.fontSize = 11;
        EditorGUILayout.LabelField(name, titleStyle);

        // プログレスバー的な表示
        float multiplier = Calculation.GetPartTypeMultiplier(partType);
        float min = 0f, max = 0f;

        if (name == "Speed")
        {
            min = KaomojiPart.Speed.MIN_VALUE * multiplier;
            max = KaomojiPart.Speed.MAX_VALUE * multiplier;
        }
        else if (name == "Power")
        {
            min = KaomojiPart.Power.MIN_VALUE * multiplier;
            max = KaomojiPart.Power.MAX_VALUE * multiplier;
        }
        else if (name == "Guard")
        {
            min = KaomojiPart.Guard.MIN_VALUE * multiplier;
            max = KaomojiPart.Guard.MAX_VALUE * multiplier;
        }
        else if (name == "Stamina")
        {
            min = KaomojiPart.Stamina.MIN_VALUE * multiplier;
            max = KaomojiPart.Stamina.MAX_VALUE * multiplier;
        }

        Rect barRect = EditorGUILayout.GetControlRect(false, 18);

        // 背景（より暗い灰色）
        EditorGUI.DrawRect(barRect, new Color(0.15f, 0.15f, 0.15f));

        float normalizedValue = value >= 0 ? (value / max) : (value / Mathf.Abs(min));
        float barWidth = Mathf.Abs(normalizedValue) * (barRect.width / 2f);
        float barX = value >= 0 ? barRect.x + barRect.width / 2f : barRect.x + barRect.width / 2f - barWidth;

        Rect fillRect = new Rect(barX, barRect.y, barWidth, barRect.height);

        // バーの色を暗めに調整（視認性向上）
        Color barColor;
        if (value >= 0)
        {
            // プラス値: 暗めの緑
            barColor = new Color(0.2f, 0.6f, 0.2f);
        }
        else
        {
            // マイナス値: 暗めの赤
            barColor = new Color(0.7f, 0.2f, 0.2f);
        }

        EditorGUI.DrawRect(fillRect, barColor);

        // 中央線（少し明るめの灰色で見やすく）
        Rect centerLine = new Rect(barRect.x + barRect.width / 2f - 1, barRect.y, 2, barRect.height);
        EditorGUI.DrawRect(centerLine, new Color(0.6f, 0.6f, 0.6f));

        // 値テキスト（白で明確に）
        GUIStyle valueStyle = new GUIStyle(EditorStyles.miniLabel);
        valueStyle.alignment = TextAnchor.MiddleCenter;
        valueStyle.normal.textColor = Color.white;
        valueStyle.fontStyle = FontStyle.Bold;
        string percentText = value >= 0 ? $"+{value * 100:F1}%" : $"{value * 100:F1}%";
        GUI.Label(barRect, percentText, valueStyle);

        EditorGUILayout.Space(2);

        // 成長率
        GUIStyle growthStyle = new GUIStyle(EditorStyles.helpBox);
        growthStyle.alignment = TextAnchor.MiddleCenter;
        growthStyle.normal.textColor = Color.yellow;
        growthStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField($"{Calculation.GetGrowthRateStar(growth)} {growth}", growthStyle);

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// コンパクトなレベルプレビュー
    /// </summary>
    private void DrawCompactLevelPreview()
    {
        // ヘッダー
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(60));
        for (int i = 1; i <= 5; i++)
        {
            GUIStyle headerStyle = new GUIStyle(EditorStyles.miniLabel);
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label($"Lv{i}", headerStyle, GUILayout.Width(65));
        }
        EditorGUILayout.EndHorizontal();

        // 各ステータス行
        DrawCompactPreviewRow("Speed", speedValue, speedGrowth, new Color(0.3f, 0.9f, 0.9f));
        DrawCompactPreviewRow("Power", powerValue, powerGrowth, new Color(1f, 0.4f, 0.4f));
        DrawCompactPreviewRow("Guard", guardValue, guardGrowth, new Color(0.4f, 0.7f, 1f));
        DrawCompactPreviewRow("Stamina", staminaValue, staminaGrowth, new Color(1f, 0.9f, 0.3f));
    }

    /// <summary>
    /// コンパクトなプレビュー行
    /// </summary>
    private void DrawCompactPreviewRow(string name, float baseValue, GrowthRateType growth, Color color)
    {
        EditorGUILayout.BeginHorizontal();

        GUIStyle nameStyle = new GUIStyle(EditorStyles.miniLabel);
        nameStyle.normal.textColor = color;
        nameStyle.fontStyle = FontStyle.Bold;
        GUILayout.Label(name, nameStyle, GUILayout.Width(60));

        for (int level = 1; level <= 5; level++)
        {
            float value = CalculateStatByLevel(baseValue, growth, level);
            string displayValue = value >= 0 ? $"+{value * 100:F0}%" : $"{value * 100:F0}%";

            GUIStyle valueStyle = new GUIStyle(EditorStyles.miniLabel);
            valueStyle.alignment = TextAnchor.MiddleCenter;
            valueStyle.normal.textColor = value >= 0 ? new Color(0.6f, 1f, 0.6f) : new Color(1f, 0.6f, 0.6f);

            GUILayout.Label(displayValue, valueStyle, GUILayout.Width(65));
        }

        EditorGUILayout.EndHorizontal();
    }

    private float CalculateStatByLevel(float baseValue, GrowthRateType growthType, int level)
    {
        if (level <= 1) return baseValue;
        float growthRate = Calculation.GetGrowthRateValue(growthType);
        return baseValue * level + (growthRate * baseValue);
    }

    private float GetCurrentTotalNormalized()
    {
        float multiplier = Calculation.GetPartTypeMultiplier(partType);

        float normalizedSpeed = NormalizeValue(speedValue, KaomojiPart.Speed.MIN_VALUE * multiplier, KaomojiPart.Speed.MAX_VALUE * multiplier);
        float normalizedPower = NormalizeValue(powerValue, KaomojiPart.Power.MIN_VALUE * multiplier, KaomojiPart.Power.MAX_VALUE * multiplier);
        float normalizedGuard = NormalizeValue(guardValue, KaomojiPart.Guard.MIN_VALUE * multiplier, KaomojiPart.Guard.MAX_VALUE * multiplier);
        float normalizedStamina = NormalizeValue(staminaValue, KaomojiPart.Stamina.MIN_VALUE * multiplier, KaomojiPart.Stamina.MAX_VALUE * multiplier);

        return Mathf.Abs(normalizedSpeed) + Mathf.Abs(normalizedPower) + Mathf.Abs(normalizedGuard) + Mathf.Abs(normalizedStamina);
    }

    private float NormalizeValue(float value, float min, float max)
    {
        return value >= 0 ? value / max : value / Mathf.Abs(min);
    }

    private float GetStatBudget()
    {
        return 2.0f * Calculation.GetPartTypeMultiplier(partType);
    }

    private string GetPrefix(KaomojiPartType type)
    {
        switch (type)
        {
            case KaomojiPartType.Eyes: return "Eyes_";
            case KaomojiPartType.Mouth: return "Mouth_";
            case KaomojiPartType.Hands: return "Hands_";
            case KaomojiPartType.Decoration_First: return "Deco1_";
            case KaomojiPartType.Decoration_Second: return "Deco2_";
            default: return "";
        }
    }

    private void ResetFields()
    {
        fileName = "";
        partName = "";
        partSymbol = "";
        partType = KaomojiPartType.Eyes;
        dropProbability = 0.3f;

        RandomizeAll();
        levelGrowth = GrowthRateType.Normal;

        maxDup = 50;
        isInitDisplay = false;
    }

    private void CreateKaomojiPartData()
    {
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("エラー", "ファイル名を入力してください", "OK");
            return;
        }

        if (string.IsNullOrEmpty(partName))
        {
            EditorUtility.DisplayDialog("エラー", "パーツ名を入力してください", "OK");
            return;
        }

        if (string.IsNullOrEmpty(partSymbol))
        {
            EditorUtility.DisplayDialog("エラー", "記号を入力してください", "OK");
            return;
        }

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        KaomojiPartData newData = CreateInstance<KaomojiPartData>();
        SerializedObject serializedData = new SerializedObject(newData);

        serializedData.FindProperty("dorpProbability").floatValue = dropProbability;
        serializedData.FindProperty("partType").enumValueIndex = (int)partType;

        SerializedProperty dataProp = serializedData.FindProperty("data");
        dataProp.FindPropertyRelative("partName").stringValue = partName;
        dataProp.FindPropertyRelative("part").stringValue = partSymbol;
        dataProp.FindPropertyRelative("maxDup").intValue = maxDup;
        dataProp.FindPropertyRelative("isInitDisplay").boolValue = isInitDisplay;

        SerializedProperty levelDetail = dataProp.FindPropertyRelative("levelDetail");
        levelDetail.FindPropertyRelative("growthRateType").enumValueIndex = (int)levelGrowth;

        SerializedProperty speed = dataProp.FindPropertyRelative("speed");
        speed.FindPropertyRelative("value").floatValue = speedValue;
        speed.FindPropertyRelative("growthRateType").enumValueIndex = (int)speedGrowth;

        SerializedProperty power = dataProp.FindPropertyRelative("power");
        power.FindPropertyRelative("value").floatValue = powerValue;
        power.FindPropertyRelative("growthRateType").enumValueIndex = (int)powerGrowth;

        SerializedProperty guard = dataProp.FindPropertyRelative("guard");
        guard.FindPropertyRelative("value").floatValue = guardValue;
        guard.FindPropertyRelative("growthRateType").enumValueIndex = (int)guardGrowth;

        SerializedProperty stamina = dataProp.FindPropertyRelative("stamina");
        stamina.FindPropertyRelative("value").floatValue = staminaValue;
        stamina.FindPropertyRelative("growthRateType").enumValueIndex = (int)staminaGrowth;

        serializedData.ApplyModifiedProperties();

        string fullFileName = $"{GetPrefix(partType)}{fileName}";
        string assetPath = $"{savePath}/{fullFileName}.asset";

        if (AssetDatabase.LoadAssetAtPath<KaomojiPartData>(assetPath) != null)
        {
            if (!EditorUtility.DisplayDialog("警告", $"'{fullFileName}.asset' を上書きしますか？", "上書き", "キャンセル"))
            {
                return;
            }
        }

        AssetDatabase.CreateAsset(newData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("成功", $"作成完了！\n{fullFileName}.asset", "OK");

        Selection.activeObject = newData;
        EditorGUIUtility.PingObject(newData);
    }
}