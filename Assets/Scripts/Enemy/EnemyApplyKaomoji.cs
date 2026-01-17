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
    [SerializeField] TextMeshPro level_Difficulty_Text;
    [SerializeField] SpriteRenderer minimapIcon;
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

    /// <summary>
    /// 半透明
    /// </summary>
    public void Translucent()
    {
        Color c = faceText.color;
        c.a = 0.5f;
        faceText.color = c;
    }

    /// <summary>
    /// 不透明
    /// </summary>
    public void Opaque()
    {
        Color c = faceText.color;
        c.a = 1f;
        faceText.color = c;
    }

    /// <summary>
    /// レベルと難易度を表示
    /// </summary>
    /// <param name="level">レベル</param>
    /// <param name="difficulty">難易度</param>
    public void SetLevelAndDifficultyText(int level, ENUM.Difficulty difficulty)
    {
        level_Difficulty_Text.color = Calculation.GetColorByDifficulty(difficulty);
        level_Difficulty_Text.text = $"Lv{level} {difficulty.ToString().ToUpper()}";
    }

    /// <summary>
    /// レベルと難易度表示テキストを非表示
    /// </summary>
    public void DisableLevelAndDifficulty()
    {
        level_Difficulty_Text.gameObject.SetActive(false);
    }

    public void EnableLevelAndDifficulty()
    {
        level_Difficulty_Text.gameObject.SetActive(true);
    }

    public void ChangeMinimapIconColor(ENUM.Difficulty difficulty)
    {
        minimapIcon.color = Calculation.GetColorByDifficulty(difficulty);
    }
}