using TMPro;
using UnityEngine;

public class CharacterDamageText : MonoBehaviour
{
    TextMeshProUGUI mesh;

    void Awake()
    {
        mesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetValueWithColor(float val, Color32 color)
    {
        mesh.color = color;
        mesh.text = val.ToString("F0");
    }

    public void SetValue(float val)
    {
        mesh.text = val.ToString("F0");
    }
}
