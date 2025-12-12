using UnityEngine;

public abstract class Reflect : MonoBehaviour
{
    protected Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected abstract void OnCollisionEnter2D(Collision2D col);
    protected void Reflection(Collision2D col)
    {
        // 現在の速度
        Vector2 v = rb.linearVelocity;

        // 衝突点の法線（最も信頼度が高い）
        Vector2 n = col.contacts[0].normal;

        // 反射ベクトルを計算
        Vector2 reflected = Vector2.Reflect(v, n);

        // 速度の大きさは維持したまま向きだけ変更
        rb.linearVelocity = reflected.normalized * v.magnitude;
    }

    protected bool CanReflection()
    {
        return rb.linearVelocity.sqrMagnitude >= 1f;
    }
}
