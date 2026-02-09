namespace UI.TotalResult.Temp
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ArenaItem
    {
        public ArenaItem(VisualElement tempRoot, Sprite sprite, int usageCount, int maxCount)
        {
            VisualElement icon = tempRoot.Q<VisualElement>("icon");
            ProgressBar dupBar = tempRoot.Q<ProgressBar>();

            icon.style.backgroundImage = new StyleBackground(sprite);
            dupBar.highValue = maxCount;
            dupBar.value = usageCount;
        }
    }
}