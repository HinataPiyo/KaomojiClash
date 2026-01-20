using UnityEngine;
using UI.Base;

namespace UI.Main
{
    /// <summary>
    /// パネル切り替えを管理するクラス
    /// </summary>
    public class PanelChangeManager : MonoBehaviour
    {
        public static PanelChangeManager I { get; private set; }

        [System.Serializable]
        public class PanelInfo
        {
            public ENUM.Panel panelType;
            public GameObject panelObject;
        }

        [SerializeField] PanelInfo[] panels;

        void Awake()
        {
            if(I == null)
            {
                I = this;
            }

            Change(ENUM.Panel.KaomojiBuild);
        }

        /// <summary>
        /// パネルを切り替える
        /// </summary>
        /// <param name="next">次に表示するパネルが渡される</param>
        public void Change(ENUM.Panel next)
        {
            foreach(var panel in panels)
            {
                if(panel.panelType == next)
                {
                    panel.panelObject.SetActive(true);
                }
                else
                {
                    panel.panelObject.SetActive(false);
                }
            }
        }
    }
}