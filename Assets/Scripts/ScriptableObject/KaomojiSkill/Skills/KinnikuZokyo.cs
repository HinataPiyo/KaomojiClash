using UnityEngine;

[CreateAssetMenu(fileName = "KinnikuZokyo", menuName = "SkillTags/KinnikuZokyo")]
public class KinnikuZokyo : SkillTag
{
    public override string GetDescription(int stackCount)
    {
        switch(stackCount)
        {
            case 1:
            case 2:
            case 3:
                return $"基礎パワーを{GetStatusModifier(stackCount).add}上昇させる。";
            case 4:
            case 5:
                return $"パワーを{GetStatusModifier(stackCount).mul}倍する。基礎パワーを{GetStatusModifier(stackCount).add}上昇する。";
        }

        return string.Empty;
    }
    
}