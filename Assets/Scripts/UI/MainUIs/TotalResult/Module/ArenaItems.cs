namespace UI.TotalResult.Module
{
    using UnityEngine.UIElements;
    using UnityEngine;
    
    public class ArenaItems : MonoBehaviour, IUIModuleHandler
    {
        [SerializeField] VisualTreeAsset temp_ArenaItem;
        ScrollView arenaItemList;
        public void Initialize(VisualElement moduleRoot)
        {
            arenaItemList = moduleRoot.Q<ScrollView>();
            ListClear();
        }

        public void CreateUI(ArenaItemData[] datas)
        {
            foreach (var data in datas)
            {
                // 使用上、装備する際AreaItemをはすじたらRemoveじゃなくてnullにするので、必然的にここで弾く必要がある
                if(data == null) continue;
                VisualElement tempRoot = temp_ArenaItem.Instantiate();
                new Temp.ArenaItem(tempRoot, data.Item_Icon, data.UsageCount, data.GetMaxUsageCountByGrade());
                arenaItemList.Add(tempRoot);
            }
        }

        void ListClear()
        {
            arenaItemList.Clear();
        }


    }
}