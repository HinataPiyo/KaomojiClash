using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public static CameraZoom I { get; private set; }
    static readonly float DefaultBattleCamSize = 3.3f;
    static readonly float DefaultSearchCamSize = 5f;
    [SerializeField] float zoomFactor = 0.1f; // ドラッグ距離に対するズーム倍率
    [SerializeField] float smoothTime = 0.15f;   // 滑らかに戻る時間

    CinemachineCamera cam;
    CinemachinePositionComposer cpc;
    Coroutine zoomRoutine;
    float targetSize;
    float zoomVelocity; // SmoothDamp 用

    void Awake()
    {
        if (I == null) I = this;

        cam = GetComponent<CinemachineCamera>();
        cpc = GetComponent<CinemachinePositionComposer>();

        ResetSearchZoom();
    }

    void LateUpdate()
    {
        // 現在値 → targetSize へ滑らかに補間
        float current = cam.Lens.OrthographicSize;
        float next = Mathf.SmoothDamp(current, targetSize, ref zoomVelocity, smoothTime);
        cam.Lens.OrthographicSize = next;
    }

    /// <summary>
    /// ドラッグしたときにZOOMする
    /// </summary>
    /// <param name="dragDis"></param>
    public void ApplyZoomByDrag(Vector2 dragDis)
    {
        if(zoomRoutine != null)
        {
            return;
        }
        
        // ここでは「目標値」を更新するだけにする
        if(Context.I.BattleStat == ENUM.BattleStat.None)
        {
            targetSize = DefaultSearchCamSize - dragDis.magnitude * zoomFactor * 0.8f;
        }
        else if(Context.I.BattleStat == ENUM.BattleStat.Now)
        {
            targetSize = DefaultBattleCamSize - dragDis.magnitude * zoomFactor;
        }
    }

    /// <summary>
    /// ただ単にZOOMする
    /// </summary>
    /// <param name="factor"></param>
    /// <param name="duration"></param>
    public void ApplyZoom(float factor, float duration = 0.2f)
    {
        if(zoomRoutine != null)
        {
            return;
        }

        zoomRoutine = StartCoroutine(ZoomRoutine(factor, duration));
    }

    /// <summary>
    /// 敵をKILLしたときにZOOMする
    /// </summary>
    public void EnemyKilledZoom()
    {
        if(zoomRoutine != null)
        {
            StopCoroutine(zoomRoutine);
        }

        zoomRoutine = StartCoroutine(ZoomRoutine(2f, 0.3f));
    }

    public void InitSetCameraOrthographic(ENUM.BattleStat stat)
    {
        switch (stat)
        {
            case ENUM.BattleStat.None:
            case ENUM.BattleStat.End:
                cpc.Composition.DeadZone.Enabled = false;
                ResetSearchZoom();
                break;
            case ENUM.BattleStat.Start:
            case ENUM.BattleStat.Now:
                cpc.Composition.DeadZone.Enabled = true;
                cpc.Composition.DeadZone.Size = new Vector2(0.2f, 0.2f);
                ResetBattleZoom();
                break;
        }
    }

    public void SetCameraOrthographic(ENUM.BattleStat stat)
    {
        switch (stat)
        {
            case ENUM.BattleStat.None:
            case ENUM.BattleStat.End:
                ResetSearchZoom();
                break;
            case ENUM.BattleStat.Start:
            case ENUM.BattleStat.Now:
                ResetBattleZoom();
                break;
        }
    }

    /// <summary>
    /// デフォルトに戻す
    /// </summary>
    public void ResetSearchZoom()
    {
        // 目標値をデフォルトに戻す。あとは LateUpdate 側で自然に戻る
        targetSize = DefaultSearchCamSize;
    }

    public void ResetBattleZoom()
    {
        targetSize = DefaultBattleCamSize;
    }

    IEnumerator ZoomRoutine(float factor, float duration)
    {
        targetSize = factor;
        yield return new WaitForSeconds(duration);
        ResetBattleZoom();

        zoomRoutine = null;
    }

    public void StartEncountZoom(float factor, float duration)
    {
        if(zoomRoutine != null)
        {
            StopCoroutine(zoomRoutine);
        }

        zoomRoutine = StartCoroutine(EncountRoutine(factor, duration));
    }

    IEnumerator EncountRoutine(float factor, float duration)
    {
        targetSize = factor;
        yield return new WaitForSeconds(duration);
        ResetBattleZoom();

        zoomRoutine = null;
    }
}