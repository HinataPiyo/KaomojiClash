using System;
using TMPro;
using UnityEngine;

public abstract class Mental : MonoBehaviour
{
    [SerializeField] protected CharacterData data;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected int currentMental;
    protected TextMeshPro kaomoji;
    protected CharacterDieEffect dieEffect;

    void Awake()
    {
        kaomoji = GetComponentInChildren<TextMeshPro>();
        dieEffect = GetComponent<CharacterDieEffect>();
        currentHealth = data.Status.maxHealth;
        currentMental = data.Status.maxMental;     // テスト用に3回分離可能に設定
    }

    public abstract void TakeDamage(float damage);

    protected virtual void Die()
    {
        dieEffect.SetText(kaomoji.text);
        // 死亡処理（例: オブジェクトの破壊、アニメーションの再生など）
        Destroy(gameObject);
    }
}