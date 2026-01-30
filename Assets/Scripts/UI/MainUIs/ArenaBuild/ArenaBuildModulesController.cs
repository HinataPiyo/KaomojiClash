using UnityEngine;
using UI.Base;
using UnityEngine.UIElements;
using UI.ArenaBuild.Module;

namespace UI.ArenaBuild
{    
    public class ArenaBuildModulesController : ModuleControllerBase
    {
        [SerializeField] ArenaItemBoxController arenaItemBoxController;
        [SerializeField] Wall wall;
        
        UIDocument uiDocs;
        const string SELECT_ITEM_LIST_MODULE = "SelectItemList";
        const string TOTAL_INFORMATION_MODULE = "TotalInformation";


        public SelectItemList module_SL{ get; private set; }
        public TotalInformation module_TI { get; private set; }
        public ArenaItemData SelectedArenaItemData { get; set; } = null;        // 選択中のアリーナアイテムデータ

        public bool IsSetting { get; private set; } = false;        // 設定モードか削除モードかどうか

        /// <summary>
        /// 選択中のアリーナアイテムデータをnullにする
        /// </summary>
        public void SelectedArenaItemDataNull()
        {
            SelectedArenaItemData = null;
        }

        /// <summary>
        /// 設定モードの切り替え
        /// </summary>
        public void ChangeIsSetting(bool setting)
        {
            IsSetting = setting;
            if(setting)
            {
                // Listパネルと戻るボタンを操作不可にする
                module_SL.IsIntaractable(false);
                BackButton.IsIntaractable(false);
            }
            else
            {
                // Listパネルと戻るボタンを操作可能にする
                module_SL.IsIntaractable(true);
                BackButton.IsIntaractable(true);
                
            }
        }

        void Awake()
        {
            uiDocs = GetComponent<UIDocument>();
            module_SL = GetComponent<SelectItemList>();
            module_TI = GetComponent<TotalInformation>();
        }

        void Start()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            VisualElement root = uiDocs.rootVisualElement;
            CreateHasMoney(root);
            CreateBackButton(root);
            Initialize(module_SL, SELECT_ITEM_LIST_MODULE, root);
            Initialize(module_TI, TOTAL_INFORMATION_MODULE, root);
        }

        /// <summary>
        /// TotalInformationモジュールの情報更新
        /// </summary>
        /// <param name="totalPrice">アリーナにセッティングされたアイテムの合計使用料</param>
        /// <param name="setItemCount">現在セットしている数</param>
        /// <param name="maxSetItemCount">最大セット数</param>
        public void UpdateTotalInfo(int totalPrice, int setItemCount, int maxSetItemCount)
        {
            module_TI.UpdatePrice(totalPrice);
            module_TI.UpdateMaxSetItemCount(setItemCount, maxSetItemCount);
        }

        void OnDisable()
        {
            if(arenaItemBoxController == null || arenaItemBoxController.gameObject == null) return;
            arenaItemBoxController.PanelIsEnable(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            arenaItemBoxController.PanelIsEnable(true);
        }
    }
}