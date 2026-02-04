using Unity.VisualScripting;
using UnityEngine;

public class WorldCanvasManager : MonoBehaviour
{
    public static WorldCanvasManager I { get; private set; }
    [Header("ダメージテキスト")]
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] Vector2 damageTextOffset;
    [Header("回復テキスト")]
    [SerializeField] GameObject healTextPrefab;
    [SerializeField] Vector2 healTextOffset;

    [Header("射撃方向矢印")]
    [SerializeField] GameObject arrowPrefab;

    [Header("衝突したときのエフェクト")]
    [SerializeField] GameObject clashEffectPrefab;


    void Awake()
    {
        if(I == null)
        {
            I = this;
        }
    }

    /// <summary>
    /// ダメージテキストを表示
    /// </summary>
    public void ShowDamageText(Vector3 position, float damage, Color32 color)
    {
        Vector2 newPos = position + (Vector3)damageTextOffset;
        GameObject obj = Instantiate(damageTextPrefab, newPos, Quaternion.identity, transform);
        CharacterDamageText damageText = obj.GetComponent<CharacterDamageText>();
        damageText.SetValueWithColor(damage, color);
    }

    /// <summary>
    /// 回復テキストを表示
    /// </summary>
    public void ShowHealText(Vector3 position, float healAmount)
    {
        Vector2 newPos = position + (Vector3)healTextOffset;
        GameObject obj = Instantiate(healTextPrefab, newPos, Quaternion.identity, transform);
        CharacterDamageText healText = obj.GetComponent<CharacterDamageText>();
        healText.SetValue(healAmount);
    }

    /// <summary>
    /// 射撃方向矢印を作成
    /// </summary>
    public CharacterShootDirectionArrow CreateShootDirectionArrow(Vector3 position)
    {
        GameObject obj = Instantiate(arrowPrefab, position, Quaternion.identity, transform);
        CharacterShootDirectionArrow arrow = obj.GetComponent<CharacterShootDirectionArrow>();
        return arrow;
    }

    /// <summary>
    /// 射撃方向矢印を表示
    /// </summary>
    public void ShowShootDirectionArrow(CharacterShootDirectionArrow arrow, Vector3 position, Vector2 dir, float power = 1f)
    {
        arrow.transform.position = position;
        arrow.UpdateArrow(dir, power);
    }

    public void CrashEffect(Vector3 position)
    {
        Instantiate(clashEffectPrefab, position, Quaternion.identity, transform);
    }


}
