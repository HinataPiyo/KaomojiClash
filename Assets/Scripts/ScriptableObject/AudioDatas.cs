using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDatas", menuName = "AudioDatas", order = 0)]
public class AudioDatas : ScriptableObject
{
    [System.Serializable]
    public class Clip
    {
        public string tag;
        public AudioClip clip;
    }

    [SerializeField] List<Clip> seClips = new List<Clip>();

    public AudioClip GetSEClip(string tag)
    {
        foreach (var clip in seClips)
        {
            if (clip.tag == tag)
            {
                return clip.clip;
            }
        }
        return null;
    }
}