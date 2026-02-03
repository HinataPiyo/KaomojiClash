namespace ArenaItem.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "HealingPad", menuName = "ArenaItem/HealingPad")]
    public class HealingPad : ReboundPad
    {
        [Header("回復量"), SerializeField] float healMultiplier = 0.2f;

        public override string GetDiscription()
        {
            switch(GradeType)
            {
                case ENUM.ArenaItemGradeType.None:
                    return $"自身が衝突すると少し跳ね返り、微量回復させるパッド。";
                case ENUM.ArenaItemGradeType.MK_ONE:
                    return $"自身が衝突すると跳ね返り、少し回復させるパッド。";
                case ENUM.ArenaItemGradeType.MK_TWO:
                    return $"自身が衝突すると強く跳ね返し、やや回復させるパッド。";
                default:
                    return $"-";
            }
        }

        public float GetHealAmountByGrade(float maxHealth)
        {
            switch(GradeType)
            {
                case ENUM.ArenaItemGradeType.None:
                    return maxHealth * healMultiplier;
                case ENUM.ArenaItemGradeType.MK_ONE:
                    return maxHealth * (healMultiplier  * 1.25f);
                case ENUM.ArenaItemGradeType.MK_TWO:
                    return maxHealth * (healMultiplier * 1.5f);
                default:
                    return 0f;
            }
        }
    }

}