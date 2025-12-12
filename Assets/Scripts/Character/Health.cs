using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 10f;
    [SerializeField] float currentHealth;
    TextMeshPro kaomoji;
    CharacterDieText dieText;

    void Awake()
    {
        dieText = GetComponent<CharacterDieText>();
        kaomoji = GetComponentInChildren<TextMeshPro>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        dieText.SetText(kaomoji.text);
        // 死亡処理（例: オブジェクトの破壊、アニメーションの再生など）
        Destroy(gameObject);
    }
}