using UnityEngine;

public class PlayerMental : Mental, IHeal
{
    StatusUIControl statusUI;

    public float GetMaxHealthAmount => maxHealth;

    void Awake()
    {
        statusUI = FindAnyObjectByType<StatusUIControl>();
    }

    public void Initialize(PlayerData data)
    {
        this.data = data;
        maxHealth = Context.I.GetPlayerStamina();
        currentHealth = maxHealth;
        currentMental = data.Status.mentalData.maxMental;

        statusUI.UpdateMental(data.Status.mentalData.maxMental);
        statusUI.SetMaxHealth(maxHealth);
    }

    public override void TakeDamage(float damage)
    {
        float guard = Context.I.GetPlayerGuard();
        float finalDamage = Constants.DamageCalc.CalcTakenDamage(damage, guard);
        Debug.Log($"Player TakeDamage: {damage} with Guard: {guard} => Reduced Damage: {finalDamage}");
        currentHealth -= finalDamage;

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

        Context.I.StageFailed();    // ゲームオーバー処理を呼び出す
    }

    /// <summary>
    /// 回復処理
    /// </summary>
    /// <param name="amount">回復量</param>
    public void Heal(float amount)
    {
        currentHealth += amount;
        WorldCanvasManager.I.ShowHealText(transform.position, amount);
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        statusUI.UpdateHealth(currentHealth);
    }
}