namespace UI.ArenaBuild
{
    using UnityEngine;
    using UI.ArenaBuild.Template;
    using System.Collections.Generic;

    public class ArenaItemBoxController : MonoBehaviour
    {
        [SerializeField] GameObject box_Prefab;
        [SerializeField] Wall wall;
        [SerializeField] ArenaBuildModulesController ctrl;

        List<ArenaItemBox> itemBoxes = new List<ArenaItemBox>();

        public void Initialize()
        {
            Clear();

            // 設置可能Boxの数だけ生成
            for(int ii = 0; ii < wall.CanSetPositions.Length; ii++)
            {
                Vector2 spawnPos = wall.CanSetPositions[ii].position;
                GameObject boxObj = Instantiate(box_Prefab, spawnPos, Quaternion.identity, transform);
                ArenaItemData itemData = null;

                // 既に設定されているデータを取得
                foreach(ArenaItemSettingData.Entry entry in wall.ArenaItemSettingData.ArenaItemDatas)
                {
                    // 設置Boxナンバーが一致するデータを探す
                    if(entry.settingBoxNumber == ii)
                    {
                        itemData = entry.itemData;
                        break;
                    }
                }

                ArenaItemBox box = boxObj.GetComponent<ArenaItemBox>();
                box.Initialize(itemData);

                itemBoxes.Add(box);
            }

            ButtonsOnClick();
            ButtonsSetIntaractable();       // ボタンの相互作用可能性を更新
        }

        /// <summary>
        /// 既存のBoxを削除
        /// </summary>
        void Clear()
        {
            ArenaItemBox[] box = GetComponentsInChildren<ArenaItemBox>();
            if(box != null && box.Length > 0)
            {
                foreach(ArenaItemBox b in box)
                {
                    Destroy(b.gameObject);
                }
            }

            itemBoxes.Clear();
        }

        void ButtonsOnClick()
        {
            foreach(ArenaItemBox box in itemBoxes)
            {
                ArenaItemSettingData settingData = wall.ArenaItemSettingData;
                box.Button.onClick.RemoveAllListeners();

                int boxNumber = itemBoxes.IndexOf(box);     // ボックスのナンバーを取得
                box.Button.onClick.AddListener(() =>
                {
                    if(ctrl.IsSetting)
                    {
                        settingData.AddArenaItemData(ctrl.SelectedArenaItemData, boxNumber);
                        box.SetData(ctrl.SelectedArenaItemData);
                        ctrl.ChangeIsSetting(false);            // 削除モードに変更
                        ctrl.SelectedArenaItemDataNull();       // 選択データをnullに戻す
                    }
                    else
                    {
                        box.Remove();
                        settingData.RemoveDataByBoxNumber(boxNumber);
                    }

                    // ボタンの相互作用可能性を更新
                    ButtonsSetIntaractable();
                });
            }
        }

        void ButtonsSetIntaractable()
        {
            bool isMax = wall.ArenaItemSettingData.CheckMaxSettingItems();
            foreach(ArenaItemBox box in itemBoxes)
            {
                if(isMax && box.ItemData == null)
                {
                    // 設定モードかつ最大数に達している場合は操作不可にする
                    box.IsIntaractable(false);
                }
                else if(box.ItemData != null)
                {
                    // すでにアイテムが設定されているBoxは常に操作可能にする
                    box.IsIntaractable(true);
                }
                else
                {
                    // それ以外は操作可能にする
                    box.IsIntaractable(true);
                }
            }
        }

        /// <summary>
        /// パネルの表示・非表示
        /// </summary>
        /// <param name="isEnable"></param>
        public void PanelIsEnable(bool isEnable)
        {
            gameObject.SetActive(isEnable);
        }

        void OnEnable()
        {
            Initialize();
        }
    }
}