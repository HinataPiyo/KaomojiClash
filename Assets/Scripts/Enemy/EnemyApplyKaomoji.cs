using TMPro;
using UnityEngine;
using Constants.Global;

/// <summary>
/// 雑魚敵用（通常の顔文字を用いる際に使用）
/// ボスはPrefabで用意するため不要
/// </summary>
public class EnemyApplyKaomoji : MonoBehaviour, IEnemyInitialize
{
    [SerializeField] TextMeshPro faceText;
    [SerializeField] CapsuleCollider2D col;
    
    EnemyData data;

    public void EnemyInitialize(EnemyData data)
    {
        this.data = data;
        faceText.text = data.Kaomoji_Body;
        SetColliderSize();
    }

    /// <summary>
    /// コライダーサイズ設定
    /// </summary>
    void SetColliderSize()
    {
        Vector2 size = col.size;
        size.x = KAOMOJI.ColiderXSize * data.Kaomoji_Body.Length;
        col.size = size;
    }
}