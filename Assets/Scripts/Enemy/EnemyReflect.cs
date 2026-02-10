using Constants.Global;
using UnityEngine;
using ENUM;
using NUnit.Framework.Constraints;

public class EnemyReflect : Reflect, IEnemyInitialize
{
    EnemyData e_Data;
    public Difficulty dif { get; private set; }

    public void EnemyInitialize(EnemyData data, Difficulty dif)
    {
        e_Data = data;
        this.dif = dif;
    }

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

            Rigidbody2D otherRb = col.collider.GetComponent<Rigidbody2D>();

            // 相手より自分のほうが明確に速い場合のみダメージを与える
            if (CanApplyDamage(otherRb))
            {
                Mental player = col.collider.GetComponent<Mental>();
                float power = AreaManager.I.GetStatusParamByCultureLevel(StatusType.Power, e_Data.Status.power)
                            * Calculation.GetDifficultyRate(dif);

                if(!Context.I.IsPlayerArive()) return;
                player?.TakeDamage(power);
                AudioManager.I.PlaySEReflect();
            }
        }
    }

    protected override bool CanApplyDamage(Rigidbody2D otherRb)
    {
        // 相手より自分のほうが明確に速い場合のみダメージを与える
        return otherRb != null && rb.linearVelocity.sqrMagnitude > otherRb.linearVelocity.sqrMagnitude * 1.2f;
    }
}