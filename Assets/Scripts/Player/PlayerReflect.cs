using UnityEngine;

[RequireComponent(typeof(Combo))]
public class PlayerReflect : Reflect
{
    [SerializeField] Combo combo;    
    protected override void OnCollisionEnter2D(Collision2D col)
    {
        // 敵と衝突した
        if (col.collider.CompareTag("Enemy"))
        {
            if (!CanReflection())       // 反射できない状態なら何もしない
            {
                Debug.Log("速度が小さいため反射しません");
                return;
            }

            Reflection(col);        // 反射

            Rigidbody2D otherRb = col.collider.GetComponent<Rigidbody2D>();
            // 相手より自分のほうが速い場合のみダメージを与える
            if (CanApplyDamage(otherRb))
            {
                Mental enemyHealth = col.collider.GetComponent<Mental>();
                combo.IncreaseCombo();
                float finalPower = data.Status.attackPower * combo.CurrentAmplifyer;
                enemyHealth.TakeDamage(finalPower);
                HitStop.I.StartHitStop();
            }
        }
    }
}