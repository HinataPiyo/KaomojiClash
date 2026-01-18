using UI.KaomojiBuild.Module;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.KaomojiBuild
{
    public class KaomojiBuildModulesController : MonoBehaviour
    {
        /// <summary>
        /// moduleの名前
        /// </summary>
        const string MODULE_SELECT_KAOMOJI_PARTS = "SelectKaomojiParts";
        const string MODULE_SELECT_KAOMOJI_TYPE = "SelectKaomojiType";
        const string MODULE_SELECTED_KAOMOJI_PART_STATUS_PARAMATER = "SelectedKaomojiPartStatusParamater";

        /// <summary>
        /// キャッシュ
        /// </summary>
        UIDocument uiDocument;
        public SelectKaomojiParts module_SKP { get; private set; }
        public SelectKaomojiType module_SKT { get; private set; }
        public SelectedKaomojiPartStatusParamater module_SKP_StatusParamater { get; private set; }

        void Awake()
        {
            uiDocument = GetComponent<UIDocument>();

            module_SKP = GetComponent<SelectKaomojiParts>();
            module_SKT = GetComponent<SelectKaomojiType>();
            module_SKP_StatusParamater = GetComponent<SelectedKaomojiPartStatusParamater>();

            Initialize(module_SKP, MODULE_SELECT_KAOMOJI_PARTS);
            Initialize(module_SKT, MODULE_SELECT_KAOMOJI_TYPE);
            Initialize(module_SKP_StatusParamater, MODULE_SELECTED_KAOMOJI_PART_STATUS_PARAMATER);
        }

        /// <summary>
        /// 各モジュールの初期化を行う
        /// </summary>
        /// <param name="uI">Moduleの初期化に必要なInterface</param>
        /// <param name="name">UIBuilderで設定している名前</param>
        void Initialize(IUIModuleHandler uI, string name)
        {
            VisualElement root = uiDocument.rootVisualElement;
            VisualElement moduleRoot = root.Q(name);

            if (moduleRoot != null)
            {
                uI.Initialize(moduleRoot);
            }
            else
            {
                Debug.LogError($"[ {name} ] モジュールのルート要素が見つかりません。");
            }
        }
    }
}