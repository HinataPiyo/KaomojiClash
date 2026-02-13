using System;
using System.ComponentModel;
using Constants;
using Constants.Global;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.KaomojiBuild.Module
{

    public class SelectKaomojiType : MonoBehaviour, IUIModuleHandler
    {
        KaomojiBuildModulesController moduleCtrl;

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
            Button[] type_buttons = moduleRoot.Query<Button>("type").ToList().ToArray();

            for (int i = 0; i < type_buttons.Length; i++)
            {
                int index = i;      // ローカルコピーを作成
                ENUM.KaomojiPartType type = (ENUM.KaomojiPartType)index;

                bool isCleared = CheckIsCleared(type);
                type_buttons[index].style.opacity = isCleared ? 1f : 0.5f; // 解放されていないタイプのボタンを半透明にする
                // type_buttons[index].SetEnabled(isCleared);
                type_buttons[index].text = Calculation.GetKaomojiPartTypeName(type);

                // ボタンがクリックされたときの処理
                type_buttons[index].clicked += () =>
                {
                    if (!isCleared)
                    {
                        moduleCtrl.ShowMessage(moduleCtrl.GetTypeLockedMessage(type));
                        return;
                    }

                    moduleCtrl.module_SKP.SortByType(type);     // ここでタイプ別にソートする関数を呼び出す
                };
            }
        }

        /// <summary>
        /// 解放されているかどうかを確認
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CheckIsCleared(ENUM.KaomojiPartType type)
        {
            return AreaManager.I.CheckIsClearedByCultureLevel(type);
        }
    }
}