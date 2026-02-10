using Constants;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    public static AreaManager I { get; private set; }
    [SerializeField] AreaDatabase areaDatabase;    public AreaDatabase AreaDatabase => areaDatabase;
    public AreaData CurrentAreaData { get; private set; }

    void Awake()
    {
        if(I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentAreaData(int index)
    {
        if(index < 0 || index >= areaDatabase.GetAllAreas().Length)
        {
            Debug.LogError("Invalid area index: " + index);
            return;
        }

        CurrentAreaData = areaDatabase.GetAllAreas()[index];
    }

    /// <summary>
    /// 文化圏レベルに応じたステータスパラメーターを取得する
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    public float GetStatusParamByCultureLevel(ENUM.StatusType type, float baseValue)
    {
        switch(type)
        {
            case ENUM.StatusType.Speed:
            case ENUM.StatusType.Power:
            case ENUM.StatusType.Guard:
            case ENUM.StatusType.Stamina:
                return baseValue * AreaBuild.GetCultureLevelMultiplier(CurrentAreaData.Build.cultureLevel);
            default:
                return baseValue;
        }
    }
}