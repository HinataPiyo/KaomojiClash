namespace UI.ArenaBuild.Module
{
    using System.Linq;
    using UI.ArenaBuild.Template;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class SelectItemList : MonoBehaviour, IUIModuleHandler
    {
        [SerializeField] ArenaItemDatabase arenaItemDatabase;
        [SerializeField] VisualTreeAsset temp_SelectArenaItem;
        [SerializeField] ArenaItemBoxController arenaItemBoxController;
        ScrollView scrollView_ItemList;
        VisualElement moduleRoot;
        ArenaBuildModulesController ctrl;

        void Awake()
        {
            ctrl = GetComponent<ArenaBuildModulesController>();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize(VisualElement moduleRoot)
        {
            this.moduleRoot = moduleRoot;
            scrollView_ItemList = moduleRoot.Q<ScrollView>();
            CreateItems();
        }

        void CreateItems()
        {
            Debug.Log("Creating arena item list UI elements.");
            scrollView_ItemList.Clear();

            ArenaItemData[] itemDatas = arenaItemDatabase.GetAllArenaItemDatas();
            ArenaItemData[] sort = itemDatas.OrderBy(item => item.Item_Type).ToArray();     // 種類でソート
            
            for(int ii = 0; ii < sort.Length; ii++)
            {
                int index = ii;
                ArenaItemData itemData = sort[index];
                CreateItem(itemData);
            }
        }

        /// <summary>
        /// UIを生成しDataをセットする
        /// </summary>
        /// <param name="itemData"></param>
        void CreateItem(ArenaItemData itemData)
        {
            VisualElement elem = temp_SelectArenaItem.Instantiate();
            ArenaItem item = new ArenaItem(elem, itemData);
            item.ButtonClickedAction(() =>
            {
                ctrl.ChangeIsSetting(true);   // 設定モードに変更
                ctrl.SelectedArenaItemData = itemData;
                AudioManager.I.PlaySE("ArenaItemSelect");
            });
            scrollView_ItemList.Add(elem);
        }

        /// <summary>
        /// 操作可能かどうか
        /// </summary>
        /// <param name="isEnable"></param>
        public void IsIntaractable(bool isEnable)
        {
            moduleRoot.SetEnabled(isEnable);
        }
    }
}