using UnityEngine;

public class PlayerReflect : Reflect
{
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
            Mental enemyHealth = col.collider.GetComponent<Mental>();
            enemyHealth.TakeDamage(3f);
        }
    }
}