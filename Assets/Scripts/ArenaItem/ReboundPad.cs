namespace ArenaItem
{
    using UnityEngine;

    interface IReboundPad
    {
        void ApplyRebound(Rigidbody2D rb, GameObject target);
    }

    /// <summary>
    /// 反発パッド
    /// </summary>
    public class ReboundPad : AreaItemBase, IReboundPad
    {
        [SerializeField] Data.ReboundPad data;

        void OnCollisionEnter2D(Collision2D col)
        {
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            ApplyRebound(rb, col.gameObject);
        }

        /// <summary>
        /// 反発を適用する
        /// </summary>
        /// <param name="target">衝突した相手</param>
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