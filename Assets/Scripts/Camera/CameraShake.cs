using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake I { get; private set; }
    CinemachineBasicMultiChannelPerlin channel;


    void Awake()
    {
        if (I == null) I = this;
        channel = GetComponent<CinemachineBasicMultiChannelPerlin>();
        Reset();
    }

    /// <summary>
    /// 振動を発生させる
    /// </summary>
    /// <param name="amplitudeGain">振幅</param>
    /// <param name="frequencyGain">周波数</param>
    /// <param name="duration">持続時間</param>
    public void ApplyShake(float amplitudeGain, float frequencyGain, float duration)
    {
        StartCoroutine(ShakeFlow(amplitudeGain, frequencyGain, duration));
        Reset();
    }

    IEnumerator ShakeFlow(float amplitudeGain, float frequencyGain, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            channel.AmplitudeGain = amplitudeGain;
            channel.FrequencyGain = frequencyGain;

            time += Time.deltaTime;
            yield return null;
        }

        Reset();
    }
    
    void Reset()
    {
        channel.AmplitudeGain = 0;
        channel.FrequencyGain = 0;
    }
}