using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combo))]
public class PlayerReflect : Reflect 
{
    [SerializeField] Combo combo;
    [SerializeField] PlayerUpgradeService upgradeService;

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
                float power = Context.I.GetPlayerPower();
                float finalPower = combo.GetPowerMultiplier(power);
                enemyHealth.TakeDamage(finalPower);
                HitStop.I.StartHitStop();
                AudioManager.I.PlaySEReflect();
            }
        }
        else if (col.collider.CompareTag("Wall"))
        {
            ApplaySkillTagEffects();     // 反射に関連するスキルタグの効果を適用
            AudioManager.I.PlaySEReflect();
        }
    }

    protected override bool CanApplyDamage(Rigidbody2D otherRb)
    {
        // 相手より自分のほうが速い場合のみダメージを与える
        return otherRb != null && rb.linearVelocity.sqrMagnitude > otherRb.linearVelocity.sqrMagnitude * 0.92f;
    }

    void ApplaySkillTagEffects()
    {
        // 反射に関連するスキルタグの効果を適用する処理をここに実装
        // 例: 反射ダメージ増加、反射後の無敵時間付与など

        List<SkillTag.Stack> skillTags = upgradeService.ConditionTags;

        foreach (SkillTag.Stack elem in skillTags)
        {
            if(elem.tag is ReboundBoost boost)        // RebounBoostがタグにある場合
            {
                float multiplier = boost.GetSpeedUpMultiplierByLevel(elem.stackCount);
                float duration = boost.GetDurationByLevel(elem.stackCount);

                upgradeService.StartSpeedUpEffect(duration, multiplier);        // 速度上昇効果を開始
            }
        }
    }
}