using System.Collections;
using UnityEngine;

public class PlayerMovement : CharacterMoveBase
{
    /// <summary>
    /// 待機中の入力処理
    /// </summary>
    protected override void HandleIdleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartWorld = GetMouseWorldPos();
            state = State.Dragging;

            if (aimLine != null)
            {
                aimLine.positionCount = 2;
            }
        }
    }

    /// <summary>
    /// ドラッグ中の入力処理
    /// </summary>
    protected override void HandleDraggingInput()
    {
        Vector2 currentWorld = GetMouseWorldPos();
        Vector2 dragVector = currentWorld - dragStartWorld;

        // 最大ドラッグ距離を制限
        if (dragVector.magnitude > maxDragDistance)
        {
            dragVector = dragVector.normalized * maxDragDistance;
        }

        // LineRendererで方向表示（スリングショットっぽく逆向きに伸ばす）
        if (aimLine != null)
        {
            Vector3 start = dragStartWorld;
            Vector3 end = dragStartWorld + dragVector;
            aimLine.SetPosition(0, start);
            aimLine.SetPosition(1, end);
        }

        CameraZoom.I.ApplyZoom(dragVector);

        // ボタンを離したら発射
        if (Input.GetMouseButtonUp(0))
        {
            if (aimLine != null)
            {
                aimLine.positionCount = 0;
            }

            if (dragVector.magnitude < minLaunchDistance)
            {
                // ほぼ動いてない → 発射キャンセル
                state = State.Idle;
                return;
            }

            // スリングショットなのでドラッグ方向の逆に飛ばす
            Vector2 launchDir = -dragVector.normalized;

            // 距離に応じて速度をスケール
            float powerRate = Mathf.Clamp01(dragVector.magnitude / maxDragDistance);
            float launchSpeed = launchPower * powerRate;

            Launch(launchDir * launchSpeed);
            CameraZoom.I.ResetZoom();
        }
    }

}
