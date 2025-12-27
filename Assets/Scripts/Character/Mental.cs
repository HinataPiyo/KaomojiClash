using System;
using Constants.Global;
using TMPro;
using UnityEngine;

public abstract class Mental : MonoBehaviour
{
    protected CharacterData data;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected int currentMental;
    protected TextMeshPro kaomoji;
    protected CharacterDieEffect dieEffect;
    protected ApplyKaomoji totalStatus;

    Movement movement;

    void Awake()
    {
        kaomoji = GetComponentInChildren<TextMeshPro>();
        dieEffect = GetComponent<CharacterDieEffect>();
        movement = GetComponent<Movement>();
        totalStatus = GetComponent<ApplyKaomoji>();
    }

    /// <summary>
    /// この関数はApplyKaomojiから実行する
    /// </summary>
    /// <param name="stamina"></param>
    /// <param name="data"></param>
    public virtual void Initialize(float stamina, CharacterData data)
    {
        this.data = data;
        currentHealth = data.Status.maxHealth * (1f + stamina);
        currentMental = data.Kaomoji.mentalData.maxMental;     // テスト用に3回分離可能に設定
    }

    public abstract void TakeDamage(float damage);

    protected virtual void Die()
    {
        dieEffect.SetText(kaomoji.text);
        AudioManager.I.PlaySE("K.O.");
        // 矢印が残らないように破棄する
        if(movement.ShootDirectionArrow != null) movement.ShootDirectionArrow.Del();
        // 死亡処理（例: オブジェクトの破壊、アニメーションの再生など）
        Destroy(gameObject);
    }
}