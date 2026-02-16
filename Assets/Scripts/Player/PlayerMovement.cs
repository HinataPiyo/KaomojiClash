using System.Collections;
using UnityEngine;

public class PlayerMovement : Movement
{
    /// <summary>
    /// プレイヤー固有ステータス適用コンポーネント。
    /// 現時点では Initialize 時に取得して保持している。
    /// </summary>
    PlayerApplyKaomoji totalStatus;

    /// <summary>
    /// プレイヤーの各種移動パラメータ（最小/最大ドラッグ距離、硬直時間など）。
    /// </summary>
    PlayerData data;

    enum DragInputSource
    {
        Mouse
    }

    // DragInputSource dragInputSource;

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
        // マウス開始を優先。
        // 同フレームで両入力が来た場合に意図しない競合を避ける。
        TryStartMouseDrag();
    }

    /// <summary>
    /// ドラッグ中の入力処理
    /// </summary>
    protected override void HandleDraggingInput()
    {
        // 入力デバイスに応じた現在ドラッグ量を取得。
        Vector2 dragVector = GetMouseWorldPos() - dragStartWorld;
        dragVector = ClampDragDistance(dragVector);

        // 照準UI（ライン・矢印・カメラズーム）を毎フレーム更新。
        UpdateAimVisuals(dragVector);

        // 発射トリガー待ち。
        // Mouse: ボタンを離す
        // Gamepad: LB を押す（ただし右Stickが倒れている時のみ有効）
        if (!IsReleaseTriggered()) return;

        EndAimVisuals();

        if (!TryPrepareLaunchVector(ref dragVector))
        {
            // 十分な入力量がない場合は発射せずに待機へ戻す。
            CancelDragging();
            return;
        }

        LaunchByDragVector(dragVector);
    }

    void TryStartMouseDrag()
    {
        // 左クリック押下でドラッグ開始。
        if (!Input.GetMouseButtonDown(0)) return;

        dragStartWorld = GetMouseWorldPos();
        // dragInputSource = DragInputSource.Mouse;
        state = State.Dragging;

        BeginAimVisuals();
    }

    Vector2 ClampDragDistance(Vector2 dragVector)
    {
        if (dragVector.magnitude <= data.Status.maxDragDistance) return dragVector;
        return dragVector.normalized * data.Status.maxDragDistance;
    }

    void BeginAimVisuals()
    {
        shootDirectionArrow = WorldCanvasManager.I.CreateShootDirectionArrow(transform.position);

        if (aimLine != null)
        {
            aimLine.positionCount = 2;
        }
    }

    void UpdateAimVisuals(Vector2 dragVector)
    {
        if (aimLine != null)
        {
            Vector3 start = dragStartWorld;
            Vector3 end = dragStartWorld + dragVector;
            aimLine.SetPosition(0, start);
            aimLine.SetPosition(1, end);
        }

        CameraZoom.I.ApplyZoomByDrag(dragVector);
        WorldCanvasManager.I.ShowShootDirectionArrow(shootDirectionArrow, transform.position, dragVector, dragVector.magnitude);
    }

    void EndAimVisuals()
    {
        if (aimLine != null)
        {
            aimLine.positionCount = 0;
        }
    }

    void CancelDragging()
    {
        state = State.Idle;
        if (shootDirectionArrow != null) shootDirectionArrow.Del();
    }

    bool IsReleaseTriggered()
    {
        return Input.GetMouseButtonUp(0);
    }

    bool TryPrepareLaunchVector(ref Vector2 dragVector)
    {
        // 最小発射距離を満たしていればそのまま発射可能。
        if (dragVector.magnitude >= data.Status.minLaunchDistance) return true;

        return false;
    }

    void LaunchByDragVector(Vector2 dragVector)
    {
        // スリングショット挙動: ドラッグ方向の逆向きに飛ばす。
        Vector2 launchDir = -dragVector.normalized;
        float powerRate = Mathf.Clamp01(dragVector.magnitude / data.Status.maxDragDistance);
        float playerSpeed = Context.I.GetPlayerSpeed();
        float launchSpeed = (playerSpeed + (playerSpeed * speedupRate)) * powerRate;

        Launch(launchDir * launchSpeed);
        CameraZoom.I.SetCameraOrthographic(Context.I.BattleStat);

        if (shootDirectionArrow != null)
        {
            shootDirectionArrow.Del();
        }
    }

    protected override IEnumerator LandingCooldownRoutine()
    {
        state = State.Cooldown;

        // 発射後の硬直時間。
        yield return new WaitForSeconds(data.landingCooldown);

        state = State.Idle;
        cooldown = null;
    }
}
