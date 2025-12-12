using UnityEngine;

public class WorldCanvasManager : MonoBehaviour
{
    public static WorldCanvasManager I { get; private set; }
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] Vector2 damageTextOffset;

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
}
