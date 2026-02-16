using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeManager : MonoBehaviour
{
    public static GlobalVolumeManager I { get; private set; }
    [SerializeField] Volume globalVolume;

    Vignette vignette;
    Bloom bloom;
    LensDistortion lensDistortion;

    void Awake()
    {
        if( I == null) I = this;

        globalVolume.profile = Instantiate(globalVolume.profile);

        // Vignette が無ければ追加して初期化
        if (!globalVolume.profile.TryGet(out vignette) || vignette == null)
        {
            vignette = globalVolume.profile.Add<Vignette>(true);
            bloom = globalVolume.profile.Add<Bloom>(true);
            lensDistortion = globalVolume.profile.Add<LensDistortion>(true);

            lensDistortion.intensity.value = 0f;
            bloom.intensity.value = 0f;
            vignette.intensity.value = 0f;
        }
    }

    /// <summary>
    /// 被弾エフェクトを設定する
    /// </summary>
    public void HitFlashEffect()
    {
        StartCoroutine(FlashEffectRoutine(Color.red));
    }

    public void DieFlashEffect()
    {
        Color color = new Color(0.7f, 0.15f, 0.2f);
        StartCoroutine(FlashBloomEffectRoutine(color));
        StartCoroutine(LensDistortionEffectRoutine());
    }

    /// <summary>
    /// 被弾エフェクトコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator FlashEffectRoutine(Color32 color)
    {
        vignette.color.value = color;
        vignette.intensity.value = 0.3f;

        float duration = 0.5f;
        float elapsed = 0f;
        float initialIntensity = vignette.intensity.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(initialIntensity, 0f, elapsed / duration);
            yield return null;
        }

        vignette.intensity.value = 0f;
    }

    IEnumerator FlashBloomEffectRoutine(Color color)
    {
        // Bloom エフェクトを追加して初期化

        if (!globalVolume.profile.TryGet(out bloom) || bloom == null)
        {
            bloom = globalVolume.profile.Add<Bloom>(true);
            bloom.intensity.value = 0f;
        }

        bloom.tint.value = color;
        bloom.intensity.value = 0.2f;

        float duration = 0.5f;
        float elapsed = 0f;
        float initialIntensity = bloom.intensity.value;
        Color initialTint = bloom.tint.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            bloom.intensity.value = Mathf.Lerp(initialIntensity, 0f, t);
            bloom.tint.value = Color.Lerp(initialTint, Color.black, t);
            yield return null;
        }

        bloom.intensity.value = 0f;
        bloom.tint.value = Color.black;
    }

    IEnumerator LensDistortionEffectRoutine()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        float initialIntensity = -0.25f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            lensDistortion.intensity.value = Mathf.Lerp(initialIntensity, 0f, elapsed / duration);
            yield return null;
        }

        vignette.intensity.value = 0f;
    }
}