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
    public void SetDamage(float damage, Color32 color)
    {
        mesh.color = color;
        // mesh.text = damage.ToString("0.##");
        mesh.text = damage.ToString("F0");
    }
}
