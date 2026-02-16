using UnityEngine;
using Constants;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Character/PlayerData")]
public class PlayerData : CharacterData 
{
    [Header("次に操作できるまでの時間")]
    public float landingCooldown = 0.25f;
    [SerializeField] KAOMOJI kaomoji;
    public KAOMOJI Kaomoji => kaomoji;
    [Header("攻撃力をcomboの状態に応じて増幅する最大値"), SerializeField] 
    float strengthMaxAmplifyer = 1.5f;
    float strengthMinAmplifyer = 0.8f;

    [Header("マウスドラッグからクリックで発射トリガーにするかどうか")]
    [SerializeField] bool isMouseClickDrag = true;

    public bool IsMouseClickDrag => isMouseClickDrag;
    public float StrengthMaxAmplifyer => strengthMaxAmplifyer;
    public float StrengthMinAmplifyer => strengthMinAmplifyer;
}