using UI.KaomojiBuild.Template;
using UnityEngine;
using UnityEngine.UIElements;
using Constants;
using Constants.Global;

namespace UI.KaomojiBuild.Module
{
    /// <summary>
    ///  選択された記号のステータスを表示する
    /// </summary>
    public class SelectedKaomojiPartStatusParamater : MonoBehaviour, IUIModuleHandler, IUIPartHandler
    {
        [SerializeField] VisualTreeAsset temp_SkillTag;
        StatusParamater statusParamater;
        KaomojiBuildModulesController moduleCtrl;

        Label partType;     // 部位
        Label partName;     // パーツ名
        Label partIcon;     // アイコン
        VisualElement skillTagContainer;

        void Awake()
        {
            moduleCtrl = GetComponent<KaomojiBuildModulesController>();
        }

        public void Initialize(VisualElement moduleRoot)
        {
            statusParamater = new StatusParamater();
            statusParamater.Initialize(moduleRoot);

            VisualElement selected_part_info = moduleRoot.Q("selected-part-info");
            partType = selected_part_info.Q<Label>("type-value");
            partName = selected_part_info.Q<Label>("name-value");
            partIcon = selected_part_info.Q<Label>("icon-value");

            skillTagContainer = moduleRoot.Q<VisualElement>("skill-tags-box");
            
            Reset();
        }

        /// <summary>
        /// パーツデータを受け取る
        /// </summary>
        /// <param name="data"></param>
        public void AssignPart(KaomojiPartData part)
        {
            KaomojiPart.Speed speed = part.Data.speed;
            KaomojiPart.Power power = part.Data.power;
            KaomojiPart.Guard guard = part.Data.guard;
            KaomojiPart.Stamina stamina = part.Data.stamina;

            statusParamater.ShowStatus(speed, power, guard, stamina, part.PartType);

            partType.text = Calculation.GetKaomojiPartTypeName(part.PartType);
            partName.text = part.Data.partName;
            partIcon.text = part.Data.part;

            // スキルタグの表示を更新
            skillTagContainer.Clear();
            foreach(SkillTag tag in part.Data.SkillTags)
            {
                VisualElement tempRoot = temp_SkillTag.Instantiate();
                SkillTagUI ui = new SkillTagUI(tempRoot, tag);
                RegisterTagAndCreateDescription(tempRoot, tag, ui);
                skillTagContainer.Add(tempRoot);
            }

        }

        // 正しいタイミングで座標を取得
        public void RegisterTagAndCreateDescription(VisualElement elem, SkillTag tag, SkillTagUI ui)
        {
            // GeometryChangedEventに登録
            elem.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                CreateTagAtPosition(elem, tag, ui);
            });
        }
        
        void CreateTagAtPosition(VisualElement elem, SkillTag tag, SkillTagUI ui)
        {
            GameObject skillTagObject = null;

            elem.RegisterCallback<PointerEnterEvent>(ev =>
            {
                Vector2 uiToolkitScreenPos = elem.worldBound.position;
                skillTagObject = moduleCtrl.ShowSkillTagDescription(tag, uiToolkitScreenPos);
            });

            elem.RegisterCallback<PointerLeaveEvent>(ev =>
            {
                if (skillTagObject == null) return;

                Destroy(skillTagObject);
                skillTagObject = null;
            });

            // elem.RegisterCallback<DetachFromPanelEvent>(ev =>
            // {
            //     if (skillTagObject == null) return;

            //     Destroy(skillTagObject);
            //     skillTagObject = null;
            // });
        }

        public void Reset()
        {
            statusParamater.Reset();
            partType.text = "-";
            partName.text = "-";
            partIcon.text = string.Empty;
            skillTagContainer.Clear();
        }
    }
}