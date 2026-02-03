using UnityEngine;

public class PlayerMental : Mental 
{
    StatusUIControl statusUI;

    void Start()
    {
        statusUI = FindAnyObjectByType<StatusUIControl>();
        statusUI.UpdateMental(data.Status.mentalData.maxMental);
        statusUI.SetMaxHealth(data.Status.health * (1f + Context.I.PlayerData.Kaomoji.Stamina));
    }

    public override void TakeDamage(float damage)
    {
        float reductDamage = damage * (1f - Context.I.PlayerData.Kaomoji.Guard);
        float finalDamage = Mathf.Max(1f, reductDamage); // 最低1ダメージ保証
        currentHealth -= finalDamage;     // 最低1ダメージ保証

        WorldCanvasManager.I.ShowDamageText(transform.position, finalDamage, Color.red);
        statusUI.UpdateHealth(currentHealth);
        GlobalVolumeManager.I.HitFlashEffect();
        CameraZoom.I.ApplyZoom(2.5f);
        
        if (currentHealth <= 0)
        {
            CameraShake.I.ApplyShake(3f, 1.5f, 0.2f);
            if(currentMental > 0)
            {
                currentMental--;
                statusUI.UpdateMental(currentMental);
                currentHealth = data.Status.health * ((float)(currentMental + 1) / (data.Status.mentalData.maxMental + 1));   // 分離時は精神力を割合で回復（整数除算を防ぐため float にキャスト）
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