using System;
using TMPro;
using UnityEngine;

public class Mental : MonoBehaviour
{
    [SerializeField] CharacterData data;
    [SerializeField] float currentMental;
    [SerializeField] int currentSeparate;
    TextMeshPro kaomoji;
    CharacterDieText dieText;

    void Awake()
    {
        dieText = GetComponent<CharacterDieText>();
        kaomoji = GetComponentInChildren<TextMeshPro>();
        currentMental = data.Status.maxMental;
        currentSeparate = data.Status.maxSeparate;     // テスト用に3回分離可能に設定
    }

    public void TakeDamage(float damage)
    {
        currentMental -= damage;
        
        WorldCanvasManager.I.ShowDamageText(transform.position, damage);
        
        if (currentMental <= 0)
        {
            CameraShake.I.ApplyShake(3f, 1.5f, 0.2f);
            if(currentSeparate > 0)
            {
                currentSeparate--;
                currentMental = data.Status.maxMental * ((float)(currentSeparate + 1) / (data.Status.maxSeparate + 1));   // 分離時は精神力を割合で回復（整数除算を防ぐため float にキャスト）
                if(currentMental < 1f) currentMental = 1f;    // 最低1は確保
                // 分離エフェクトなどをここで実行可能
                // 注意: CharacterDieText に SetSeparateText が存在しないため既存の SetText を呼ぶ
                dieText.SetText(kaomoji.text);
                return;
            }

            Die();
            return;
        }

        CameraShake.I.ApplyShake(1.5f, 1.5f, 0.2f);
    }

    void Die()
    {
        dieText.SetText(kaomoji.text);
        // 死亡処理（例: オブジェクトの破壊、アニメーションの再生など）
        Destroy(gameObject);
    }
}