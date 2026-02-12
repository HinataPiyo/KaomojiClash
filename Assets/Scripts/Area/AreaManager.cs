using Constants;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class AreaManager : MonoBehaviour
{
    public static AreaManager I { get; private set; }

    private AreaData[] cachedAreas;
    public AreaData[] AreaDatas => cachedAreas;

    public AreaData CurrentAreaData { get; private set; }

    void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
            LoadAllAreas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Resourcesフォルダから全てのAreaDataを読み込み
    /// </summary>
    private void LoadAllAreas()
    {
        Debug.Log("=== AreaManager: エリア読み込み開始 ===");

        AreaData[] loadedAreas = Resources.LoadAll<AreaData>("Areas");

        Debug.Log($"Resources/Areas から {loadedAreas.Length} 個のエリアを読み込みました");

        if (loadedAreas.Length == 0)
        {
            Debug.LogWarning("エリアが見つかりません！Assets/Resources/Areas/ フォルダにAreaDataを配置してください。");
            cachedAreas = new AreaData[0];
            return;
        }

        // nullチェックとバリデーション
        var validAreas = new List<AreaData>();
        foreach (var area in loadedAreas)
        {
            if (area != null && area.Build != null && area.Build.spawnConfig != null)
            {
                validAreas.Add(area);
                Debug.Log($"  ✓ {area.AreaName} (Lv{area.Build.cultureLevel})");
            }
            else
            {
                Debug.LogWarning($"  ✕ 無効なAreaDataをスキップしました: {(area != null ? area.name : "null")}");
            }
        }

        // 文化圏レベル順にソート
        cachedAreas = validAreas.OrderBy(a => a.Build.cultureLevel).ToArray();

        // デフォルトで最初のエリアを設定
        if (cachedAreas.Length > 0)
        {
            SetCurrentAreaData(0);
            Debug.Log($"現在のエリアを設定: {CurrentAreaData.AreaName}");
        }
        else
        {
            Debug.LogError("有効なエリアが1つもありません！");
        }

        Debug.Log("=== AreaManager: 読み込み完了 ===");
    }

    /// <summary>
    /// 現在のエリアを設定
    /// </summary>
    public void SetCurrentAreaData(int index)
    {
        if (index < 0 || index >= cachedAreas.Length)
        {
            Debug.LogError("Invalid area index: " + index);
            return;
        }

        CurrentAreaData = cachedAreas[index];
    }

    /// <summary>
    /// 文化圏レベルでエリアを検索して設定
    /// </summary>
    public void SetCurrentAreaDataByCultureLevel(int cultureLevel)
    {
        var area = cachedAreas.FirstOrDefault(a => a.Build.cultureLevel == cultureLevel);
        if (area != null)
        {
            CurrentAreaData = area;
        }
        else
        {
            Debug.LogWarning($"Area with culture level {cultureLevel} not found.");
        }
    }

    /// <summary>
    /// 文化圏レベルに応じたステータスパラメーターを取得する
    /// </summary>
    public float GetStatusParamByCultureLevel(ENUM.StatusType type, float baseValue)
    {
        float multiplier;

        switch (type)
        {
            case ENUM.StatusType.Speed:
                // スピードは緩やかに上昇（5%ずつ）
                multiplier = AreaBuild.GetCultureLevelMultiplier(CurrentAreaData.Build.cultureLevel, 0.05f);
                return baseValue * multiplier;

            case ENUM.StatusType.Power:
            case ENUM.StatusType.Guard:
            case ENUM.StatusType.Stamina:
                // その他は通常通り（15%ずつ）
                multiplier = AreaBuild.GetCultureLevelMultiplier(CurrentAreaData.Build.cultureLevel);
                return baseValue * multiplier;

            default:
                return baseValue;
        }
    }

    /// <summary>
    /// 文化圏レベルで解放済みかチェック
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool CheckIsClearedByCultureLevel(ENUM.KaomojiPartType type)
    {
        int level = AreaBuild.PartTypeToReleaseLevel[type];
        for (int i = 0; i < cachedAreas.Length; i++)
        {
            if (cachedAreas[i].Build.cultureLevel >= level)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// キャッシュをリロード（エディタ用）
    /// </summary>
    public void ReloadAreas()
    {
        LoadAllAreas();
    }
}