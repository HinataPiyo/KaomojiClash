using System;
using Constants.Global;
using ENUM;
using UnityEngine;

public class EnemyMental : Mental, IEnemyInitialize
{
    EnemyData e_Data;
    public Difficulty dif { get; private set; }

    public void EnemyInitialize(EnemyData data, Difficulty dif)
    {
        e_Data = data;
        this.dif = dif;
        float helth = AreaManager.I.GetStatusParamByCultureLevel(StatusType.Stamina, data.Status.health)
                            * Calculation.GetDifficultyRate(dif);
        currentHealth = helth;
    }

    public override void TakeDamage(float damage)
    {
        float guard = AreaManager.I.GetStatusParamByCultureLevel(StatusType.Guard, e_Data.Status.guard)
                            * Calculation.GetDifficultyRate(dif);
        float reduct = damage - guard;
        currentHealth -= Mathf.Max(1f, reduct);     // 最低1ダメージ保証

        WorldCanvasManager.I.ShowDamageText(transform.position, damage, Color.yellow);
        
        if (currentHealth <= 0)
        {
            CameraShake.I.ApplyShake(3f, 1.5f, 0.2f);
            if(currentMental > 0)
            {
                currentMental--;
                currentHealth = data.Status.health * ((float)(currentMental + 1) / (data.Status.mentalData.maxMental + 1));   // 分離時は精神力を割合で回復（整数除算を防ぐため float にキャスト）
                if(currentHealth < 1f) currentHealth = 1f;    // 最低1は確保
                // 分離エフェクトなどをここで実行可能
                // 注意:CharacterDieText に SetSeparateText が存在しないため既存の SetText を呼ぶ
                dieEffect.SetSeparateText(e_Data.Kaomoji.BuildKaomoji(e_Data.Status.mentalData));
                return;
            }

            Die();
            return;
        }

        CameraShake.I.ApplyShake(1.5f, 1.5f, 0.2f);
    }

    protected override void Die()
    {
        GlobalVolumeManager.I.DieFlashEffect();
        WorldCanvasManager.I.CrashEffect(transform.position);
        CameraZoom.I.EnemyKilledZoom();
        BattleFlowManager.I.RemoveEnemy(transform);

        // 敵固有の死亡処理をここに追加可能
        base.Die();
    }
}