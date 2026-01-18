using UI.KaomojiBuild.Template;
using UnityEngine;
using UnityEngine.UIElements;
using Constants.Global;

namespace UI.KaomojiBuild.Module
{
    /// <summary>
    ///  選択された記号のステータスを表示する
    /// </summary>
    public class SelectedKaomojiPartStatusParamater : MonoBehaviour, IUIModuleHandler, IUIPartHandler
    {
        const string TEMP_STATUS_PARAMATER = "Temp_StatusParamater";
        StatusParamater statusParamater;

        Label partType;     // 部位
        Label partName;     // パーツ名
        Label partIcon;     // アイコン

        public void Initialize(VisualElement moduleRoot)
        {
            VisualElement statusRoot = moduleRoot.Q(TEMP_STATUS_PARAMATER);
            statusParamater = new StatusParamater();
            statusParamater.Initialize(statusRoot);

            VisualElement selected_part_info = moduleRoot.Q("selected-part-info");
            partType = selected_part_info.Q<Label>("type-value");
            partName = selected_part_info.Q<Label>("name-value");
            partIcon = selected_part_info.Q<Label>("icon-value");
            
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
        }

        public void Reset()
        {
            statusParamater.Reset();
            partType.text = "-";
            partName.text = "-";
            partIcon.text = string.Empty;
        }
    }
}