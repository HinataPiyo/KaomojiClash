using UnityEngine;

[CreateAssetMenu(fileName = "AreaDatabase", menuName = "Database/AreaDatabase")]
public class AreaDatabase : ScriptableObject
{
    /// <summary>
    /// Areaのカウントが増えていくにつれ文化圏レベルが上昇する
    /// </summary>
    [SerializeField] AreaData[] areas;
    public AreaData[] GetAllAreas() => areas;

    public bool CheckIsClearedByCultureLevel(int cultureLevel)
    {
        int index = cultureLevel - 1;    // 文化圏レベルは1から始まるので-1してインデックスに変換
        if(index < 0 || index >= areas.Length)
        {
            return false;
        }
        return areas[index].IsClear;
    }
}