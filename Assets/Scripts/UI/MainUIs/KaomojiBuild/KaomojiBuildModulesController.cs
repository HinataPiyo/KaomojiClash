using UI.Base;
using UI.KaomojiBuild.Module;
using UnityEngine.UIElements;
using UnityEngine;
using Constants;

namespace UI.KaomojiBuild
{
    public class KaomojiBuildModulesController : ModuleControllerBase
    {
        /// <summary>
        /// moduleの名前
        /// </summary>
        const string MODULE_SELECT_KAOMOJI_PARTS = "SelectKaomojiParts";
        const string MODULE_SELECT_KAOMOJI_TYPE = "SelectKaomojiType";
        const string MODULE_SELECTED_DISPLAY = "SelectedDisplay";
        const string MODULE_FACE_LINE = "FaceLine";
        const string MODULE_SELECTED_KAOMOJI_PART_STATUS_PARAMATER = "SelectedKaomojiPartStatusParamater";

        /// <summary>
        /// キャッシュ
        /// </summary>
        UIDocument uiDocument;
        public SelectKaomojiParts module_SKP { get; private set; }
        public SelectKaomojiType module_SKT { get; private set; }
        public SelectedKaomojiPartStatusParamater module_SKP_StatusParamater { get; private set; }
        public SelectedDisplay module_SD { get; private set; }
        public FaceLine module_FL { get; private set; }

        

        void Awake()
        {
            uiDocument = GetComponent<UIDocument>();

            module_SKP = GetComponent<SelectKaomojiParts>();
            module_SKT = GetComponent<SelectKaomojiType>();
            module_SD = GetComponent<SelectedDisplay>();
            module_FL = GetComponent<FaceLine>();
            module_SKP_StatusParamater = GetComponent<SelectedKaomojiPartStatusParamater>();
        }

        protected override void Initialize()
        {
            VisualElement root = uiDocument.rootVisualElement;
            CreateBackButton(root);
            Initialize(module_SKP, MODULE_SELECT_KAOMOJI_PARTS, root);
            Initialize(module_SKT, MODULE_SELECT_KAOMOJI_TYPE, root);
            Initialize(module_SD, MODULE_SELECTED_DISPLAY, root);
            Initialize(module_FL, MODULE_FACE_LINE, root);
            Initialize(module_SKP_StatusParamater, MODULE_SELECTED_KAOMOJI_PART_STATUS_PARAMATER, root);
        }

        /// <summary>
        /// タイプ解放メッセージを取得
        /// </summary>
        public string GetTypeLockedMessage(ENUM.KaomojiPartType type)
        {
            return $"文化圏レベル{AreaBuild.PartTypeToReleaseLevel[type]}をクリアすると解放されます。";
        }
    }
}
