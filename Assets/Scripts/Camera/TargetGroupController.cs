using Unity.Cinemachine;
using UnityEngine;

public class TargetGroupController : MonoBehaviour
{
    CinemachineTargetGroup targetGroup;

    void Awake()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    public void AddTarget(Transform target)
    {
        CinemachineTargetGroup.Target t = new CinemachineTargetGroup.Target
        {
            Object = target,
            Weight = 1f,
            Radius = 0.5f
        };

        targetGroup.Targets.Add(t);
    }

    public void RemoveTarget(Transform target)
    {
        targetGroup.RemoveMember(target);
    }

    public void AddTargets(Transform[] targets)
    {
        foreach(Transform t in targets)
        {
            AddTarget(t);
        }
    }
}