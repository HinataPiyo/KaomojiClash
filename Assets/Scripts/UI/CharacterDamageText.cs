using TMPro;
using UnityEngine;

public class CharacterDamageText : MonoBehaviour
{
    TextMeshProUGUI mesh;

    void Awake()
    {
        mesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// ダメージ数値を設定
    /// </summary>
    /// <param name="damage"></param>
    public void SetDamage(float damage)
    {
        mesh.text = damage.ToString("F2");
    }
}
