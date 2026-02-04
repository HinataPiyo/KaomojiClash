using UnityEngine;

[CreateAssetMenu(fileName = "AreaDatabase", menuName = "Database/AreaDatabase")]
public class AreaDatabase : ScriptableObject
{
    [System.Serializable]
    public class Record
    {
        public bool isCleared;
        public AreaData data;
    }
    /// <summary>
    /// Areaのカウントが増えていくにつれ文化圏レベルが上昇する
    /// </summary>
    [SerializeField] Record[] areas;
    public Record[] GetAllAreas() => areas;

    public bool CheckIsClearedByCultureLevel(int cultureLevel)
    {
        if(cultureLevel < 0 || cultureLevel >= areas.Length)
        {
            return false;
        }
        return areas[cultureLevel].isCleared;
    }
}