using Constants;
using UI.KaomojiBuild.Template;
using UnityEngine;
using UnityEngine.UIElements;
using ENUM;

namespace UI.Home.Module
{    
    public class KaomojiPreview : MonoBehaviour, IUIModuleHandler
    {
        HomeModulesController homeCtrl;
        StatusParamater statusParamater;

        Label kaomoji_preview;

        /// <summary>
        /// モジュールの初期化を行う
        /// </summary>
        /// <param name="moduleRoot">ControllerからModuleのRootを渡す</param>
        public void Initialize(VisualElement moduleRoot)
        {
            homeCtrl = GetComponent<HomeModulesController>();
            statusParamater = new StatusParamater();
            statusParamater.Initialize(moduleRoot);

            kaomoji_preview = moduleRoot.Q<VisualElement>("kaomoji-preview").Q<Label>("value");
            ChangeKaomojiDisplay();     // 顔文字の表示を更新する
        }

        /// <summary>
        /// 顔文字表示を更新する
        /// </summary>
        public void ChangeKaomojiDisplay()
        {
            KAOMOJI K = homeCtrl.PlayerData.Kaomoji;
            // 装備されている記号を合体させ顔文字を取得する
            string builtKaomoji = K.BuildKaomoji(homeCtrl.PlayerData.Status.mentalData);
            kaomoji_preview.text = builtKaomoji;

            int equipmentCount = K.GetEquippedPartsCount();     // 装備されている記号パーツの数を取得s
            // ステータス表示も更新(Progressbar)
            statusParamater.TotalShowStatus(K.GetInitialParam(StatusType.Speed), K.GetInitialParam(StatusType.Power),
                                            K.GetInitialParam(StatusType.Guard), K.GetInitialParam(StatusType.Stamina), equipmentCount);
        }
    }
}