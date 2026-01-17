using UnityEngine;

public class StartWaveEffect : MonoBehaviour
{
    public void ReadyPlaySE()
    {
        AudioManager.I.PlaySE("READY");
    }

    public void FightPlaySE()
    {
        AudioManager.I.PlaySE("FIGHT");
    }
}