using UnityEngine;
using UnityEngine.UIElements;

namespace UI.KaomojiBuild.Module
{
    /// <summary>
    /// 顔文字記号のパーツを選択するモジュール
    /// </summary>
    public class SelectKaomojiParts : MonoBehaviour, IUIModuleHandler, IUIPartHandler
    {
        [SerializeField] KaomojiPartsDatabase partsDatabase;
        [SerializeField] VisualTreeAsset temp_SelectKaomojiParts;

        KaomojiBuildModulesController modulesCtrl;
        VisualElement moduleRoot;
        ScrollView list_view;

        void Awake()
        {
            modulesCtrl = GetComponent<KaomojiBuildModulesController>();
        }

        /// <summary>
        /// Managerクラスからこの関数が実行されRoot要素が渡される
        /// </summary>
        public void Initialize(VisualElement moduleRoot)
        {
            this.moduleRoot = moduleRoot;
            list_view = moduleRoot.Q<ScrollView>();
            Reset();

            SortByType(ENUM.KaomojiPartType.Eyes);      // 初期表示は目パーツで
        }

        /// <summary>
        /// パーツを追加する
        /// </summary>
        public void AssignPart(KaomojiPartData part)
        {
            VisualElement elem = temp_SelectKaomojiParts.Instantiate();
            Button button = elem.Q<Button>();
            button.text = part.Data.part;
            button.clicked += () =>
            {
                Debug.Log($"選択されたパーツ: {part.Data.part}");
                // ここにパーツ選択時の処理を追加
                // SelectedKaomojiPartStatuParamater　にDataを渡す
                modulesCtrl.module_SKP_StatusParamater.AssignPart(part);
                modulesCtrl.module_SD.AssignPart(part);     // ! 一旦ボタンを押下しただけで反映されるようにする
            };

            list_view.contentContainer.Add(elem);
        }

        /// <summary>
        /// 指定したタイプのパーツのみを表示する
        /// </summary>
        /// <param name="type">顔文字の部位</param>
        public void SortByType(ENUM.KaomojiPartType type)
        {
            list_view.Clear();
            foreach (var part in partsDatabase.GetPartsByType(type))
            {
                AssignPart(part);
            }
        }

        /// <summary>
        /// 表示をリセットする
        /// </summary>
        public void Reset()
        {
            list_view.Clear();
        }
    }
}