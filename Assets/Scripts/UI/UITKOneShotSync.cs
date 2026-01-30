using UnityEngine;
using UnityEngine.UIElements;

public static class UITKOneShotSync
{
    public static void SnapToWorldOnce(VisualElement target, Vector3 worldPos, Camera cam, bool centerPivot = true)
    {
        if (target == null || cam == null) return;

        // Panel未接続のタイミングだと変換不能
        if (target.panel == null) return;

        // World -> Panel(2D)
        Vector2 panelPos = RuntimePanelUtils.CameraTransformWorldToPanel(target.panel, worldPos, cam);

        // Panel -> Parent Local（style.left/top は親基準）
        var parent = target.parent;
        if (parent == null) return;

        Vector2 local = parent.WorldToLocal(panelPos);

        target.style.position = Position.Absolute;

        if (centerPivot)
        {
            float w = target.resolvedStyle.width;
            float h = target.resolvedStyle.height;
            local.x -= w * 0.5f;
            local.y -= h * 0.5f;
        }

        target.style.left = local.x;
        target.style.top  = local.y;
    }
}
