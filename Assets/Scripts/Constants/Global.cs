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
            public string name = "括弧";
            public int maxMental = 0;

            public string GetEffectDescription()
            {
                return $"精神強度が{maxMental}上昇する。";
            }

            public static string GetConditionBodyByLevel(int level)
            {
                switch(level)
                {
                    case 1:
                        return "手を装備している。";
                    case 2:
                        return "装飾1を装備している。";
                    case 3:
                        return "装飾2を装備している。";
                    default:
                        return "";
                }
            }
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


        // 総合ステータス
        public float Speed { get; private set; }
        public float Power { get; private set; }
        public float Guard { get; private set; }
        public float Stamina { get; private set; }

        public KaomojiPartData[] GetAllPartsData() => new KaomojiPartData[]
        { eyes, mouth, hands, decoration_first, decoration_second };

        public int GetEquippedPartsCount()
        {
            int count = 0;
            KaomojiPartData[] parts = GetAllPartsData();
            foreach(var part in parts)
            {
                if(part != null)
                {
                    count++;
                }
            }
            return count;
        }

        public void SetPartDataByType(KaomojiPartData partData)
        {
            switch(partData.PartType)
            {
                case KaomojiPartType.Eyes:
                    eyes = partData;
                    break;
                case KaomojiPartType.Mouth:
                    mouth = partData;
                    break;
                case KaomojiPartType.Hands:
                    hands = partData;
                    break;
                case KaomojiPartType.Decoration_First:
                    decoration_first = partData;
                    break;
                case KaomojiPartType.Decoration_Second:
                    decoration_second = partData;
                    break;
            }
        }

        /// <summary>
        /// 顔文字の組み立て
        /// </summary>
        /// <returns>設定された顔文字のPartsを合体させたもの</returns>
        public string BuildKaomoji(CharacterStatus.MentalData mentalData)
        {
            string _left_faceline = SeparatePart(mentalData?.faceline, 0);
            string _right_faceline = SeparatePart(mentalData?.faceline, 1);
            string _left_eye = SeparatePart(eyes?.Data.part, 0);
            string _right_eye = SeparatePart(eyes?.Data.part, 1);
            string _mouth = mouth?.Data.part;
            // string left_hands = SeparatePart(K.hands.Data.part, 0);
            // string right_hands = SeparatePart(K.hands.Data.part, 1);

            string merged = _left_faceline + _left_eye + _mouth + _right_eye + _right_faceline;
            return merged;
        }

        /// <summary>
        /// 記号全体のステータスを更新する
        /// </summary>
        public void UpdateTotalParameter()
        {
            KaomojiPartData[] datas = new KaomojiPartData[]
            { eyes, mouth, hands, decoration_first, decoration_second};

            Speed = Power = Guard = Stamina = 0f;
            foreach(var part in datas)
            {
                // 記号が割り当てられてなければスキップ
                if (part == null || part.Data == null) continue;
                
                int level = part.Data.levelDetail.Level;
                Speed += part.Data.speed.GetParameterByLevel(level);
                Power += part.Data.power.GetParameterByLevel(level);
                Guard += part.Data.guard.GetParameterByLevel(level);
                Stamina += part.Data.stamina.GetParameterByLevel(level);
            }
        }

        /// <summary>
        /// パーツの左右分割
        /// </summary>
        /// <param name="part">SOで設定した記号</param>
        /// <param name="index">左右どちらか(0=左側、1=右側)</param>
        /// <returns></returns>
        public string SeparatePart(string part, int index)
        {
            // index: 0=左側、1=右側
            // partがnullまたは空文字、indexが範囲外の場合は空文字を返す
            if(string.IsNullOrEmpty(part) || part.Length <= index)
            {
                return "";
            }

            return part[index].ToString();
        }
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
                CheckLevelUp(); // 引数は不要
            }

            // 複数レベルアップ対応
            public void CheckLevelUp()
            {
                // 無限ループ防止：必要経験値が0以下を返す設計は不可
                int safety = 1000;

                /*
                    経験値テーブルが壊れていて needExp = 0 を返した場合
                        → while (Exp >= need) が永久ループする
                        → ゲームがフリーズ
                */
                while (safety-- > 0)
                {
                    float need = GetNeedExpBorder(Level);

                    if (need <= 0f)
                    {
                        // テーブル不正。ここは例外でもログでも良い
                        break;
                    }

                    if (Exp < need) break;

                    Exp -= need;  // ここが重要：getExpではなく必要量を引く
                    Level++;
                }
            }

            public float GetNeedExpBorder(int level)
            {
                return Calculation.GetNeedExpBorderByLevelAndGrowthRateType(level, growthRateType);
            }
        }
        

        // レベルごとの効果値
        [System.Serializable]
        public sealed class Speed : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.5f;
            public const float MAX_VALUE = 0.5f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType;     public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                return level * (1 + Calculation.GetGrowthRate(growthRateType)) * value;
            }
        }

        [System.Serializable]
        public sealed class Power : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.7f;
            public const float MAX_VALUE = 0.7f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType;     public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                return level * (1 + Calculation.GetGrowthRate(growthRateType)) * value;
            }
        }

        [System.Serializable]
        public sealed class Guard : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.05f;
            public const float MAX_VALUE = 0.05f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType;     public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                return level * (1 + Calculation.GetGrowthRate(growthRateType)) * value;
            }
        }

        [System.Serializable]
        public sealed class Stamina : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.2f;
            public const float MAX_VALUE = 0.2f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType;     public GrowthRateType GrowthRateType => growthRateType;
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
}