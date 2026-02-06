namespace UI.TotalResult.Module
{
    using UnityEngine.UIElements;
    using UnityEngine;
    
    public class FewArenaInfo : MonoBehaviour, IUIModuleHandler
    {
        Label cultureLevel;
        Label averageLevel;
        Label kaomojiDensity;

        public void Initialize(VisualElement moduleRoot)
        {
            cultureLevel = moduleRoot.Q<VisualElement>("culture-level-box").Q<Label>("value");
            averageLevel = moduleRoot.Q<VisualElement>("average-level-box").Q<Label>("value");
            kaomojiDensity = moduleRoot.Q<VisualElement>("kaomoji-density-box").Q<Label>("value");
        }

        /// <summary>
        /// UIを更新
        /// </summary>
        /// <param name="cultureLvl">文化圏レベル</param>
        /// <param name="avgLvl">平均レベル</param>
        /// <param name="density">顔文字密度</param>
        public void UpdateUI(int cultureLvl, float avgLvl, float density)
        {
            cultureLevel.text = cultureLvl.ToString();
            averageLevel.text = avgLvl.ToString("F0");
            kaomojiDensity.text = (density * 100).ToString("F0") + "%";
        }
    }
}