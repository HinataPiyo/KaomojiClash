using UnityEngine;
using UI.Base;
using UnityEngine.UIElements;
using UI.ArenaBuild.Module;

namespace UI.ArenaBuild
{    
    public class ArenaBuildModulesController : ModuleControllerBase
    {
        UIDocument uiDocs;
        const string SELECT_ITEM_LIST_MODULE = "SelectItemList";

        public SelectItemList module_SL{ get; private set; }


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
    }
}