namespace UI
{
    using TMPro;
    using UnityEngine;
    
    public class MessageUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        public void SetMessage(string message)
        {
            text.text = message;
        }
    }
}