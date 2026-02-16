namespace ArenaItem.Data
{
    using System.Threading;
    using UnityEditor.EditorTools;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "Medikit", menuName = "ArenaItem/Medikit")]
    public class Medikit : ArenaItemData
    {
        [Tooltip("デフォルト回復量（%）")]
        [Header("回復量"), Range(0.01f, 1f), SerializeField] float healAmount = 0.25f;
        
        [Tooltip("再度使用可能になるまでの時間（秒）")]
        [Header("クールタイム"), Range(1f, 60f), SerializeField] float coolTime = 10f;

        [Tooltip("0がActive, 1がInactive")]
        [Header("状態表示"), SerializeField] Sprite[] sprites;

        /// <summary>
        /// 状態に応じたスプライトを取得するs
        /// </summary>
        /// <param name="isActive">使用済みかどうか</param>
        /// <returns></returns>
        public Sprite GetSpriteByActive(bool isActive)
        {
            if(isActive)
            {
                return sprites[0];
            }
            else
            {
                return sprites[1];
            }
        }

        public override string GetDiscription()
        {
            switch(GradeType)
            {
                case ENUM.ArenaItemGradeType.None:
                    return $"触れると最大スタミナの{healAmount * 100}%回復する簡易医療キット。";
                case ENUM.ArenaItemGradeType.MK_ONE:
                    return $"触れると最大スタミナの{healAmount * 1.5f * 100}%回復する医療キット。";
                case ENUM.ArenaItemGradeType.MK_TWO:
                    return $"触れると最大スタミナの{healAmount * 2f * 100}%回復する高性能医療キット。";
                default:
                    return $"-";
            }
        }

        public float GetHealAmountByGrade()
        {
            switch(GradeType)
            {
                case ENUM.ArenaItemGradeType.None:
                    return healAmount;
                case ENUM.ArenaItemGradeType.MK_ONE:
                    return healAmount * 1.5f;
                case ENUM.ArenaItemGradeType.MK_TWO:
                    return healAmount * 2f;
                default:
                    return 0f;
            }
        }

        public float GetCoolTimeByGrade()
        {
            switch(GradeType)
            {
                case ENUM.ArenaItemGradeType.None:
                    return coolTime;
                case ENUM.ArenaItemGradeType.MK_ONE:
                    return coolTime * 0.75f;
                case ENUM.ArenaItemGradeType.MK_TWO:
                    return coolTime * 0.5f;
                default:
                    return 0f;
            }
        }
        
    }
}