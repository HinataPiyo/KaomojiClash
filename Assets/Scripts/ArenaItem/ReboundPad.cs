
using UnityEngine;

/// <summary>
/// 反発パッド
/// </summary>
public class ReboundPad : AreaItemBase
{
    const string ANIM_TRIGGER_PLAY = "Play";
    [SerializeField] Data.ArenaItem.ReboundPad data;
    Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        ApplyRebound(col.gameObject);
    }

    /// <summary>
    /// 反発を適用する
    /// </summary>
    /// <param name="target">衝突した相手</param>
    void ApplyRebound(GameObject target)
    {
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            Vector2 reboundDir = (target.transform.position - transform.position).normalized;
            float reboundForce = data.GetReboundForceByGrade();
            rb.AddForce(reboundDir * reboundForce, ForceMode2D.Impulse);
            anim.SetTrigger(ANIM_TRIGGER_PLAY);
        }
    }
}