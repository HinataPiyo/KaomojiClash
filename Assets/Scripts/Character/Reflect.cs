using UnityEngine;

public class Reflect : MonoBehaviour
{
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // 敵と衝突した?
        if (col.collider.CompareTag("Enemy"))
        {
            // 現在の速度
            Vector2 v = rb.linearVelocity;

            // 衝突点の法線（最も信頼度が高い）
            Vector2 n = col.contacts[0].normal;

            // 反射ベクトルを計算
            Vector2 reflected = Vector2.Reflect(v, n);

            // 速度の大きさは維持したまま向きだけ変更
            rb.linearVelocity = reflected.normalized * v.magnitude;
            Health enemyHealth = col.collider.GetComponent<Health>();
            enemyHealth.TakeDamage(5f);
        }
    }
}
