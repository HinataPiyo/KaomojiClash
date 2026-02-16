using UnityEngine;
using UI;

public class OverlayCanvasManager : MonoBehaviour
{
    public static OverlayCanvasManager I { get; private set; }

    [Header("メッセージテキスト")]
    [SerializeField] GameObject messageUI_Prefab;
    [SerializeField] GameObject skillTag_Prefab;
    Canvas canvas;

    void Awake()
    {
        if(I == null) I = this;
        canvas = GetComponent<Canvas>();
    }

    /// <summary>
    /// スキルタグの説明を表示
    /// </summary>
    public GameObject ShowSkillTagDescription(SkillTag tag, Vector2 uiToolkitScreenPos, bool isLevelDisplay = false)
    {
        GameObject obj = Instantiate(skillTag_Prefab, transform);
        RectTransform rect = obj.GetComponent<RectTransform>();
        // 座標変換
        Vector2 canvasPosition = ConvertUIToolkitToCanvasPosition(
            uiToolkitScreenPos, canvas, rect
        );

        Vector2 offsetY = new Vector2(0, -40);
        rect.anchoredPosition = canvasPosition + offsetY;

        SkillTagDescription skillTag = obj.GetComponent<SkillTagDescription>();
        skillTag.ShowDescription(tag.GetDescriptionsArray(), isLevelDisplay);

        return obj;
    }

    Vector2 ConvertUIToolkitToCanvasPosition(Vector2 uiToolkitPos, Canvas canvas, RectTransform targetRect)
    {
        // UIToolkitの座標系はスクリーン左上が原点(0,0)
        // Y軸は下向きが正
        Vector2 screenPoint = new Vector2(
            uiToolkitPos.x,
            Screen.height - uiToolkitPos.y // Y軸を反転
        );

        // スクリーン座標からCanvas座標へ変換
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        // 5. 座標変換
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        return localPoint;
    }


    /// <summary>
    /// メッセージを表示
    /// </summary>
    /// <param name="message"></param>
    public void ShowMessage(string message)
    {
        GameObject messageUI_Obj = Instantiate(messageUI_Prefab, transform);
        MessageUI messageUI = messageUI_Obj.GetComponent<MessageUI>();
        messageUI.SetMessage(message);
    }

}