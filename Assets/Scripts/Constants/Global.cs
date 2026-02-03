using System.Collections.Generic;
using ENUM;
using UnityEngine;

namespace Constants.Global
{
    public sealed class Wave
    {
        public Difficulty difficulty;
        public List<Element> elements = new List<Element>();
        public List<HasKaomojiParts> dropKaomojiParts = new List<HasKaomojiParts>();
        public int getMoney;
        public float getExp;
        public List<int> befor_level = new List<int>();
        public List<float> befor_exp = new List<float>();


        /// <summary>
        /// 1Waveごとのデータ
        /// </summary>
        public sealed class Element
        {
            // 出現する敵データ
            public List<EnemyData> datas = new List<EnemyData>();
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

        /// <summary>
        /// 難易度に応じて色を取得する
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns></returns>
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
                    return 0.85f;
                case GrowthRateType.Low:
                    return 0.92f;
                case GrowthRateType.Normal:
                    return 1f;
                case GrowthRateType.High:
                    return 1.20f;
                case GrowthRateType.VeryHigh:
                    return 1.35f;
                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static float GetExperienceByDifficultyAndLevel(Difficulty difficulty, int level)
        {
            int baseExp = 100;
            float difficultyRate = 1.0f + (int)difficulty * 0.25f;
            float levelRate      = 1.0f + level * 0.05f;

            float exp = baseExp * difficultyRate * levelRate;
            return exp;
        }

        public static float GetNeedExpBorderByLevelAndGrowthRateType(int level, GrowthRateType type)
        {
            float baseBoderExp = 500f;
            if(level == 1) return baseBoderExp;

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

        public static string GetKaomojiPartTypeName(KaomojiPartType type)
        {
            switch(type)
            {
                case KaomojiPartType.Eyes:
                    return "目";
                case KaomojiPartType.Mouth:
                    return "口";
                case KaomojiPartType.Hands:
                    return "手";
                case KaomojiPartType.Decoration_First:
                    return "装飾1";
                case KaomojiPartType.Decoration_Second:
                    return "装飾2";
                default:
                    return "";
            }
            
        }

#region StatusParamater

        public static string GetGrowthRateStar(GrowthRateType type)
        {
            switch(type)
            {
                case GrowthRateType.None:
                    return "-";
                case GrowthRateType.VeryLow:
                    return "★";
                case GrowthRateType.Low:
                    return "★★";
                case GrowthRateType.Normal:
                    return "★★★";
                case GrowthRateType.High:
                    return "★★★★";
                case GrowthRateType.VeryHigh:
                    return "★★★★★";
                default:
                    return "";
            }
        }

        public static Color GetStatusTypeColorByType(StatusType type)
        {
            Color color;
            switch(type)
            {
                case StatusType.Speed:
                    ColorUtility.TryParseHtmlString("#06D6A0", out color);
                    return color;
                case StatusType.Power:
                    ColorUtility.TryParseHtmlString("#EF476F", out color);
                    return color;
                case StatusType.Guard:
                    ColorUtility.TryParseHtmlString("#118AB2", out color);
                    return color;
                case StatusType.Stamina:
                    ColorUtility.TryParseHtmlString("#FFD166", out color);
                    return color;
                default:
                    return Color.white;
            }
        }

        public static string GetStatusTypeNameByType(StatusType type)
        {
            switch(type)
            {
                case StatusType.Speed:
                    return "スピード";
                case StatusType.Power:
                    return "パワー";
                case StatusType.Guard:
                    return "ガード";
                case StatusType.Stamina:
                    return "スタミナ";
                default:
                    return "";
            }
        }

#endregion

        public static readonly Dictionary<ArenaItemGradeType, string> GradeTypeName = new Dictionary<ArenaItemGradeType, string>
        {
            { ArenaItemGradeType.None, "NORMAL" },
            { ArenaItemGradeType.MK_ONE, "MK-1" },
            { ArenaItemGradeType.MK_TWO, "MK-2" },
        };
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
    { None = -1, VeryLow, Low, Normal, High, VeryHigh }
    public enum StatusType
    { None = -1, Speed, Power, Guard, Stamina, Max}
    public enum Panel
    { None = -1, Home, KaomojiBuild, ArenaBuild, Shop, Max}
    public enum Scene
    { None = -1, Title, Home, Battle, Max}
    public enum ArenaItemGradeType
    { None = -1, MK_ONE, MK_TWO, Max }
    public enum ArenaBuildType
    { None = -1, Placeable, Automatic, Max }
}