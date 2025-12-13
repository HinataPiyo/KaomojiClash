using UnityEngine;

public class CharacterShootDirectionArrow : MonoBehaviour
{
    [SerializeField] Transform arrow;
    [SerializeField] float maxScale = 10f;
    public void UpdateArrow(Vector2 dir, float dragDis)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0f, 0f, angle);

        float scale = Mathf.Min(dragDis, maxScale);
        arrow.localScale = new Vector3(scale, arrow.localScale.y, 1f);
    }

    public void Del()
    {
        Destroy(gameObject);
    }
}