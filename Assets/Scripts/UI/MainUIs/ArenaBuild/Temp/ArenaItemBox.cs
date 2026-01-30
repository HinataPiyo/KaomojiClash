namespace UI.ArenaBuild.Template
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class ArenaItemBox : MonoBehaviour
    {
        [SerializeField] Button button;     public Button Button => button;
        [SerializeField] Image icon;
        public ArenaItemData ItemData { get; private set; }

        public void Initialize(ArenaItemData data)
        {
            SetData(data);
        }

        /// <summary>
        /// アリーナアイテムをセット
        /// </summary>
        public void SetData(ArenaItemData data)
        {
            if(data == null) 
            {
                Remove();
                return;
            }

            icon.enabled = true;
            icon.sprite = data.Item_Icon;
            ItemData = data;
        }

        /// <summary>
        /// アリーナアイテムを削除
        /// </summary>
        public void Remove()
        {
            // icon.enabled = false;
            icon.sprite = null;
            ItemData = null;
        }

        public void IsIntaractable(bool isIntaractable)
        {
            button.interactable = isIntaractable;
            Color tempColor = new Color();
            if(!isIntaractable)
            {
                // 非操作可能時はアイコンを半透明にする
                tempColor = Color.gray;
                tempColor.a = 0.5f;
                icon.color = tempColor;
            }
            else
            {
                // 操作可能時はアイコンを通常表示にする
                tempColor = Color.white;
                tempColor.a = 1f;
                icon.color = tempColor;
            }
        }
    }
}