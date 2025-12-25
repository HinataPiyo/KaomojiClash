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
        public float launchPower = 10f;          // 最大発射パワー
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
        public float coliderXSize = 1.5f;      // コライダーの当たり判定の幅を調整
        public MentalData mentalData;
        public KaomojiPart eyes;
        public KaomojiPart mouth;
        public KaomojiPart decoration_first;
        public KaomojiPart decoration_second;
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
        public string part;
        
        [Tooltip("移動速度に影響")] public Level speed;
        [Tooltip("攻撃力に影響")] public Level power;
        [Tooltip("防御力に影響")] public Level guard;
        [Tooltip("体力に影響")] public Level stamina;
        [Serializable]
        public class Level
        {
            [Range(0, 8)] public int level;
            [Range(-10, 10)] public float value;
        }
    }
}