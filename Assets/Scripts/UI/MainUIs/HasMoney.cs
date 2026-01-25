using UnityEngine.UIElements;

namespace UI.Main
{
    /// <summary>
    /// 所持金表示パネルの管理クラス
    /// </summary>
    public class HasMoney
    {
        const string HAS_MONEY = "Temp_HasMoney";
        Label hasMoney;

        /// <summary>
        /// パネルのRootを受け取り、所持金表示ラベルを初期化する
        /// </summary>
        /// <param name="root"></param>
        public HasMoney(VisualElement root)
        {
            hasMoney = root.Q<VisualElement>(HAS_MONEY).Q<Label>("value");
            UpdateMoney();      // 初期表示更新
        }

        /// <summary>
        /// 所持金表示ラベルを更新する
        /// </summary>
        public void UpdateMoney()
        {
            if(Money.HasMoney == 0) hasMoney.text = "0";
            else
            hasMoney.text = Money.HasMoney.ToString("#,###");
        }
    }
}