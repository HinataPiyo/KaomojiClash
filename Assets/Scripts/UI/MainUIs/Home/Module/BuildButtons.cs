using UnityEngine;
using UnityEngine.UIElements;
namespace UI.Home.Module
{    
    public class BuildButtons : MonoBehaviour, IUIModuleHandler
    {
        NextPanelButton kaomoji_bb;
        NextPanelButton arena_bb;
        public void Initialize(VisualElement moduleRoot)
        {
            Button kaomojiBuildButton = moduleRoot.Q<VisualElement>("KaomojiBuildButton").Q<Button>();
            Button arenaBuildButton = moduleRoot.Q<VisualElement>("ArenaBuildButton").Q<Button>();

            if(kaomojiBuildButton == null) Debug.Log("kaomojiBuildButton is null");
            kaomoji_bb = new NextPanelButton(ENUM.Panel.KaomojiBuild, kaomojiBuildButton);
            arena_bb = new NextPanelButton(ENUM.Panel.ArenaBuild, arenaBuildButton);
        }
    }
}