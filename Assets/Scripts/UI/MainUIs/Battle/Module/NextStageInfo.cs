namespace UI.Battle
{
    using Constants.Global;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class NextStageInfo : MonoBehaviour, IUIModuleHandler
    {
        Label difficulty;
        Label avgLevel;
        VisualElement root;

        public void Initialize(VisualElement root)
        {
            this.root = root.Q<VisualElement>("root");
            difficulty = root.Q<VisualElement>("difficulty").Q<Label>("value");
            avgLevel = root.Q<VisualElement>("avgLevel").Q<Label>("value");
        }

        /// <summary>
        /// 次のステージの難易度と平均レベルを表示する。
        /// </summary>
        public void SetNextStageInfo(ENUM.Difficulty dif, int avgLv)
        {
            root.RemoveFromClassList("hidden");     // 非表示状態を解除して表示する
            root.AddToClassList("show");            // 表示アニメーション開始
            difficulty.text = dif.ToString().ToUpper();
            difficulty.style.color = Calculation.GetColorByDifficulty(dif);
            avgLevel.text = avgLv.ToString();
        }

        public void Hidden()
        {
            root.RemoveFromClassList("show");       // 表示アニメーションをリセットして非表示にする
            root.AddToClassList("hidden");
        }

        
    }
}