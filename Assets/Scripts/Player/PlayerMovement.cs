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

    [SerializeField, Range(0.1f, 1f)] float freeDragMaxDistanceRate = 0.5f; // モバイル専用: 最大ドラッグ距離倍率
    Coroutine moveToNextEnemyRoutine;

    public void Initialize(PlayerData data)
    {
        totalStatus = GetComponent<PlayerApplyKaomoji>();
        this.data = data;
        // IsMouseClickDragはモバイル専用化で不要のため参照しない
    }

    /// <summary>
    /// 待機中の入力処理
    /// </summary>
    protected override void HandleIdleInput()
    {
        TryStartTouchDrag();
    }


    protected override void HandleDraggingInput()
    {
        // タップ開始位置を中心にドラッグ距離を計算
        Vector2 currentDragStart = dragStartWorld;
        Vector2 currentTouchPos = GetTouchWorldPos();
        Vector2 dragVector = currentTouchPos - currentDragStart;
        Debug.Log($"CurrentTouchPos: {currentTouchPos}, DragStart: {currentDragStart}, DragVector: {dragVector}");
        dragVector = ClampDragDistance(-dragVector);

        UpdateAimVisuals(currentDragStart, dragVector);

        if (!IsTouchEndedThisFrame()) return;
        if (!TryPrepareLaunchVector(ref dragVector))
        {
            EndAimVisuals();
            CancelDragging();
            return;
        }

        EndAimVisuals();
        LaunchByDragVector(dragVector);
    }

    // タッチ位置をワールド座標に変換
    Vector2 GetTouchWorldPos()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            Vector3 screenPos = touch.position;
            return Camera.main.ScreenToWorldPoint(screenPos);
        }
        // タッチがなければ現在位置を返す
        return transform.position;
    }

    // タッチがこのフレームで終了したか判定
    bool IsTouchEndedThisFrame()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                return true;
        }
        return false;
    }

    void TryStartTouchDrag()
    {
        // モバイル専用: タッチ開始時のみドラッグ開始
        if (!IsTouchBeganThisFrame()) return;
        // タップ開始時のワールド座標を原点に
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            Vector3 screenPos = touch.position;
            dragStartWorld = Camera.main.ScreenToWorldPoint(screenPos);
        }
        else
        {
            dragStartWorld = transform.position;
        }
        state = State.Dragging;
        BeginAimVisuals();
    }

    // タッチがこのフレームで始まったか判定
    bool IsTouchBeganThisFrame()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
                return true;
        }
        return false;
    }

    /// <summary>
    /// ドラッグ距離を最大値でクランプ。
    /// モバイル専用: freeDragMaxDistanceRateを考慮してクランプする
    /// </summary>
    /// <param name="dragVector"></param>
    /// <returns></returns>
    Vector2 ClampDragDistance(Vector2 dragVector)
    {
        float maxDragDistance = GetCurrentMaxDragDistance();
        if (dragVector.magnitude <= maxDragDistance) return dragVector;
        return dragVector.normalized * maxDragDistance;
    }

    float GetCurrentMaxDragDistance()
    {
        // モバイル専用: freeDragMaxDistanceRateを常に適用
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

        // モバイル専用: 矢印の見た目向きは常に反転
        arrowVector = -arrowVector;

        WorldCanvasManager.I.ShowShootDirectionArrow(shootDirectionArrow, transform.position, arrowVector, arrowMagnitude);
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

    /// <summary>
    /// ドラッグベクトルを元に発射準備ができているか判定。
    /// 最小発射距離を満たしていれば発射可能とする。
    /// </summary>
    /// <param name="dragVector"></param>
    /// <returns></returns>
    bool TryPrepareLaunchVector(ref Vector2 dragVector)
    {
        // 最小発射距離を満たしていればそのまま発射可能。
        if (dragVector.magnitude >= data.Status.minLaunchDistance) return true;
        return false;
    }

    void LaunchByDragVector(Vector2 dragVector)
    {
        // モバイル専用: カーソル方向へそのまま飛ばす（常に正方向）
        Vector2 launchDir = dragVector.normalized;
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
        // モバイル専用: freeDragMaxDistanceRateで補正
        powerRate /= Mathf.Max(freeDragMaxDistanceRate, 0.0001f);
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
            if (state != State.MoveToNextEnemy)
            {
                // 状態が変わったら移動を中断
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration); ;
            Vector2 newPos = Vector2.Lerp(startPos, nextEnemyPos, t);
            transform.position = newPos;
            yield return null;
        }

        moveToNextEnemyRoutine = null;
    }

}
