using UnityEngine;
using Constants;

[CreateAssetMenu(fileName = "AreaData", menuName = "AreaData")]
public class AreaData : ScriptableObject
{
    [SerializeField] AreaBuild area;       public AreaBuild Build => area;
    [SerializeField] bool isClear;         public bool IsClear => isClear;

    /// <summary>
    /// クリア済みにする
    /// </summary>
    public void ChangeClear() => isClear = true;

    public void ResetClear() => isClear = false;
}