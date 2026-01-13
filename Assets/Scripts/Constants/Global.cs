using System.Collections.Generic;
using ENUM;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace Constants.Global
{
    [System.Serializable]
    public sealed class CharacterStatus
    {
        public float attackPower = 1f;
        public float maxHealth = 10f;

        [Header("Launch")]
        public float default_LaunchPower = 10f;          // 最大発射パワー
        public float maxDragDistance = 3f;       // ドラッグ最大距離
        public float minLaunchDistance = 0.1f;   // これ未満なら発射しない

        [Header("Landing / Cooldown")]
        public float landingCooldown = 0.25f;    // 着地硬直時間(=CT)

        [Header("Reflect")]
        public float reflectPower = 1f;         // 反射時の速度倍率

        [Header("MentalPower")]
        public MentalData mentalData;

        // 精神強度など精神に関するデータ
        [System.Serializable]
        public sealed class MentalData
        {
            public string faceline = "()";
            public int maxMental = 3;
        }
    }

    [System.Serializable]
    public sealed class KAOMOJI
    {
        public const float ColiderXSize = 0.1f;      // コライダーの当たり判定の幅を調整
        public KaomojiPartData eyes;
        public KaomojiPartData mouth;
        public KaomojiPartData hands;
        public KaomojiPartData decoration_first;
        public KaomojiPartData decoration_second;
    }

    [System.Serializable]
    public sealed class KaomojiPart
    {
        /// <summary>
        /// 顔文字のパーツが持つステータスはこんだけ
        /// </summary>
        public string partName;
        public string part;
        public LevelDetail levelDetail;
            
        // %で計算(例: 0.1なら10%UP)
        [Tooltip("移動速度に影響")] public Speed speed;
        [Tooltip("攻撃力に影響")] public Power power;
        [Tooltip("防御力に影響")] public Guard guard;
        [Tooltip("体力に影響")] public Stamina stamina;

        [System.Serializable]
        public class LevelDetail
        {
            [SerializeField] GrowthRateType growthRateType;
            public int Level { get; private set; } = 1;
            public float Exp { get; private set; } = 0f;

            public void AddExperience(float exp)
            {
                Exp += exp;
                CheckLevelUp();     // レベルアップの確認
            }

            public void CheckLevelUp()
            {
                if(Exp >= Calculation.GetNeedExpBorderByLevelAndGrowthRateType(Level, growthRateType))
                {
                    Level++;
                    Exp = 0f;
                }
            }

            public float GetNeedExpBorder()
            {
                return Calculation.GetNeedExpBorderByLevelAndGrowthRateType(Level, growthRateType);
            }
        }
        

        // レベルごとの効果値
        [System.Serializable]
        public sealed class Speed : IKaomojiPartParameter
        {
            [Range(-0.5f, 0.5f)] public float value;
            [SerializeField] GrowthRateType growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                return level * (1 + Calculation.GetGrowthRate(growthRateType)) * value;
            }
        }

        [System.Serializable]
        public sealed class Power : IKaomojiPartParameter
        {
            [Range(-0.7f, 0.7f)] public float value;
            [SerializeField] GrowthRateType growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                return level * (1 + Calculation.GetGrowthRate(growthRateType)) * value;
            }
        }

        [System.Serializable]
        public sealed class Guard : IKaomojiPartParameter
        {
            [Range(-0.05f, 0.05f)] public float value;
            [SerializeField] GrowthRateType growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                return level * (1 + Calculation.GetGrowthRate(growthRateType)) * value;
            }
        }

        [System.Serializable]
        public sealed class Stamina : IKaomojiPartParameter
        {
            [Range(-0.2f, 0.2f)] public float value;
            [SerializeField] GrowthRateType growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                return level * (1 + Calculation.GetGrowthRate(growthRateType)) * value;
            }
        }
        
    }

    /// <summary>
    /// 顔文字記号データの所持数
    /// </summary>
    [System.Serializable]
    public sealed class HasKaomojiParts
    {
        public int amount;
        public KaomojiPartData part;
    }

    public sealed class Wave
    {
        public Difficulty difficulty;
        public List<Element> elements = new List<Element>();
        public List<HasKaomojiParts> dropKaomojiParts = new List<HasKaomojiParts>();
        public int getMoney;
        public float getExp;

        /// <summary>
        /// 1Waveごとのデータ
        /// </summary>
        public sealed class Element
        {
            // 出現する敵データ
            public List<EnemyData> datas = new List<EnemyData>();
        }
    }

    public static class Calculation
    {
        /// <summary>
        /// 難易度に応じてレベルを取得する
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public static int GetLevelByDifficulty(Difficulty difficulty)
        {
            switch(difficulty)
            {
                case Difficulty.Easy:
                    return Random.Range(1, 3);
                case Difficulty.Normal:
                    return Random.Range(2, 4);
                case Difficulty.Hard:
                    return Random.Range(3, 6);
                case Difficulty.Extreme:
                    return Random.Range(5, 8);
                default:
                    return 0;
            }
        }

        public static Color GetColorByDifficulty(Difficulty difficulty)
        {
            switch(difficulty)
            {
                case Difficulty.Easy:
                    return Color.green;
                case Difficulty.Normal:
                    return Color.yellow;
                case Difficulty.Hard:
                    return new Color32(255, 100, 0, 255);
                case Difficulty.Extreme:
                    return Color.red;
                default:
                    return Color.white;
            }
        }

        /// <summary>
        /// レベルや難易度に応じて獲得金額を調整する
        /// </summary>
        public static int GetMoneyByDifficultyAndLevel(Difficulty difficulty, int level)
        {
            int baseMoney = 5;
            float difficultyRate = 1.0f + (int)difficulty * 0.1f;
            float levelRate      = 1.0f + level * 0.05f;

            int getMoney = Mathf.FloorToInt(baseMoney * difficultyRate * levelRate);
            return getMoney;
        }

        /// <summary>
        /// 記号のレベルアップ時の成長率を取得する関数
        /// </summary>
        public static float GetGrowthRate(GrowthRateType type)
        {
            switch(type)
            {
                case GrowthRateType.VeryLow:
                    return 0.05f;
                case GrowthRateType.Low:
                    return 0.1f;
                case GrowthRateType.Normal:
                    return 0.2f;
                case GrowthRateType.High:
                    return 0.3f;
                case GrowthRateType.VeryHigh:
                    return 0.4f;
                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static float GetExperienceByDifficultyAndLevel(Difficulty difficulty, int level)
        {
            int baseExp = 10;
            float difficultyRate = 1.0f + (int)difficulty * 0.25f;
            float levelRate      = 1.0f + level * 0.05f;

            float exp = baseExp * difficultyRate * levelRate;
            return exp;
        }

        public static float GetNeedExpBorderByLevelAndGrowthRateType(int level, GrowthRateType type)
        {
            float baseBoderExp = 500f;
            switch(type)
            {
                case GrowthRateType.VeryLow:
                    return baseBoderExp * 1.4f * level;
                case GrowthRateType.Low:
                    return baseBoderExp * 1.2f * level;
                case GrowthRateType.Normal:
                    return baseBoderExp * 1.0f * level;
                case GrowthRateType.High:
                    return baseBoderExp * 0.8f * level;
                case GrowthRateType.VeryHigh:
                    return baseBoderExp * 0.6f * level;
                default:
                    return 0.0f;
            }
        }
    }
}

namespace ENUM
{
    public enum KaomojiPartType
    { Eyes, Mouth, Hands, Decoration_First, Decoration_Second }
    public enum BattleStat
    { None = -1, Start, Now, End }
    public enum Difficulty
    { Easy, Normal, Hard, Extreme, Max }
    public enum GrowthRateType
    { VeryLow, Low, Normal, High, VeryHigh }
}