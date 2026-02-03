using Constants;
using UI.KaomojiBuild.Template;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.KaomojiBuild.Module
{

    public class SelectedDisplay : MonoBehaviour, IUIModuleHandler, IUIPartHandler
    {
        KaomojiBuildModulesController modulesCtrl;
        StatusParamater statusParamater;
        Label kaomoji;

        void Awake()
        {
            modulesCtrl = GetComponent<KaomojiBuildModulesController>();
        }

        public void Initialize(VisualElement moduleRoot)
        {
            statusParamater = new StatusParamater();
            statusParamater.Initialize(moduleRoot);
            kaomoji = moduleRoot.Q<VisualElement>("face").Q<Label>("value");

            Reset();
            BuildKaomojiDisplay();
            UpdateStatusDisplay();
        }

        /// <summary>
        /// パーツを割り当てる
        /// </summary>
        /// <param name="part"></param>
        public void AssignPart(KaomojiPartData part)
        {
            KAOMOJI K = modulesCtrl.PlayerData.Kaomoji;
            K.SetPartDataByType(part);

            BuildKaomojiDisplay();
            UpdateStatusDisplay();
        }

        /// <summary>
        /// 顔文字表示を更新する
        /// </summary>
        void BuildKaomojiDisplay()
        {
            KAOMOJI K = modulesCtrl.PlayerData.Kaomoji;
            string builtKaomoji = K.BuildKaomoji(modulesCtrl.PlayerData.Status.mentalData);
            kaomoji.text = builtKaomoji;
        }

        /// <summary>
        /// ステータス表示を更新する
        /// </summary>
        void UpdateStatusDisplay()
        {
            KAOMOJI K = modulesCtrl.PlayerData.Kaomoji;
            int equippedPartsCount = K.GetEquippedPartsCount();
            K.UpdateTotalParameter();
            statusParamater.TotalShowStatus(K.Speed, K.Power, K.Guard, K.Stamina, equippedPartsCount);
        }

        public void Reset()
        {
            statusParamater.Reset();
            kaomoji.text = string.Empty;
        }
    }
}