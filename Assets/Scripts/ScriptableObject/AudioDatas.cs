using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDatas", menuName = "Config/AudioDatas")]
public class AudioDatas : ScriptableObject
{
    [System.Serializable]
    public class Clip
    {
        public string tag;
        public AudioClip clip;
    }

    [SerializeField] List<Clip> clips = new List<Clip>();

    public AudioClip GetAudioClip(string tag)
    {
        foreach (var clip in clips)
        {
            if (clip.tag == tag)
            {
                return clip.clip;
            }
        }
        return null;
    }
}