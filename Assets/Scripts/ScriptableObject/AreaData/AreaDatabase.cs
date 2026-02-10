using UnityEngine;

[CreateAssetMenu(fileName = "AreaDatabase", menuName = "Database/AreaDatabase")]
public class AreaDatabase : ScriptableObject
{
    [System.Serializable]
    public class ReleaseCondition
    {
        public ENUM.KaomojiPartType type;
        public int requiredCount;
    }

    /// <summary>
    /// Areaのカウントが増えていくにつれ文化圏レベルが上昇する
    /// </summary>
    [SerializeField] AreaData[] areas;      public AreaData[] GetAllAreas() => areas;
    [SerializeField] ReleaseCondition[] releaseConditions; 
    

    public bool CheckIsClearedByCultureLevel(ENUM.KaomojiPartType type)
    {
        int index = GetReleaseRequiredByType(type) - 1;
        if(index < 0) return true;      // 必要数が1未満の場合は常に解放済みとする
        if(index >= areas.Length) return false;   // インデックスが範囲外の場合は解放されていないとみなす
        return areas[index].IsClear;
    }

    /// <summary>
    /// タイプ別の解放に必要な文化圏レベルを取得
    /// </summary>
    int GetReleaseRequiredByType(ENUM.KaomojiPartType type)
    {
        foreach(ReleaseCondition condition in releaseConditions)
        {
            if(condition.type == type)
            {
                return condition.requiredCount;
            }
        }
        return int.MaxValue;      // 見つからなかった場合は非常に大きな値を返す
    }
}