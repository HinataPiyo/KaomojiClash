using System;
using UnityEngine;

namespace Constants.Global
{
    [Serializable]
    public class CharacterStatus
    {
        public float maxMental = 10f;
        public int maxSeparate = 3;

        [Header("Launch")]
        public float launchPower = 10f;          // 最大発射パワー
        public float maxDragDistance = 3f;       // ドラッグ最大距離
        public float minLaunchDistance = 0.1f;   // これ未満なら発射しない

        [Header("Landing / Cooldown")]
        public float landingCooldown = 0.25f;    // 着地硬直時間(=CT)
    }
}