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

    [SerializeField, Range(0.1f, 1f)] float freeDragMaxDistanceRate = 0.5f; // data.IsMouseClickDrag=false時の最大ドラッグ距離倍率
    Coroutine moveToNextEnemyRoutine;

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
        TryStartMouseDrag();
    }

    /// <summary>
    /// ドラッグ中の入力処理
    /// </summary>
    protected override void HandleDraggingInput()
    {
        Vector2 currentDragStart = GetCurrentDragStart();

        // 入力デバイスに応じた現在ドラッグ量を取得。
        Vector2 dragVector = GetMouseWorldPos() - currentDragStart;
        dragVector = ClampDragDistance(dragVector);

        // 照準UI（ライン・矢印・カメラズーム）を毎フレーム更新。
        UpdateAimVisuals(currentDragStart, dragVector);

        if (!IsLaunchTriggered()) return;

        if (!TryPrepareLaunchVector(ref dragVector))
        {
            if (data.IsMouseClickDrag)
            {
                // クリック→ドラッグ→離しモードでは不成立時にキャンセル
                EndAimVisuals();
                CancelDragging();
            }

            // ドラッグ→クリックモードでは照準継続
            return;
        }

        EndAimVisuals();
        LaunchByDragVector(dragVector);
    }

    void TryStartMouseDrag()
    {
        if (data.IsMouseClickDrag)
        {
            // クリック開始でドラッグ開始
            if (!Input.GetMouseButtonDown(0)) return;
            dragStartWorld = GetMouseWorldPos();
        }
        else
        {
            // クリック不要で常時照準（開始位置はプレイヤー位置固定）
            dragStartWorld = transform.position;
        }

        state = State.Dragging;

        BeginAimVisuals();
    }

    Vector2 ClampDragDistance(Vector2 dragVector)
    {
        float maxDragDistance = GetCurrentMaxDragDistance();
        if (dragVector.magnitude <= maxDragDistance) return dragVector;
        return dragVector.normalized * maxDragDistance;
    }

    float GetCurrentMaxDragDistance()
    {
        if (data.IsMouseClickDrag)
        {
            return data.Status.maxDragDistance;
        }

        return data.Status.maxDragDistance * freeDragMaxDistanceRate;
    }

    void BeginAimVisuals()
    {
        shootDirectionArrow = WorldCanvasManager.I.CreateShootDirectionArrow(transform.position);

        if (aimLine != null)
        {
            aimLine.positionCount = 2;
        }
    }

    void UpdateAimVisuals(Vector2 startWorld, Vector2 dragVector)
    {
        if (aimLine != null)
        {
            Vector3 start = startWorld;
            Vector3 end = startWorld + dragVector;
            aimLine.SetPosition(0, start);
            aimLine.SetPosition(1, end);
        }

        CameraZoom.I.ApplyZoomByDrag(dragVector);

        float powerRate = GetPowerRateFromDrag(dragVector);
        float arrowMagnitude = data.Status.maxDragDistance * powerRate;
        Vector2 arrowVector = dragVector.sqrMagnitude > 0f
            ? dragVector.normalized * arrowMagnitude
            : Vector2.zero;

        // freeDragモード時は矢印の見た目向きが逆になるため、表示用ベクトルのみ反転する。
        if (!data.IsMouseClickDrag)
        {
            arrowVector = -arrowVector;
        }

        WorldCanvasManager.I.ShowShootDirectionArrow(shootDirectionArrow, transform.position, arrowVector, arrowMagnitude);
    }

    Vector2 GetCurrentDragStart()
    {
        if (!data.IsMouseClickDrag)
        {
            return transform.position;
        }

        return dragStartWorld;
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

    bool IsLaunchTriggered()
    {
        if (data.IsMouseClickDrag)
        {
            return Input.GetMouseButtonUp(0);
        }

        return Input.GetMouseButtonDown(0);
    }

    bool TryPrepareLaunchVector(ref Vector2 dragVector)
    {
        // 最小発射距離を満たしていればそのまま発射可能。
        if (dragVector.magnitude >= data.Status.minLaunchDistance) return true;

        return false;
    }

    void LaunchByDragVector(Vector2 dragVector)
    {
        // data.IsMouseClickDrag=true  : スリングショット挙動（逆向き）
        // data.IsMouseClickDrag=false : カーソル方向へそのまま飛ばす
        Vector2 launchDir = data.IsMouseClickDrag
            ? -dragVector.normalized
            : dragVector.normalized;
        float powerRate = GetPowerRateFromDrag(dragVector);
        float playerSpeed = Context.I.GetPlayerSpeed();
        float launchSpeed = (playerSpeed + (playerSpeed * speedupRate)) * powerRate;

        Launch(launchDir * launchSpeed);
        CameraZoom.I.SetCameraOrthographic(Context.I.BattleStat);

        if (shootDirectionArrow != null)
        {
            shootDirectionArrow.Del();
        }
    }

    float GetPowerRateFromDrag(Vector2 dragVector)
    {
        float powerRate = dragVector.magnitude / data.Status.maxDragDistance;

        // freeDrag時は最大距離を縮めているため、威力は倍率で補正して
        // 縮小後の上限距離でも最大威力(=1.0)に到達できるようにする。
        if (!data.IsMouseClickDrag)
        {
            powerRate /= Mathf.Max(freeDragMaxDistanceRate, 0.0001f);
        }

        return Mathf.Clamp01(powerRate);
    }

    protected override IEnumerator LandingCooldownRoutine()
    {
        state = State.Cooldown;

        // 発射後の硬直時間。
        yield return new WaitForSeconds(data.landingCooldown);

        state = State.Idle;
        cooldown = null;
    }

    public void StartMoveToNextEnemy(Vector2 nextEnemyPos)
    {
        if (cooldown != null)
        {
            StopCoroutine(cooldown);
            cooldown = null;
        }

        if (shootDirectionArrow != null)
        {
            shootDirectionArrow.Del();
        }

        if (aimLine != null)
        {
            aimLine.positionCount = 0;
        }

        state = State.MoveToNextEnemy;

        if (moveToNextEnemyRoutine != null)
        {
            StopCoroutine(moveToNextEnemyRoutine);
        }

        AudioManager.I.PlayBGM("NextMoveToEnemy");
        moveToNextEnemyRoutine = StartCoroutine(MoveToNextEnemyRoutine(nextEnemyPos));
    }

    IEnumerator MoveToNextEnemyRoutine(Vector2 nextEnemyPos)
    {
        yield return new WaitForSeconds(1f);
        // 次の敵に移動する際の演出や処理をここに実装
        // 例: プレイヤーを次の敵に向かって移動させる、特殊なエフェクトを再生するなど

        // 仮の移動処理（例: 1秒かけて次の敵に移動）
        float moveDuration = 3f;
        Vector2 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            if(state != State.MoveToNextEnemy)
            {
                // 状態が変わったら移動を中断
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);;
            Vector2 newPos = Vector2.Lerp(startPos, nextEnemyPos, t);
            transform.position = newPos;
            Debug.Log($"Moving to next enemy... {t * 100f:0.0}%");
            yield return null;
        }

        moveToNextEnemyRoutine = null;
    }

    
}
