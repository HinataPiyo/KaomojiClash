namespace Constants
{
    using UnityEngine;
    using ENUM;
    using System.Collections.Generic;
    using System.Linq;

    [System.Serializable]
    public class AreaBuild
    {
        [Header("敵出現設定")]
        public EnemySpawnConfig spawnConfig = new EnemySpawnConfig();
        
        [Header("MentalData設定")]
        [Tooltip("敵に使用するMentalDataのリスト（ランダム選択）")]
        public List<MentalData> mentalDataList = new List<MentalData>();
        
        [Header("部位解放設定")]
        public PartUnlockManager partUnlockManager = new PartUnlockManager();
        
        [Header("文化圏レベル")] 
        public int cultureLevel = 1;
        
        [Header("顔文字密度(%)")] 
        public float kaomojiDensity = 0.5f;

        static readonly float BaseCultureLevelMultiplier = 1f;

        /// <summary>
        /// 文化圏レベルに応じたマルチプライヤーを取得する（デフォルト15%増加）
        /// </summary>
        public static float GetCultureLevelMultiplier(int cultureLevel)
        {
            return GetCultureLevelMultiplier(cultureLevel, 0.15f);
        }

        /// <summary>
        /// 文化圏レベルに応じたマルチプライヤーを取得する（カスタム増加率）
        /// </summary>
        public static float GetCultureLevelMultiplier(int cultureLevel, float increaseRate)
        {
            float multiplier = BaseCultureLevelMultiplier + ((cultureLevel - 1) * increaseRate);
            return multiplier;
        }

        /// <summary>
        /// 文化圏レベルに応じた敵の平均レベルを取得する
        /// </summary>
        public static int GetEnemyAverageLevel(int cultureLevel)
        {
            return cultureLevel + Mathf.FloorToInt(GetCultureLevelMultiplier(cultureLevel) * cultureLevel);
        }

        /// <summary>
        /// Waveの難易度に応じた敵の平均レベルを取得する
        /// </summary>
        public static int GetEnemyAverageLevelByWaveDifficulty(int cultureLevel, Difficulty difficulty)
        {
            switch(difficulty)
            {
                case Difficulty.Easy:
                    return Mathf.Max(1, GetEnemyAverageLevel(cultureLevel) - 2);
                case Difficulty.Normal:
                    return GetEnemyAverageLevel(cultureLevel) + 1;
                case Difficulty.Hard:
                    return GetEnemyAverageLevel(cultureLevel) + 2;
                case Difficulty.Extreme:
                    return GetEnemyAverageLevel(cultureLevel) + 4;
                default:
                    return GetEnemyAverageLevel(cultureLevel);
            }
        }

        /// <summary>
        /// 文化圏レベルから顔文字密度を自動計算
        /// </summary>
        public static float CalculateKaomojiDensity(int cultureLevel)
        {
            return Mathf.Clamp(0.3f + (cultureLevel * 0.05f), 0.3f, 0.9f);
        }
        
        /// <summary>
        /// MentalDataリストからランダムに1つ取得
        /// </summary>
        public MentalData GetRandomMentalData()
        {
            if (mentalDataList == null || mentalDataList.Count == 0)
            {
                Debug.LogWarning("MentalDataリストが空です");
                return null;
            }
            
            var validMentals = mentalDataList.Where(m => m != null).ToList();
            if (validMentals.Count == 0)
            {
                Debug.LogWarning("有効なMentalDataがありません");
                return null;
            }
            
            return validMentals[UnityEngine.Random.Range(0, validMentals.Count)];
        }
    }

    /// <summary>
    /// 敵出現設定
    /// </summary>
    [System.Serializable]
    public class EnemySpawnConfig
    {
        [Header("生成モード")]
        public GenerationMode mode = GenerationMode.Manual;

        [Header("敵リスト")]
        [Tooltip("出現させる敵データ（この中からランダムに選択）")]
        public List<EnemyData> fixedEnemies = new List<EnemyData>();

        [Header("難易度別出現数")]
        public DifficultySpawnAmount[] spawnAmounts = new DifficultySpawnAmount[]
        {
            new DifficultySpawnAmount { difficulty = Difficulty.Easy, amount = 2 },
            new DifficultySpawnAmount { difficulty = Difficulty.Normal, amount = 3 },
            new DifficultySpawnAmount { difficulty = Difficulty.Hard, amount = 2 },
            new DifficultySpawnAmount { difficulty = Difficulty.Extreme, amount = 1 }
        };

        /// <summary>
        /// 全難易度の合計出現数を取得
        /// </summary>
        public int GetTotalSpawnAmount()
        {
            int total = 0;
            if (spawnAmounts != null)
            {
                foreach (var amount in spawnAmounts)
                {
                    total += amount.amount;
                }
            }
            return total;
        }

        /// <summary>
        /// 特定難易度の出現数を取得
        /// </summary>
        public int GetAmountByDifficulty(Difficulty difficulty)
        {
            if (spawnAmounts != null)
            {
                foreach (var amount in spawnAmounts)
                {
                    if (amount.difficulty == difficulty)
                        return amount.amount;
                }
            }
            return 0;
        }
    }

    /// <summary>
    /// 生成モード
    /// </summary>
    public enum GenerationMode
    {
        Manual       // 手動（敵リストからランダム選択）
    }

    /// <summary>
    /// 難易度別出現数
    /// </summary>
    [System.Serializable]
    public class DifficultySpawnAmount
    {
        public Difficulty difficulty;
        public int amount;
    }

    /// <summary>
    /// 文化圏レベルによる部位解放設定
    /// </summary>
    [System.Serializable]
    public class PartUnlockConfig
    {
        [Tooltip("この文化圏レベルから解放される")]
        public int unlockCultureLevel;
        
        [Tooltip("解放される部位タイプ")]
        public KaomojiPartType partType;
        
        [Tooltip("説明（エディタ表示用）")]
        public string description = "";
    }

    /// <summary>
    /// 部位解放管理クラス
    /// </summary>
    [System.Serializable]
    public class PartUnlockManager
    {
        [Header("部位解放設定")]
        [Tooltip("文化圏レベルごとの解放設定")]
        public List<PartUnlockConfig> unlockConfigs = new List<PartUnlockConfig>()
        {
            new PartUnlockConfig { unlockCultureLevel = 1, partType = KaomojiPartType.Mouth, description = "口（基本）" },
            new PartUnlockConfig { unlockCultureLevel = 6, partType = KaomojiPartType.Eyes, description = "目" },
            new PartUnlockConfig { unlockCultureLevel = 11, partType = KaomojiPartType.Hands, description = "手" },
            new PartUnlockConfig { unlockCultureLevel = 16, partType = KaomojiPartType.Decoration_First, description = "装飾1" },
            new PartUnlockConfig { unlockCultureLevel = 21, partType = KaomojiPartType.Decoration_Second, description = "装飾2" }
        };

        /// <summary>
        /// 指定された文化圏レベルで使用可能な部位を取得
        /// </summary>
        public List<KaomojiPartType> GetAvailablePartTypes(int cultureLevel)
        {
            var availableTypes = new List<KaomojiPartType>();
            
            if (unlockConfigs != null)
            {
                foreach (var config in unlockConfigs)
                {
                    if (cultureLevel >= config.unlockCultureLevel)
                    {
                        if (!availableTypes.Contains(config.partType))
                        {
                            availableTypes.Add(config.partType);
                        }
                    }
                }
            }
            
            return availableTypes;
        }

        /// <summary>
        /// 指定された部位が使用可能かチェック
        /// </summary>
        public bool IsPartTypeAvailable(int cultureLevel, KaomojiPartType partType)
        {
            if (unlockConfigs != null)
            {
                foreach (var config in unlockConfigs)
                {
                    if (config.partType == partType && cultureLevel >= config.unlockCultureLevel)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 次に解放される部位の情報を取得
        /// </summary>
        public PartUnlockConfig GetNextUnlock(int cultureLevel)
        {
            PartUnlockConfig nextUnlock = null;
            int minLevel = int.MaxValue;
            
            if (unlockConfigs != null)
            {
                foreach (var config in unlockConfigs)
                {
                    if (config.unlockCultureLevel > cultureLevel && config.unlockCultureLevel < minLevel)
                    {
                        nextUnlock = config;
                        minLevel = config.unlockCultureLevel;
                    }
                }
            }
            
            return nextUnlock;
        }
    }
}