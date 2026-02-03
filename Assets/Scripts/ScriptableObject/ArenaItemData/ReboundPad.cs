namespace ArenaItem.Data
{
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "ReboundPad", menuName = "ArenaItem/ReboundPad")]
    public class ReboundPad : ArenaItemData
    {
        [Header("反発力の初期値"), SerializeField] float reboundForce = 10f;   public float ReboundForce => reboundForce;

        public override string GetDiscription()
        {
            switch(GradeType)
            {
                case ENUM.ArenaItemGradeType.None:
                    return $"衝突した相手を跳ね返すパッド。";
                case ENUM.ArenaItemGradeType.MK_ONE:
                    return $"衝突した相手をやや強く跳ね返すパッド。";
                case ENUM.ArenaItemGradeType.MK_TWO:
                    return $"衝突した相手を強く跳ね返すパッド。";
                default:
                    return $"衝突した相手を跳ね返すパッド。";
            }
        }

        public float GetReboundForceByGrade()
        {
            switch(GradeType)
            {
                case ENUM.ArenaItemGradeType.None:
                    return reboundForce;
                case ENUM.ArenaItemGradeType.MK_ONE:
                    return reboundForce * 1.5f;
                case ENUM.ArenaItemGradeType.MK_TWO:
                    return reboundForce * 2f;
                default:
                    return reboundForce;
            }
        }
    }
}