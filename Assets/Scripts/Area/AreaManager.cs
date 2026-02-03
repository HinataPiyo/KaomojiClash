using Constants;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    public static AreaManager I { get; private set; }
    [SerializeField] AreaData[] areaDatas;    public AreaData[] AreaDatas => areaDatas;
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

        SetCurrentAreaData(0);
    }

    public void SetCurrentAreaData(int index)
    {
        if(index < 0 || index >= areaDatas.Length)
        {
            Debug.LogError("Invalid area index: " + index);
            return;
        }

        CurrentAreaData = areaDatas[index];
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