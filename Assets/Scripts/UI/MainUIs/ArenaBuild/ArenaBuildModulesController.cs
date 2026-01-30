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


        public SelectItemList module_SL{ get; private set; }
        public ArenaItemData SelectedArenaItemData { get; set; } = null;

        public bool IsSetting { get; private set; } = false;

        public void SelectedArenaItemDataNull()
        {
            SelectedArenaItemData = null;
        }

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