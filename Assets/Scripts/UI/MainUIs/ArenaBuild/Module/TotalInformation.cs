namespace UI.ArenaBuild.Module
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class TotalInformation : MonoBehaviour, IUIModuleHandler
    {
        Label price;
        Label maxSetItemCount;
        public void Initialize(VisualElement moduleRoot)
        {
            price = moduleRoot.Q<VisualElement>("total-price-box").Q<Label>("value");
            maxSetItemCount = moduleRoot.Q<VisualElement>("total-set-item-count").Q<Label>("value");
        }

        public void UpdatePrice(int totalPrice)
        {
            price.text = totalPrice.ToString("N0") + "円";
        }

        public void UpdateMaxSetItemCount(int count, int maxCount = 0)
        {
            maxSetItemCount.text = count.ToString() + "/" + maxCount.ToString();
        }
    }
}