namespace UI.ArenaBuild.Template
{
    using System.Linq;
    using Constants.Global;
    using UnityEngine;
    using UnityEngine.UIElements;

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

        /// <summary>
        /// GradeTypeが変更された時の処理
        /// </summary>
        /// <param name="evt">選択されたGradeTypeの文字列</param>
        void OnDropdownChanged(ChangeEvent<string> evt)
        {
            ENUM.ArenaItemGradeType type = Calculation.GradeTypeName.First(p => p.Value == evt.newValue).Key;

            if (!itemData.IsGradeTypeUnlocked(type))
            {
                ApplyDefaultGradeType();
                UpdateUI();
                return;
            }

            Debug.Log($"Changed GradeType: {type}");
            itemData.SetGradeType(type);
            UpdateUI();     // UIの更新
        }

        /// <summary>
        /// UIの更新処理
        /// </summary>
        void UpdateUI()
        {
            EnsureGradeTypeIsValidByUsage();

            name.text = itemData.Name;
            discription.text = itemData.GetDiscription();
            price.text = itemData.GetPrice().ToString("#,###");
            icon.style.backgroundImage = new StyleBackground(itemData.Item_Icon);
            UpdateDuplicateRate();

            // 規定値未達時でも「触った時のメッセージ表示」が必要なため、常に有効のままにする。
            gradeType.SetEnabled(true);
        }

        /// <summary>
        /// 使用回数の更新処理
        /// </summary>
        void UpdateDuplicateRate()
        {
            duplicateRate.highValue = GetNextGradeTypeMaxUsageCount();
            duplicateRate.value = itemData.UsageCount;
            duplicateRate.title = $"{itemData.UsageCount} / {GetNextGradeTypeMaxUsageCount()}";
        }

        /// <summary>
        /// 現在のGradeTypeが使用回数条件を満たしているか確認し、
        /// 満たしていない場合は0番のGradeTypeへ戻す。
        /// </summary>
        void EnsureGradeTypeIsValidByUsage()
        {
            if (itemData.IsGradeTypeUnlocked(itemData.GradeType)) return;

            ApplyDefaultGradeType();
            OverlayCanvasManager.I.ShowMessage("使用回数が不足しているため変更できません");
        }
        
        /// <summary>
        /// 次のGradeTypeに必要な使用回数を取得する。
        /// </summary>
        /// <returns></returns>
        int GetNextGradeTypeMaxUsageCount()
        {
            // enum加算ではなく、使用回数ベースで「次に必要な目標値」を返す。
            // これにより選択中GradeTypeに依存せず、表示値を安定させる。
            int mkOneUnlockUsage = itemData.Max_First_UsageCount;
            int mkTwoUnlockUsage = itemData.GetMaxUsageCountByGrade(ENUM.ArenaItemGradeType.MK_ONE);
            int mkTwoMaxUsage = itemData.GetMaxUsageCountByGrade(ENUM.ArenaItemGradeType.MK_TWO);

            if (itemData.UsageCount < mkOneUnlockUsage)
            {
                return mkOneUnlockUsage;
            }

            if (itemData.UsageCount < mkTwoUnlockUsage)
            {
                return mkTwoUnlockUsage;
            }

            // すべて解放済みの場合は最終段階の上限を表示する。
            return mkTwoMaxUsage;
        }

        /// <summary>
        /// GradeTypeをDropDownの0番（先頭）へ戻す。
        /// </summary>
        void ApplyDefaultGradeType()
        {
            if (gradeType.choices == null || gradeType.choices.Count == 0) return;

            string defaultGradeName = gradeType.choices[0];
            ENUM.ArenaItemGradeType defaultGradeType = Calculation.GradeTypeName.First(p => p.Value == defaultGradeName).Key;

            if (itemData.GradeType != defaultGradeType)
            {
                itemData.SetGradeType(defaultGradeType);
            }

            gradeType.SetValueWithoutNotify(defaultGradeName);
        }
    }
}