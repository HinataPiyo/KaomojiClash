using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("壁の内側に生成できる範囲")]
    [SerializeField] Vector2 spawnArea;

    [SerializeField] GameObject waveDataUIPrefab;
    public Vector2 SpawnArea => spawnArea;
    public Vector2 CenterPosition => transform.position;

    [Header("壁の内側に設置可能な位置一覧")]
    [SerializeField] Transform[] canSetPositions;       public Transform[] CanSetPositions => canSetPositions;
    [Header("アリーナに設置するアイテム設定データ")]
    [SerializeField] ArenaItemSettingData arenaItemSettingData;
    public ArenaItemSettingData ArenaItemSettingData => arenaItemSettingData;

    List<GameObject> spawnedItems = new List<GameObject>();

    /// <summary>
    /// アリーナに設置するアイテムを生成する
    /// </summary>
    public void CreateArenaItem()
    {
        for(int ii = 0; ii < arenaItemSettingData.ArenaItemDatas.Count; ii++)
        {
            ArenaItemSettingData.Entry entry = arenaItemSettingData.ArenaItemDatas[ii];
            if(entry.itemData == null) continue;
            ArenaItemData itemData = entry.itemData;
            int boxNumber = entry.settingBoxNumber;

            Vector2 spawnPos = CanSetPositions[boxNumber].position;
            GameObject obj = Instantiate(itemData.Item_Prefab, spawnPos, Quaternion.identity);
            spawnedItems.Add(obj);
            Context.I.TR_Params.ApplyDecMoney(itemData.GetPrice());      // TotalResult用のデータに加算
            itemData.ApplyUsageCountUp();       // 使用回数を増加
        }
    }

    /// <summary>
    /// アリーナに設置したアイテムを全て破棄する
    /// </summary>
    public void ClearArenaItems()
    {
        for(int ii = 0; ii < spawnedItems.Count; ii++)
        {
            Destroy(spawnedItems[ii]);
        }
        spawnedItems.Clear();
    }

    /// <summary>
    /// アリーナに設置するアイテムデータを追加する
    /// ButtonUIが押下された時に呼ぶ
    /// </summary>
    /// <param name="itemData">データSO</param>
    /// <param name="settingBoxNumber">設置するBoxのナンバー</param>
    public void AddArenaItemData(ArenaItemData itemData, int settingBoxNumber)
    {
        arenaItemSettingData.AddArenaItemData(itemData, settingBoxNumber);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, (Vector3)spawnArea);
    }
}