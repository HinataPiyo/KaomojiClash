using UnityEngine;
using ArenaItem;
public class WallLine : MonoBehaviour, IReboundPad
{
    [SerializeField] float reboundForce = 10f;

    void OnCollisionEnter2D(Collision2D col)
    {
        Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
        ApplyRebound(rb, col);
    }

    /// <summary>
    /// 反発を適用する
    /// </summary>
    /// <param name="target">衝突した相手</param>
    public void ApplyRebound(Rigidbody2D rb, Collision2D target)
    {
        if (rb == null) return;

        Transform centerTransform = transform.parent != null ? transform.parent : transform;
        Vector2 reboundDir = ((Vector2)centerTransform.position - rb.position).normalized;
        rb.AddForce(reboundDir * reboundForce, ForceMode2D.Impulse);
    }
}