using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArenaItemSettingData", menuName = "ArenaItemSettingData", order = 0)]
public class ArenaItemSettingData : ScriptableObject
{
    [SerializeField] int maxSetItemCount = 1;   public int MaxSetItemCount => maxSetItemCount;      // 最大設定数(2個まで)
    public class Entry
    {
        public ArenaItemData itemData;
        public int settingBoxNumber;        // 設置するBoxの番号
    }

    public List<Entry> ArenaItemDatas { get; private set; } = new List<Entry>();

    public int GetUsageFee()
    {
        int totalPrice = 0;
        foreach(Entry entry in ArenaItemDatas)
        {
            if(entry.itemData != null)
            {
                totalPrice += entry.itemData.GetPrice();
            }
        }

        return totalPrice;
    }


    /// <summary>
    /// アリーナに設置するアイテムデータを追加する
    /// </summary>
    /// <param name="itemData">データSO</param>
    /// <param name="settingBoxNumber">設置するBoxのナンバー</param>
    public void AddArenaItemData(ArenaItemData itemData, int settingBoxNumber)
    {
        if(itemData == null) return;

        // 同種のアイテムが存在した場合のUIの更新をしなければならない
        // bool isSameKind = CheckSameKindItems(itemData, settingBoxNumber);
        bool isAlreadyInBox = CheckAlreadyInBox(itemData, settingBoxNumber);

        if(/*!isSameKind && */!isAlreadyInBox)
        {
            // 新規追加
            Entry newEntry = new Entry()
            {
                itemData = itemData,
                settingBoxNumber = settingBoxNumber
            };

            ArenaItemDatas.Add(newEntry);
        }
    }

    /// <summary>
    /// 最大設定数に達しているかチェック
    /// </summary>
    public bool CheckMaxSettingItems()
    {
        int count = 0;
        foreach(Entry entry in ArenaItemDatas)
        {
            if(entry.itemData != null)
            {
                count++;
                if(count >= MaxSetItemCount)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 同じ種類のアイテムが複数設定されていないかチェックする
    /// </summary>
    public bool CheckSameKindItems(ArenaItemData itemData, int newSettingBoxNumber)
    {
        // 同じ種類のアイテムが複数設定されていないかチェックする
        Entry targetBoxEntry = null;

        foreach(Entry entry in ArenaItemDatas)
        {
            // 同じアイテムがすでにあれば削除
            if(entry.itemData == itemData)
            {
                entry.itemData = null;
            }

            // これから設定するBoxを覚えておく
            if(entry.settingBoxNumber == newSettingBoxNumber)
            {
                targetBoxEntry = entry;
            }
        }

        if(targetBoxEntry != null)
        {
            targetBoxEntry.itemData = itemData;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 既に同じBoxナンバーにデータが存在するかチェックし、存在する場合は更新する
    /// </summary>
    public bool CheckAlreadyInBox(ArenaItemData itemData, int settingBoxNumber)
    {
        foreach(Entry entry in ArenaItemDatas)
        {
            // 既に同じBoxナンバーが存在する場合は更新する
            if(entry.settingBoxNumber == settingBoxNumber)
            {
                entry.itemData = itemData;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 指定したBoxナンバーのデータを削除する
    /// </summary>
    public void RemoveDataByBoxNumber(int boxNumber)
    {
        foreach(Entry entry in ArenaItemDatas)
        {
            if(entry.settingBoxNumber == boxNumber)
            {
                entry.itemData = null;
                break;
            }
        }
    }

    /// <summary>
    /// 設定されているアイテムの値段の合計値を取得する
    /// </summary>
    /// <returns></returns>
    public int CalculateTotalPrice()
    {
        int totalPrice = 0;
        foreach(Entry entry in ArenaItemDatas)
        {
            if(entry.itemData != null)
            {
                totalPrice += entry.itemData.GetPrice();
            }
        }

        Debug.Log("CalculateTotalPrice: " + totalPrice);
        return totalPrice;
    }

    /// <summary>
    /// 現在設定されているアイテム数を取得
    /// </summary>
    public int GetCurrentSetItemCount()
    {
        int count = 0;
        foreach(Entry entry in ArenaItemDatas)
        {
            if(entry.itemData != null)
            {
                count++;
            }
        }

        return count;
    }
    
}