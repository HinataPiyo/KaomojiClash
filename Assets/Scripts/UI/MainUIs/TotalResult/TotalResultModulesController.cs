namespace UI.TotalResult
{
    using UnityEngine;
    using System.Collections;
    using UI.Base;
    using UI.TotalResult.Module;
    using UnityEngine.UIElements;
    using System.ComponentModel.Design.Serialization;

    public class TotalResultModulesController : ModuleControllerBase
    {
        [SerializeField] float fadeSpeed = 0.5f;
        UIDocument uiDoc;
        const string MODULE_FEW_ARENA_INFO = "FewArenaInfo";
        const string MODULE_KAOMOJI_COMPOSITIONS = "KaomojiCompositions";
        const string MODULE_TOTAL_GET_AND_DEC_MONEY = "TotalGetMoneyAndDecMoney";
        const string MODULE_TOTAL_GET_PARTS = "TotalGetParts";
        const string MODULE_ARENA_ITEMS = "ArenaItems";

        public FewArenaInfo module_FAI { get; private set; }
        public KaomojiCompositions module_KC { get; private set; }
        public TotalGetAndDecMoney module_TGADM { get; private set; }
        public TotalGetParts module_TGP { get; private set; }
        public ArenaItems module_AI { get; private set; }

        Button btn_Retry;
        Button btn_ToHome;

        

        void Awake()
        {
            uiDoc = GetComponent<UIDocument>();

            module_FAI = GetComponent<FewArenaInfo>();
            module_KC = GetComponent<KaomojiCompositions>();
            module_TGADM = GetComponent<TotalGetAndDecMoney>();
            module_TGP = GetComponent<TotalGetParts>();
            module_AI = GetComponent<ArenaItems>();
        }

        void Start()
        {
            DisableResultPanel();
        }


        protected override void Initialize()
        {
            VisualElement root = uiDoc.rootVisualElement;
            Initialize(module_FAI, MODULE_FEW_ARENA_INFO, root);
            Initialize(module_KC, MODULE_KAOMOJI_COMPOSITIONS, root);
            Initialize(module_TGADM, MODULE_TOTAL_GET_AND_DEC_MONEY, root);
            Initialize(module_TGP, MODULE_TOTAL_GET_PARTS, root);
            Initialize(module_AI, MODULE_ARENA_ITEMS, root);

            ButtonSetup();
        }

        void ButtonSetup()
        {
            VisualElement root = uiDoc.rootVisualElement;
            btn_Retry = root.Q<VisualElement>("Temp_Retry").Q<Button>();
            btn_ToHome = root.Q<VisualElement>("Temp_BackHome").Q<Button>();

            btn_Retry.clicked += () => 
            {
                SceneChangeManager.I.ChangeScene(ENUM.Scene.Battle);
                Debug.Log("Retry Button Clicked");
            };
            btn_ToHome.clicked += () =>
            {
                SceneChangeManager.I.ChangeScene(ENUM.Scene.Home);
                Debug.Log("To Title Button Clicked");
            };
        }

        public void EnableResultPanel()
        {
            gameObject.SetActive(true);
            StartCoroutine(FadeIn());
            MovePanel();
        }

        public void DisableResultPanel()
        {
            gameObject.SetActive(false);
        }

        void MovePanel()
        {
            StartCoroutine(MovePanelCoroutine());
        }

        /// <summary>
        /// UIパネルを下から上へイーズアウトで移動
        /// </summary>
        /// <returns></returns>
        IEnumerator MovePanelCoroutine()
        {
            VisualElement root = uiDoc.rootVisualElement;
            float moveDuration = 0.8f;  // 移動時間
            float moveDistance = -100f;  // 移動距離（ピクセル）
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / moveDuration;
                float eased = EaseOutCubic(progress);  // イーズアウト：最初は早く、徐々に減速

                float yTranslate = Mathf.Lerp(-moveDistance, 0f, eased);
                root.style.translate = new Translate(new Length(0), new Length(yTranslate));

                yield return null;
            }

            // 最終位置を確定
            root.style.translate = new Translate(new Length(0), new Length(0));
        }

        /// <summary>
        /// イーズアウト関数（最初は速く、徐々に減速）
        /// </summary>
        float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        /// <summary>
        /// 取得経験値用パネルをフェードインで表示
        /// </summary>
        /// <returns></returns>
        IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while(elapsed < fadeSpeed)
            {
                elapsed += Time.deltaTime;
                uiDoc.rootVisualElement.style.opacity = Mathf.Lerp(0f, 1f, elapsed / fadeSpeed);
                yield return null;
            }

            uiDoc.rootVisualElement.style.opacity = 1f;
        }
    }
}