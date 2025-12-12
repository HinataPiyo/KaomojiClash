using UnityEngine;

public class EnemyReflect : Reflect
{
    protected override void OnCollisionEnter2D(Collision2D col)
    {
        // 敵と衝突した
        if (col.collider.CompareTag("Player"))
        {
            if (!CanReflection())
            {
                return;
            }

            Reflection(col);        // 反射
            Mental enemyHealth = col.collider.GetComponent<Mental>();
            enemyHealth.TakeDamage(1f);
        }
    }
}