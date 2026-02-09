using UnityEngine;

[CreateAssetMenu(fileName = "Shunsoku", menuName = "SkillTags/Shunsoku")]
public class Shunsoku : SkillTag
{
    public override string GetDescription(int stackCount)
    {
        switch(stackCount)
        {
            case 1:
            case 2:
            case 3:
                return $"基礎スピードを{GetStatusModifier(stackCount).add}上昇させる。";
        }

        return string.Empty;
    }
}