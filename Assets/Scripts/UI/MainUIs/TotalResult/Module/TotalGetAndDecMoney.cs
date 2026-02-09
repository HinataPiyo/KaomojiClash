namespace UI.TotalResult.Module
{
    using UnityEngine.UIElements;
    using UnityEngine;
    
    public class TotalGetAndDecMoney : MonoBehaviour, IUIModuleHandler
    {

        Label decMoney;
        Label getMoney;

        public void Initialize(VisualElement moduleRoot)
        {
            decMoney = moduleRoot.Q<VisualElement>("dec-box").Q<Label>("value");
            getMoney = moduleRoot.Q<VisualElement>("get-box").Q<Label>("value");
        }

        /// <summary>
        /// UI更新
        /// </summary>
        /// <param name="decMoneyValue">合計で消費した金額</param>
        /// <param name="getMoneyValue">合計で獲得した金額</param>
        public void UpdateUI(int decMoneyValue, int getMoneyValue)
        {
            decMoney.text = $"-{decMoneyValue}" + "円";
            getMoney.text = $"+{getMoneyValue}" + "円";
        }
        
    }
}