namespace UI.TotalResult.Module
{
    using UnityEngine.UIElements;
    using UI.TotalResult.Temp;
    using Constants;
    using UnityEngine;
    
    public class KaomojiCompositions : MonoBehaviour, IUIModuleHandler
    {
        TemplateContainer[] compositions;
        public void Initialize(VisualElement moduleRoot)
        {
            compositions = moduleRoot.Query<TemplateContainer>("composition").ToList().ToArray();
        }

        public void UpdateUI(KAOMOJI kaomoji)
        {
            KaomojiPartData[] datas = kaomoji.GetAllPartsData();
            for (int i = 0; i < compositions.Length; i++)
            {
                VisualElement comp = compositions[i];
                if(datas[i] == null)
                {
                    comp.style.display = DisplayStyle.None;
                    continue;
                }
                new KaomojiComposition(comp, datas[i].PartType, datas[i].Data.levelDetail.Level, datas[i].Data.part);
            }
        }
        
    }
}