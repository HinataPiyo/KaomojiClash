namespace ArenaItem
{
    using UnityEngine;

    public class HealingPad : ArenaItemBase, IReboundPad
    {
        [SerializeField] Data.HealingPad data;
        void OnCollisionEnter2D(Collision2D col)
        {
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            ApplyHealing(rb);
            ApplyRebound(rb, col.gameObject);
        }

        /// <summary>
        /// 回復を適用する
        /// </summary>
        /// <param name="rb"></param>
        void ApplyHealing(Rigidbody2D rb)
        {
            if(rb != null)
            {
                if(rb.gameObject.CompareTag(Layer.Player))
                {
                    IHeal heal = rb.GetComponent<IHeal>();
                    if(heal != null)
                    {
                        float maxHealth = heal.GetMaxHealthAmount;
                        float healAmount = data.GetHealAmountByGrade(maxHealth);
                        heal.Heal(healAmount);
                    }
                }
            }
        }

        /// <summary>
        /// 反発を適用する
        /// </summary>
        /// <param name="target"></param>
        public void ApplyRebound(Rigidbody2D rb, GameObject target)
        {
            if(rb != null)
            {
                Vector2 reboundDir = (target.transform.position - transform.position).normalized;
                float reboundForce = data.GetReboundForceByGrade();
                rb.AddForce(reboundDir * reboundForce, ForceMode2D.Impulse);
                anim.SetTrigger(ANIM_TRIGGER_PLAY);
            }
        }

        
    }
}