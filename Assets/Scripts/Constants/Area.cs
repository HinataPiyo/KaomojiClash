namespace Constants
{
    using UnityEngine;
    using ENUM;

    [System.Serializable]
    public class AreaBuild
    {
        public EnemyDatabase spawnDatabase; // 出現する敵のデータベース
        [Header("文化圏レベル")] public int cultureLevel;            // 文化圏レベル
        [Header("顔文字密度(%)")] public float kaomojiDensity;        // 顔文字密度(%表記)

        static readonly float BaseCultureLevelMultiplier = 1f;   // 基本文化圏レベル倍率

        /// <summary>
        /// 文化圏レベルに応じたマルチプライヤーを取得する
        /// </summary>
        /// <param name="cultureLevel">単純なレベル</param>
        /// <returns>敵のステータスを上昇する倍率</returns>
        public static float GetCultureLevelMultiplier(int cultureLevel)
        {
            float multiplier = BaseCultureLevelMultiplier + ((cultureLevel - 1) * 0.15f);
            return multiplier;
        }

        /// <summary>
        /// 文化圏レベルに応じた敵の平均レベルを取得する
        /// Enemyにレベルは存在しないが、強さの指標として利用する
        /// </summary>
        /// <param name="cultureLevel"></param>
        /// <returns></returns>
        public static int GetEnemyAverageLevel(int cultureLevel)
        {
            return cultureLevel + Mathf.FloorToInt(GetCultureLevelMultiplier(cultureLevel) * cultureLevel);
        }

        /// <summary>
        /// Waveの難易度に応じた敵の平均レベルを取得する
        /// </summary>
        /// <param name="cultureLevel"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
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
    }
}