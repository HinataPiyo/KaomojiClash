using System.Collections;
using Constants.Global;
using ENUM;
using UnityEngine;

public class EnemyMovement : Movement, IEnemyInitialize
{
    EnemyData data;
    [SerializeField] EnemyFindPlayer findPlayer;

    bool isInput;      // 発射可能フラグ
    bool isLaunch;     // 発射中フラグ

    Coroutine launchRoutine;

    public Difficulty dif { get; private set; }

    public void EnemyInitialize(EnemyData data, Difficulty dif)
    {
        this.data = data;
        this.dif = dif;
        isInput = false;
        isLaunch = false;
    }

    public void InitLaunchRoutine()
    {
        launchRoutine = StartCoroutine(LaunchRoutine());
    }

    IEnumerator LaunchRoutine()
    {
        // 戦闘開始の合図があるまで待機
        yield return new WaitUntil(() => Context.I.BattleStat == ENUM.BattleStat.Now);
        isInput = true;

        // 発射までの待機時間（エディタで設定可能）
        yield return new WaitForSeconds(data.LaunchWaitTime);
        isLaunch = true;
    }

    /// <summary>
    /// 待機中の入力処理
    /// </summary>
    protected override void HandleIdleInput()
    {
        if(!isInput || !findPlayer.IsEncount) return;
        dragStartWorld = transform.position;
        shootDirectionArrow = WorldCanvasManager.I.CreateShootDirectionArrow(transform.position);
        state = State.Dragging;
        isInput = false;
    }

    /// <summary>
    /// ドラッグ中の入力処理
    /// </summary>
    protected override void HandleDraggingInput()
    {
        if(Context.I.Player == null) return;
        
        Vector2 directionToPlayer = Context.I.Player.transform.position - transform.position;
        Vector2 dragVector = -directionToPlayer.normalized * data.Status.maxDragDistance;

        WorldCanvasManager.I.ShowShootDirectionArrow(shootDirectionArrow, transform.position, dragVector, dragVector.magnitude);

        if (isLaunch)
        {
            if (dragVector.magnitude < data.Status.minLaunchDistance)
            {
                state = State.Idle;
                return;
            }

            Vector2 launchDir = -dragVector.normalized;
            float powerRate = Mathf.Clamp01(dragVector.magnitude / data.Status.maxDragDistance);

            CameraZoom.I.SetCameraOrthographic(Context.I.BattleStat);
            shootDirectionArrow.Del();
            
            // 速度計算
            float speed = AreaManager.I.GetStatusParamByCultureLevel(StatusType.Speed, data.Status.speed) 
                        * Calculation.GetDifficultyRate(dif)
                        * powerRate;
            
            Launch(launchDir * speed);

            isLaunch = false;
        }
    }

    protected override IEnumerator LandingCooldownRoutine()
    {
        state = State.Cooldown;

        // 発射後の硬直時間（エディタで設定可能）
        yield return new WaitForSeconds(data.LaunchCooldown);

        state = State.Idle;
        cooldown = null;

        // 次の発射までの間隔（エディタで設定可能）
        yield return new WaitForSeconds(data.LaunchInterval);
        
        StartCoroutine(LaunchRoutine());
    }
}