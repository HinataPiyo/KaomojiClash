using UnityEngine;

public class WorldCanvasManager : MonoBehaviour
{
    public static WorldCanvasManager I { get; private set; }
    [Header("ダメージテキスト")]
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] Vector2 damageTextOffset;

    [Header("射撃方向矢印")]
    [SerializeField] GameObject arrowPrefab;

    void Awake()
    {
        if(I == null)
        {
            I = this;
        }
    }

    public void ShowDamageText(Vector3 position, float damage)
    {
        Vector2 newPos = position + (Vector3)damageTextOffset;
        GameObject obj = Instantiate(damageTextPrefab, newPos, Quaternion.identity, transform);
        CharacterDamageText damageText = obj.GetComponent<CharacterDamageText>();
        damageText.SetDamage(damage);
    }

    public CharacterShootDirectionArrow CreateShootDirectionArrow(Vector3 position)
    {
        GameObject obj = Instantiate(arrowPrefab, position, Quaternion.identity, transform);
        CharacterShootDirectionArrow arrow = obj.GetComponent<CharacterShootDirectionArrow>();
        return arrow;
    }

    public void ShowShootDirectionArrow(CharacterShootDirectionArrow arrow, Vector3 position, Vector2 dir, float power = 1f)
    {
        arrow.transform.position = position;
        arrow.UpdateArrow(dir, power);
    }
}
