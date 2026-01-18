using Constants.Global;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.KaomojiBuild.Module
{

    public class SelectKaomojiType : MonoBehaviour, IUIModuleHandler
    {
        KaomojiBuildModulesController moduleCtrl;
        Button[] buttons;

        void Awake()
        {
            moduleCtrl = GetComponent<KaomojiBuildModulesController>();
        }

        /// <summary>
        /// Managerクラスからこの関数が実行されRoot要素が渡される
        /// </summary>
        public void Initialize(VisualElement moduleRoot)
        {
            // 初期化処理をここに実装
            buttons = moduleRoot.Query<Button>().ToList().ToArray();
            for (int i = 0; i < buttons.Length; i++)
            {
                int index = i;      // ローカルコピーを作成
                ENUM.KaomojiPartType type = (ENUM.KaomojiPartType)index;
                buttons[index].text = Calculation.GetKaomojiPartTypeName(type);
                // ボタンがクリックされたときの処理
                buttons[i].clicked += () =>
                {
                    moduleCtrl.module_SKP.SortByType(type);     // ここでタイプ別にソートする関数を呼び出す
                };
            }
        }
    }
}