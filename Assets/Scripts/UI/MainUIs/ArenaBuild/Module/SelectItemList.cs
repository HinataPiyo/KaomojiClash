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
        ScrollView scrollView_ItemList;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize(VisualElement moduleRoot)
        {
            scrollView_ItemList = moduleRoot.Q<ScrollView>();
            CreateItems();
        }

        void CreateItems()
        {
            scrollView_ItemList.Clear();

            ArenaItemData[] itemDatas = arenaItemDatabase.GetAllArenaItemDatas();
            ArenaItemData[] sort = itemDatas.OrderBy(item => item.Item_Type).ToArray();     // 種類でソート
            
            foreach (ArenaItemData itemData in sort)
            {
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
                Debug.Log($"Selected Item: {itemData.Name}");
            });
            scrollView_ItemList.Add(elem);
        }
    }
}