using System.Collections.Generic;
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
        
        // %で計算(例: 0.1なら10%UP)
        [Tooltip("移動速度に影響")] public Speed speed;
        [Tooltip("攻撃力に影響")] public Power power;
        [Tooltip("防御力に影響")] public Guard guard;
        [Tooltip("体力に影響")] public Stamina stamina;

        // レベルごとの効果値
        [System.Serializable]
        public sealed class Speed
        {
            [Range(1, 7)] public int level;
            [Range(-0.5f, 0.5f)] public float value;
            public float GetSpeedByLevel() => level * value;
        }

        [System.Serializable]
        public sealed class Power
        {
            [Range(1, 7)] public int level;
            [Range(-0.7f, 0.7f)] public float value;
            public float GetPowerByLevel() => level * value;
        }

        [System.Serializable]
        public sealed class Guard
        {
            [Range(1, 7)] public int level;
            [Range(-0.05f, 0.05f)] public float value;
            public float GetGuardByLevel() => level * value;
        }

        [System.Serializable]
        public sealed class Stamina
        {
            [Range(1, 7)] public int level;
            [Range(-0.2f, 0.2f)] public float value;
            public float GetStaminaByLevel() => level * value;
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
        public ENUM.Difficulty difficulty;
        public List<Element> elements = new List<Element>();
        public List<HasKaomojiParts> dropKaomojiParts = new List<HasKaomojiParts>();

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
        public static int GetLevelByDifficulty(ENUM.Difficulty difficulty)
        {
            switch(difficulty)
            {
                case ENUM.Difficulty.Easy:
                    return Random.Range(1, 3);
                case ENUM.Difficulty.Normal:
                    return Random.Range(2, 4);
                case ENUM.Difficulty.Hard:
                    return Random.Range(3, 6);
                case ENUM.Difficulty.Extreme:
                    return Random.Range(5, 8);
                default:
                    return 0;
            }
        }

        public static Color GetColorByDifficulty(ENUM.Difficulty difficulty)
        {
            switch(difficulty)
            {
                case ENUM.Difficulty.Easy:
                    return Color.green;
                case ENUM.Difficulty.Normal:
                    return Color.yellow;
                case ENUM.Difficulty.Hard:
                    return new Color32(255, 100, 0, 255);
                case ENUM.Difficulty.Extreme:
                    return Color.red;
                default:
                    return Color.white;
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
}