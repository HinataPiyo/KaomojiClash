using UnityEngine;

[CreateAssetMenu(fileName = "AreaDatabase", menuName = "Database/AreaDatabase")]
public class AreaDatabase : ScriptableObject
{
    /// <summary>
    /// Areaのカウントが増えていくにつれ文化圏レベルが上昇する
    /// </summary>
    [SerializeField] AreaData[] areas;
    public AreaData[] GetAllAreas() => areas;
}