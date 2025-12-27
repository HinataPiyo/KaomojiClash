using System;
using UnityEngine;

namespace Constants.Global
{
    [Serializable]
    public class CharacterStatus
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
    }

    [Serializable]
    public class KAOMOJI
    {
        public const float ColiderXSize = 0.1f;      // コライダーの当たり判定の幅を調整
        public MentalData mentalData;
        public KaomojiPartData eyes;
        public KaomojiPartData mouth;
        public KaomojiPartData hands;
        public KaomojiPartData decoration_first;
        public KaomojiPartData decoration_second;

        // 精神強度など精神に関するデータ
        [Serializable]
        public class MentalData
        {
            public string faceline = "()";
            public int maxMental = 3;
        }
    }

    [Serializable]
    public class KaomojiPart
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
        [Serializable]
        public class Speed
        {
            [Range(1, 7)] public int level;
            [Range(-0.5f, 0.5f)] public float value;
            public float GetSpeedByLevel() => level * value;
        }

        [Serializable]
        public class Power
        {
            [Range(1, 7)] public int level;
            [Range(-0.7f, 0.7f)] public float value;
            public float GetPowerByLevel() => level * value;
        }

        [Serializable]
        public class Guard
        {
            [Range(1, 7)] public int level;
            [Range(-0.05f, 0.05f)] public float value;
            public float GetGuardByLevel() => level * value;
        }

        [Serializable]
        public class Stamina
        {
            [Range(1, 7)] public int level;
            [Range(-0.2f, 0.2f)] public float value;
            public float GetStaminaByLevel() => level * value;
        }
        
    }
}

namespace ENUM
{
    public enum KaomojiPartType
    { Eyes, Mouth, Hands, Decoration_First, Decoration_Second }
}