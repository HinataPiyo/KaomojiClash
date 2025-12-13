using TMPro;
using UnityEngine;

public class CharacterDieEffect : MonoBehaviour
{
    [SerializeField] GameObject dieTextPrefab;

    /// <summary>
    /// 渡された文字列を一文字ずつ分解して表示
    /// </summary>
    /// <param name="allText">顔文字</param>
    public void SetText(string allText)
    {
        char[] split = allText.ToCharArray();

        for(int ii = 0; ii < split.Length; ii++)
        {
            GameObject obj = Instantiate(dieTextPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            TextMeshPro mesh = obj.GetComponentInChildren<TextMeshPro>();

            mesh.text = split[ii].ToString();
            Impact(rb);
        }
    }

    public void SetSeparateText(string allText)
    {
        GameObject obj = Instantiate(dieTextPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        TextMeshPro mesh = obj.GetComponentInChildren<TextMeshPro>();
        mesh.text = allText;
        Impact(rb);
    }

    /// <summary>
    /// 文字にランダムな力を加える
    /// </summary>
    /// <param name="rb"></param>
    void Impact(Rigidbody2D rb)
    {
        float randomPower = Random.Range(0.8f, 3f);
        float randomDirX = Random.Range(-1f, 1f);
        float randomDirY = Random.Range(-1f, 1f);
        rb.AddForce(new Vector2(randomDirX, randomDirY) * randomPower, ForceMode2D.Impulse);
    }
}
