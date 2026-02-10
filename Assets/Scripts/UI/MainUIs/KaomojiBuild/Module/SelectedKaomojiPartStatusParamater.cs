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

        Label partType;     // 部位
        Label partName;     // パーツ名
        Label partIcon;     // アイコン
        VisualElement skillTagContainer;

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

            statusParamater.ShowStatus(speed, power, guard, stamina);

            partType.text = Calculation.GetKaomojiPartTypeName(part.PartType);
            partName.text = part.Data.partName;
            partIcon.text = part.Data.part;

            // スキルタグの表示を更新
            skillTagContainer.Clear();
            foreach(SkillTag tag in part.Data.SkillTags)
            {
                VisualElement tempRoot = temp_SkillTag.Instantiate();
                new SkillTagUI(tempRoot, tag);
                skillTagContainer.Add(tempRoot);
            }

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