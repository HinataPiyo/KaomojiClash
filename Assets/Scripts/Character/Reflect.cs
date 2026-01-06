using UnityEngine;

public abstract class Reflect : MonoBehaviour, ICharacterInitialize
{
    protected Rigidbody2D rb;
    const float REFRECT_SPEED_BORDER = 2.2f;
    
    [SerializeField] GameObject hitEffectPrefab;
    protected CharacterData data;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(CharacterData data)
    {
        this.data = data;
    }

    protected abstract void OnCollisionEnter2D(Collision2D col);
    protected void Reflection(Collision2D col)
    {
        if(data == null) return;
        // 現在の速度
        Vector2 v = rb.linearVelocity;

        // 衝突点の法線（最も信頼度が高い）
        Vector2 n = col.contacts[0].normal;

        // 反射ベクトルを計算
        Vector2 reflected = Vector2.Reflect(v, n);

        // 速度の大きさは維持したまま向きだけ変更
        rb.linearVelocity = reflected.normalized * data.Status.reflectPower * v.magnitude ;

        // エフェクトを生成（反射方向に合わせて回転）
        Vector2 dir = reflected.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);
        Instantiate(hitEffectPrefab, col.contacts[0].point, rot);
    }

    protected bool CanReflection()
    {
        return rb.linearVelocity.sqrMagnitude >= REFRECT_SPEED_BORDER;
    }

    protected bool CanApplyDamage(Rigidbody2D otherRb)
    {
        // 相手より自分のほうが速い場合のみダメージを与える
        return otherRb != null && rb.linearVelocity.sqrMagnitude > otherRb.linearVelocity.sqrMagnitude;
    }
}
