using UnityEngine;
using UI.Base;
using UnityEngine.UIElements;

namespace UI.ArenaBuild
{    
    public class ArenaBuildModulesController : ModuleControllerBase
    {
        UIDocument uiDocs;

        void Awake()
        {
            uiDocs = GetComponent<UIDocument>();
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
        }
    }
}