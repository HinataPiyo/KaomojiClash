using UnityEngine;

public class WorldCanvasBackManager : MonoBehaviour
{
    public static WorldCanvasBackManager I { get; private set; }
    

    [Header("WaveDataを表示するUI")]
    [SerializeField] GameObject waveDataUIPrefab;

    void Awake()
    {
        if (I == null)
        {
            I = this;
        }
    }


    public WaveDataUIControl CreateWaveDataUI(Vector2 position)
    {
        GameObject obj = Instantiate(waveDataUIPrefab, position, Quaternion.identity, transform);
        WaveDataUIControl waveDataUI = obj.GetComponent<WaveDataUIControl>();
        return waveDataUI;
    }
    
}