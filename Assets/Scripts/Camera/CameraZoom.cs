using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public static CameraZoom I { get; private set; }
    static readonly float DefaultOrthographicSize = 3.3f;
    [SerializeField] float zoomFactor = 0.1f; // ドラッグ距離に対するズーム倍率
    [SerializeField] float smoothTime = 0.15f;   // 滑らかに戻る時間

    CinemachineCamera cam;
    Coroutine zoomRoutine;
    float targetSize;
    float zoomVelocity; // SmoothDamp 用

    void Awake()
    {
        if (I == null) I = this;

        cam = GetComponent<CinemachineCamera>();
        targetSize = DefaultOrthographicSize;
        cam.Lens.OrthographicSize = DefaultOrthographicSize;
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
        targetSize = DefaultOrthographicSize - dragDis.magnitude * zoomFactor;
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

    /// <summary>
    /// デフォルトに戻す
    /// </summary>
    public void ResetZoom()
    {
        // 目標値をデフォルトに戻す。あとは LateUpdate 側で自然に戻る
        targetSize = DefaultOrthographicSize;
    }

    IEnumerator ZoomRoutine(float factor, float duration)
    {
        targetSize = factor;
        yield return new WaitForSeconds(duration);
        ResetZoom();

        zoomRoutine = null;
    }
}