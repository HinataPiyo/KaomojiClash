using System.Collections;
using UnityEngine;

public class PlayerMovement : Movement
{
    PlayerApplyKaomoji totalStatus;
    PlayerData data;

    public void Initialize(PlayerData data)
    {
        totalStatus = GetComponent<PlayerApplyKaomoji>();
        this.data = data;
    }

    /// <summary>
    /// 待機中の入力処理
    /// </summary>
    protected override void HandleIdleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartWorld = GetMouseWorldPos();
            state = State.Dragging;

            shootDirectionArrow = WorldCanvasManager.I.CreateShootDirectionArrow(transform.position);

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
        if (dragVector.magnitude > data.Status.maxDragDistance)
        {
            dragVector = dragVector.normalized * data.Status.maxDragDistance;
        }

        // LineRendererで方向表示（スリングショットっぽく逆向きに伸ばす）
        if (aimLine != null)
        {
            Vector3 start = dragStartWorld;
            Vector3 end = dragStartWorld + dragVector;
            aimLine.SetPosition(0, start);
            aimLine.SetPosition(1, end);
        }

        CameraZoom.I.ApplyZoomByDrag(dragVector);
        WorldCanvasManager.I.ShowShootDirectionArrow(shootDirectionArrow, transform.position, dragVector, dragVector.magnitude);

        // ボタンを離したら発射
        if (Input.GetMouseButtonUp(0))
        {
            if (aimLine != null)
            {
                aimLine.positionCount = 0;
            }

            if (dragVector.magnitude < data.Status.minLaunchDistance)
            {
                // ほぼ動いてない → 発射キャンセル
                state = State.Idle;
                shootDirectionArrow.Del();
                return;
            }

            // スリングショットなのでドラッグ方向の逆に飛ばす
            Vector2 launchDir = -dragVector.normalized;

            // 距離に応じて速度をスケール
            float powerRate = Mathf.Clamp01(dragVector.magnitude / data.Status.maxDragDistance);
            float launchSpeed = Context.I.GetPlayerSpeed() * powerRate;

            Launch(launchDir * launchSpeed);
            CameraZoom.I.SetCameraOrthographic(Context.I.BattleStat);
            shootDirectionArrow.Del();
        }
    }

    protected override IEnumerator LandingCooldownRoutine()
    {
        state = State.Cooldown;

        // 着地直後に速度を止める
        // rb.linearVelocity = Vector2.zero;

        // ここで「着地硬直中の顔文字」に切り替える
        // 例：faceController.SetLandingFace();

        yield return new WaitForSeconds(data.Status.landingCooldown);

        // 硬直終了 → Idleへ
        state = State.Idle;

        // 顔文字をIdleに戻すなど
        // faceController.SetIdleFace();

        cooldown = null;
    }

}
