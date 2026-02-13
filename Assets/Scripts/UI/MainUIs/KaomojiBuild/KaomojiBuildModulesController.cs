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

        [Header("メッセージテキスト")]
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject messageUI_Prefab;
        [SerializeField] GameObject skillTag_Prefab;

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
        /// スキルタグの説明を表示
        /// </summary>
        public GameObject ShowSkillTagDescription(SkillTag tag, Vector2 uiToolkitScreenPos, bool isLevelDisplay = false)
        {
            GameObject obj = Instantiate(skillTag_Prefab, canvas.transform);
            RectTransform rect = obj.GetComponent<RectTransform>();
            // 座標変換
            Vector2 canvasPosition = ConvertUIToolkitToCanvasPosition(
                uiToolkitScreenPos, canvas, rect
            );

            Vector2 offsetY = new Vector2(0, -30);
            rect.anchoredPosition = canvasPosition + offsetY;

            SkillTagDescription skillTag = obj.GetComponent<SkillTagDescription>();
            skillTag.ShowDescription(tag.GetDescriptionsArray(), isLevelDisplay);

            return obj;
        }

        /// <summary>
        /// メッセージを表示
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessage(string message)
        {
            GameObject messageUI_Obj = Instantiate(messageUI_Prefab, canvas.transform);
            MessageUI messageUI = messageUI_Obj.GetComponent<MessageUI>();
            messageUI.SetMessage(message);
        }

        /// <summary>
        /// タイプ解放メッセージを取得
        /// </summary>
        public string GetTypeLockedMessage(ENUM.KaomojiPartType type)
        {
            return $"文化圏レベル{AreaBuild.PartTypeToReleaseLevel[type]}をクリアすると解放されます。";
        }

        Vector2 ConvertUIToolkitToCanvasPosition(Vector2 uiToolkitPos, Canvas canvas, RectTransform targetRect)
        {
            // UIToolkitの座標系はスクリーン左上が原点(0,0)
            // Y軸は下向きが正
            Vector2 screenPoint = new Vector2(
                uiToolkitPos.x,
                Screen.height - uiToolkitPos.y // Y軸を反転
            );

            // スクリーン座標からCanvas座標へ変換
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            // 5. 座標変換
            bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPoint,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out Vector2 localPoint
            );

            return localPoint;
        }
    }
}
