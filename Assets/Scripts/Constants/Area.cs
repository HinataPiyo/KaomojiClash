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
}