namespace ArenaItem.Data
{
    using UnityEngine;
    using ENUM;
    
    [CreateAssetMenu(fileName = "SpeedUpArea", menuName = "ArenaItem/SpeedUpArea")]
    public class SpeedUpArea : ArenaItemData
    {
        [Tooltip("デフォルト値")]
        [Header("効果範囲"), Range(0.5f, 5f), SerializeField] float effectRange = 1f;
        [Header("スピードアップ倍率"), Range(0.1f, 1f), SerializeField] float speedMultiplier = 0.5f;

        public float GetEffectRange()
        {
            switch (GradeType)
            {
                case ArenaItemGradeType.None:
                    return effectRange;
                case ArenaItemGradeType.MK_ONE:
                    return effectRange * 1.5f;
                case ArenaItemGradeType.MK_TWO:
                    return effectRange * 2f;
                default:
                    return effectRange;
            }
        }

        public float GetSpeedMultiplier()
        {
            switch(GradeType)
            {
                case ArenaItemGradeType.None:
                    return speedMultiplier;
                case ArenaItemGradeType.MK_ONE:
                    return speedMultiplier * 1.5f;
                case ArenaItemGradeType.MK_TWO:
                    return speedMultiplier * 2f;
                default:
                    return speedMultiplier;
            }
        }

        public override string GetDiscription()
        {
            switch(GradeType)
            {
                case ArenaItemGradeType.None:
                    return $"エリア内にいると移動速度が{speedMultiplier}倍になる。";
                case ArenaItemGradeType.MK_ONE:
                    return $"エリア内にいると移動速度が{speedMultiplier * 1.5f}倍になる。";
                case ArenaItemGradeType.MK_TWO:
                    return $"エリア内にいると移動速度が{speedMultiplier * 2f}倍になる。";
                default:
                    return $"-";
            }
        }
    }
}