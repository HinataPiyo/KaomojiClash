namespace UI.ArenaBuild.Template
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Constants.Global;
    using UnityEngine;
    using UnityEngine.UIElements;
    using UnityEngine.XR;

    public class ArenaItem
    {
        Label name;
        Label discription;
        Label price;
        VisualElement icon;
        DropdownField gradeType;
        ProgressBar duplicateRate;

        Button button;
        ArenaItemData itemData;

        /// <summary>
        /// ボタンがクリックされた時の処理を外部から登録する]
        /// インスタンス化後に呼び出すこと
        /// </summary>
        /// <param name="action">ボタンが押された時に処理</param>
        public void ButtonClickedAction(System.Action action) => button.clicked += () => action();

        /// <summary>
        /// コンストラクタ
        /// 初期化処理
        /// </summary>
        /// <param name="root">生成されたテンプレートのRoot</param>
        /// <param name="itemData">UIに保持させておくItemData</param>
        public ArenaItem(VisualElement root, ArenaItemData itemData)
        {
            this.itemData = itemData;
            name = root.Q<Label>("name-value");
            discription = root.Q<Label>("discription-value");
            price = root.Q<Label>("price-value");
            icon = root.Q<VisualElement>("icon");
            gradeType = root.Q<DropdownField>();
            duplicateRate = root.Q<ProgressBar>();
            button = root.Q<Button>();

            gradeType.choices = Calculation.GradeTypeName.Values.ToList();
            gradeType.value = Calculation.GradeTypeName.First(p => p.Key == itemData.GradeType).Value;
            gradeType.RegisterValueChangedCallback(OnDropdownChanged);      // Dropdownが変更された時の処理登録
            UpdateUI();
        }

        // クリックされた時の処理
        // Dropdownが変更された時の処理
        // Progreaabarが変更された時の処理

        /// <summary>
        /// GradeTypeが変更された時の処理
        /// </summary>
        /// <param name="evt">選択されたGradeTypeの文字列</param>
        void OnDropdownChanged(ChangeEvent<string> evt)
        {
            ENUM.ArenaItemGradeType type = Calculation.GradeTypeName.First(p => p.Value == evt.newValue).Key;
            Debug.Log($"Changed GradeType: {type}");
            itemData.SetGradeType(type);
            UpdateUI();     // UIの更新
        }

        /// <summary>
        /// UIの更新処理
        /// </summary>
        void UpdateUI()
        {
            name.text = itemData.Name;
            discription.text = itemData.GetDiscription();
            price.text = itemData.GetPrice().ToString("#,###");
            icon.style.backgroundImage = new StyleBackground(itemData.Item_Icon);
        }
    }
}