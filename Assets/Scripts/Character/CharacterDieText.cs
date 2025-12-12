using TMPro;
using UnityEngine;

public class CharacterDieText : MonoBehaviour
{
    [SerializeField] GameObject dieTextPrefab;

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

    void Impact(Rigidbody2D rb)
    {
        float randomPower = Random.Range(0.8f, 3f);
        float randomDirX = Random.Range(-1f, 1f);
        float randomDirY = Random.Range(-1f, 1f);
        rb.AddForce(new Vector2(randomDirX, randomDirY) * randomPower, ForceMode2D.Impulse);
    }
}
