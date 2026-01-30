using UI.Main;
using UnityEngine.UIElements;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 次のパネルへ移動するボタンのクラス
    /// 基本、ボタンはUIToolkitのためSerializeFieldで持てないので
    /// このクラスで次のパネル情報を持たせる
    /// </summary>
    public class NextPanelButton
    {
        public ENUM.Panel NextPanel { get; private set;}

        /// <summary>
        /// このクラスが生成された時に次のパネル情報をセットする
        /// </summary>
        /// <param name="next">次に表示するパネルを保持</param>
        public NextPanelButton(ENUM.Panel next, Button button)
        {
            NextPanel = next;
            button.clicked += Click;
        }

        /// <summary>
        /// ボタンがクリックされたときに呼ばれる処理
        /// </summary>
        void Click()
        {
            PanelChangeManager.I.Change(NextPanel);
            Debug.Log("NextPanelButtonが押下されました");
        }
    }
}