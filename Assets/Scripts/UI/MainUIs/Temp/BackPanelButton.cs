using UnityEngine;
using UnityEngine.UIElements;
using UI.Main;

namespace UI
{

    public interface IBackPanelButtonHandler
    {
        ENUM.Panel BeforPanel { get; }
        void CreateBackButton(VisualElement root);
    }
    
    /// <summary>
    /// バックステージパネルへ移動するボタンのクラス
    /// 前回のPanelを覚えておく必要がある
    /// </summary>
    public class BackPanelButton
    {
        const string BACK_BUTTON = "Temp_Back";
        public ENUM.Panel BeforPanel { get; private set;}
        Button button;

        /// <summary>
        /// このクラスが生成された時に前のパネル情報をセットする
        /// </summary>
        /// <param name="befor">前のパネル</param>
        public BackPanelButton(ENUM.Panel befor, VisualElement root)
        {
            BeforPanel = befor;
            button = root.Q<VisualElement>(BACK_BUTTON).Q<Button>();
            button.clicked += Click;
        }

        void Click()
        {
            PanelChangeManager.I.Change(BeforPanel);
            AudioManager.I.PlaySE("BackOnClick");
        }

        public void IsIntaractable(bool isEnable)
        {
            button.SetEnabled(isEnable);
        }
    }
}