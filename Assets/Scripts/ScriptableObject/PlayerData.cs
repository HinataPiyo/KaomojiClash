using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
public class PlayerData : CharacterData 
{
    [Header("攻撃力をcomboの状態に応じて増幅する最大値"), SerializeField] 
    float strengthMaxAmplifyer = 1.5f;
    float strengthMinAmplifyer = 0.8f;
    public float StrengthMaxAmplifyer => strengthMaxAmplifyer;
    public float StrengthMinAmplifyer => strengthMinAmplifyer;
}