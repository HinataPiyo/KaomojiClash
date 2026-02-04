using TMPro;
using UnityEngine;

public abstract class Mental : MonoBehaviour
{
    protected CharacterData data;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected int currentMental;
    protected TextMeshPro kaomoji;
    protected CharacterDieEffect dieEffect;
    protected float maxHealth;
    Movement movement;

    void Awake()
    {
        kaomoji = GetComponentInChildren<TextMeshPro>();
        dieEffect = GetComponent<CharacterDieEffect>();
        movement = GetComponent<Movement>();
    }

    /// <summary>
    /// この関数はPlayerApplyKaomojiから実行する
    /// </summary>
    /// <param name="stamina"></param>
    /// <param name="data"></param>
    public virtual void Initialize(float stamina, CharacterData data)
    {
        this.data = data;
        maxHealth = data.Status.health * (1f + stamina);
        currentHealth = maxHealth;
        currentMental = data.Status.mentalData.maxMental;
        Debug.Log("stamina: " + stamina + ", maxHealth: " + maxHealth);
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