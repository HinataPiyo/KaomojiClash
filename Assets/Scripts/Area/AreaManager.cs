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
}