using TMPro;
using UnityEngine;
using Constants;

public class PlayerApplyKaomoji : MonoBehaviour
{
    [SerializeField] TextMeshPro faceText;
    [SerializeField] CapsuleCollider2D col;

    PlayerData data;
    string current_Kaomoji;     // 現在の顔文字

    public void Initialize(PlayerData data)
    {
        this.data = data;
        KAOMOJI K = data.Kaomoji;
        K.UpdateTotalParameter();
        current_Kaomoji = K.BuildKaomoji(data.Status.mentalData);
        faceText.text = current_Kaomoji;
        SetColliderSize();
        GetComponent<Mental>().Initialize(K.Stamina, data);
    }

    /// <summary>
    /// コライダーサイズ設定
    /// </summary>
    void SetColliderSize()
    {
        KAOMOJI K = data.Kaomoji;
        if(K == null) return;

        Vector2 size = col.size;
        size.x = KAOMOJI.ColiderXSize * current_Kaomoji.Length;
        col.size = size;
    }
}