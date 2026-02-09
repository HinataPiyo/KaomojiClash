using TMPro;
using UnityEngine;
using Constants;

public class PlayerApplyKaomoji : MonoBehaviour
{
    [SerializeField] TextMeshPro faceText;
    [SerializeField] CapsuleCollider2D col;

    PlayerData data;
    [SerializeField] PlayerUpgradeService upgradeService;
    string current_Kaomoji;     // 現在の顔文字

    public Status.Params BaseStatus { get; private set; } = new Status.Params();
    public Status.Params GetUpgradedStatus() => upgradeService.UpgradedStatus;
    public void RefreshUpgradedStatus() => upgradeService.RecalculateUpgrades();

    public void Initialize(PlayerData data)
    {
        this.data = data;
        KAOMOJI K = data.Kaomoji;
        upgradeService.Initialize(data, K.GetAllSkillTags());
        current_Kaomoji = K.BuildKaomoji(data.Status.mentalData);
        faceText.text = current_Kaomoji;
        SetColliderSize();
        GetComponent<PlayerMental>().Initialize(data);
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