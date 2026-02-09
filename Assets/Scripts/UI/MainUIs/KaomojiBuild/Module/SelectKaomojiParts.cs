using System.Linq;
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

            SortByType(ENUM.KaomojiPartType.Mouth);      // 初期表示
        }

        /// <summary>
        /// パーツを追加する
        /// </summary>
        public void AssignPart(KaomojiPartData part)
        {
            VisualElement elem = temp_SelectKaomojiParts.Instantiate();
            Button button = elem.Q<Button>();
            Label body = elem.Q<Label>();
            ProgressBar progress = elem.Q<ProgressBar>();

            body.text = part.Data.part;
            button.clicked += () =>
            {
                Debug.Log($"選択されたパーツ: {part.Data.part}");
                // ここにパーツ選択時の処理を追加
                // SelectedKaomojiPartStatuParamater　にDataを渡す
                modulesCtrl.module_SKP_StatusParamater.AssignPart(part);
                modulesCtrl.module_SD.AssignPart(part);     // ! 一旦ボタンを押下しただけで反映されるようにする
            };

            // 表示非表示の制御
            bool isMaxDup = part.Data.IsMaxDup();

            // 初期表示用のフラグが立っている場合
            if(part.Data.GetIsInitDisplayUI())
            {
                isMaxDup = true;    // 強制的に表示
                progress.highValue = 1;  // 最大値を1にしておく
                progress.value = 1;
            }
            else
            {
                progress.highValue = part.Data.MaxDup;
                progress.value = part.Data.CurrentDup;
            }

            elem.SetEnabled(isMaxDup);      // DupCountが最大に達していれば表示
            
            if(isMaxDup) progress.title = $"{part.Data.CurrentDup}";        // 最大に達していれば現在の所持数だけ表示
            else progress.title = $"{part.Data.CurrentDup} / {part.Data.MaxDup}";

            list_view.Add(elem);
        }

        /// <summary>
        /// 指定したタイプのパーツのみを表示する
        /// </summary>
        /// <param name="type">顔文字の部位</param>
        public void SortByType(ENUM.KaomojiPartType type)
        {
            list_view.Clear();
            var parts = partsDatabase.GetPartsByType(type).ToList();
            
            // Enabled=Trueのものを前に、Falseのものを後ろにソート
            parts.Sort((a, b) =>
            {
                bool aEnabled = a.Data.IsMaxDup() || a.Data.GetIsInitDisplayUI();
                bool bEnabled = b.Data.IsMaxDup() || b.Data.GetIsInitDisplayUI();
                return bEnabled.CompareTo(aEnabled);  // trueを前に
            });
            
            foreach (var part in parts)
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