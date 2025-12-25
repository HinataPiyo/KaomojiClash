using UnityEngine;

public class PlayerMental : Mental 
{
    StatusUIControl statusUI;

    void Start()
    {
        statusUI = FindAnyObjectByType<StatusUIControl>();
        statusUI.UpdateMental(data.Kaomoji.mentalData.maxMental);
        statusUI.SetMaxHealth(data.Status.maxHealth);
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        WorldCanvasManager.I.ShowDamageText(transform.position, damage);
        statusUI.UpdateHealth(currentHealth);
        GlobalVolumeManager.I.SetHitEffect();
        
        if (currentHealth <= 0)
        {
            CameraShake.I.ApplyShake(3f, 1.5f, 0.2f);
            if(currentMental > 0)
            {
                currentMental--;
                statusUI.UpdateMental(currentMental);
                currentHealth = data.Status.maxHealth * ((float)(currentMental + 1) / (data.Kaomoji.mentalData.maxMental + 1));   // 分離時は精神力を割合で回復（整数除算を防ぐため float にキャスト）
                if(currentHealth < 1f) currentHealth = 1f;    // 最低1は確保
                // 分離エフェクトなどをここで実行可能
                // 注意:CharacterDieText に SetSeparateText が存在しないため既存の SetText を呼ぶ
                dieEffect?.SetSeparateText(kaomoji?.text);
                statusUI.UpdateHealth(currentHealth);
                AudioManager.I.PlaySE("MentalBreak");
                return;
            }

            Die();
            return;
        }

        CameraShake.I.ApplyShake(1.5f, 1.5f, 0.2f);
    }

    protected override void Die()
    {
        // プレイヤー固有の死亡処理をここに追加可能
        statusUI.UpdateHealth(currentHealth);
        base.Die();
    }
}