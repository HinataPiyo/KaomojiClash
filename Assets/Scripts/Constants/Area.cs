namespace Constants
{
    using UnityEngine;
    using ENUM;
    using System.Collections.Generic;

    [System.Serializable]
    public class AreaBuild
    {
        [Header("敵出現設定")]
        public EnemySpawnConfig spawnConfig = new EnemySpawnConfig();

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
        /// <param name="cultureLevel">文化圏レベル</param>
        /// <param name="increaseRate">1レベルごとの増加率（例: 0.15 = 15%）</param>
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
            switch (difficulty)
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

            return availableTypes;
        }

        /// <summary>
        /// 指定された部位が使用可能かチェック
        /// </summary>
        public bool IsPartTypeAvailable(int cultureLevel, KaomojiPartType partType)
        {
            foreach (var config in unlockConfigs)
            {
                if (config.partType == partType && cultureLevel >= config.unlockCultureLevel)
                {
                    return true;
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

            foreach (var config in unlockConfigs)
            {
                if (config.unlockCultureLevel > cultureLevel && config.unlockCultureLevel < minLevel)
                {
                    nextUnlock = config;
                    minLevel = config.unlockCultureLevel;
                }
            }

            return nextUnlock;
        }
    }
}