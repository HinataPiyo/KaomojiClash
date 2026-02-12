using UnityEngine;
using Constants;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AreaData", menuName = "Arena/AreaData")]
public class AreaData : ScriptableObject
{
    [Header("基本情報")]
    [SerializeField] string areaName = "新規エリア";
    public string AreaName => areaName;

    [SerializeField] AreaBuild area = new AreaBuild(); // 初期化を追加
    public AreaBuild Build => area;

    [SerializeField] bool isClear;
    public bool IsClear => isClear;

    /// <summary>
    /// 初期化時に呼ばれる
    /// </summary>
    private void OnEnable()
    {
        // AreaBuildがnullの場合は初期化
        if (area == null)
        {
            area = new AreaBuild();
        }
        
        // spawnConfigがnullの場合は初期化
        if (area.spawnConfig == null)
        {
            area.spawnConfig = new EnemySpawnConfig();
        }
    }

    /// <summary>
    /// クリア済みにする
    /// </summary>
    public void ChangeClear() => isClear = true;

    /// <summary>
    /// クリア状態をリセット
    /// </summary>
    public void ResetClear() => isClear = false;

    /// <summary>
    /// エリア名を設定
    /// </summary>
    public void SetAreaName(string name) => areaName = name;
}