using UnityEngine;

[CreateAssetMenu(fileName = "MentalData", menuName = "MentalData", order = 0)]
public class MentalData : ScriptableObject
{
    // 精神強度など精神に関するデータ
    public string faceline = "()";
    public new string name = "括弧";
    public int maxMental = 0;

    public string GetEffectDescription()
    {
        return $"精神強度が{maxMental}上昇する。";
    }

    public static string GetConditionBodyByLevel(int level)
    {
        switch(level)
        {
            case 1:
                return "手を装備している。";
            case 2:
                return "装飾1を装備している。";
            case 3:
                return "装飾2を装備している。";
            default:
                return "";
        }
    }
}