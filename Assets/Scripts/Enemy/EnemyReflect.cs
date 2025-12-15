using UnityEngine;

public class EnemyReflect : Reflect
{
    protected override void OnCollisionEnter2D(Collision2D col)
    {
        // 敵と衝突した
        if (col.collider.CompareTag("Player"))
        {
            if (!CanReflection())       // 反射できない状態なら何もしない
            {
                return;
            }

            Reflection(col);        // 反射
            Mental player = col.collider.GetComponent<Mental>();
            player.TakeDamage(1f);
        }
    }
}