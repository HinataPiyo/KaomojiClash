using Constants;
using UI.KaomojiBuild.Template;
using UnityEngine;
using UnityEngine.UIElements;
using ENUM;
using System.Collections.Generic;

namespace UI.KaomojiBuild.Module
{
    public class SelectedDisplay : MonoBehaviour, IUIModuleHandler, IUIPartHandler
    {
        [SerializeField] VisualTreeAsset temp_SkillTag;
        KaomojiBuildModulesController modulesCtrl;
        StatusParamater statusParamater;

        VisualElement skilltagsContainer;
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
            skilltagsContainer = moduleRoot.Q<VisualElement>("parts-skill-tags-box").Q<VisualElement>("container");

            Reset();
            KAOMOJI K = modulesCtrl.PlayerData.Kaomoji;
            BuildKaomojiDisplay();
            UpdateStatusDisplay();
            RefreshSkillTags(K);
        }

        /// <summary>
        /// パーツを割り当てる
        /// </summary>
        /// <param name="part"></param>
        public void AssignPart(KaomojiPartData part)
        {
            Reset();
            KAOMOJI K = modulesCtrl.PlayerData.Kaomoji;
            K.SetPartDataByType(part);

            BuildKaomojiDisplay();
            UpdateStatusDisplay();
            RefreshSkillTags(K);
        }

        /// <summary>
        /// スキルタグ表示を更新する
        /// </summary>
        /// <param name="K"></param>
        void RefreshSkillTags(KAOMOJI K)
        {
            // スキルタグ表示更新
            List<SkillTag.Stack> tags = PlayerUpgradeService.SetTags(K.GetAllSkillTags());
            foreach (SkillTag.Stack elem in tags)
            {
                VisualElement tempRoot = temp_SkillTag.Instantiate();
                new SkillTagUI(tempRoot, elem.tag, elem.stackCount);
                skilltagsContainer.Add(tempRoot);
            }
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
            int equipmentCount = K.GetEquippedPartsCount();
            statusParamater.TotalShowStatus(K.GetInitialParam(StatusType.Speed), K.GetInitialParam(StatusType.Power),
                                            K.GetInitialParam(StatusType.Guard), K.GetInitialParam(StatusType.Stamina), equipmentCount);
        }

        public void Reset()
        {
            statusParamater.Reset();
            kaomoji.text = string.Empty;
            skilltagsContainer.Clear();
        }
    }
}