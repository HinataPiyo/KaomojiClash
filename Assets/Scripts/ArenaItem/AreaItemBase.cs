namespace ArenaItem
{
    using UnityEngine;

    public class ArenaItemBase : MonoBehaviour
    {
        protected const string ANIM_TRIGGER_PLAY = "Play";
        protected Animator anim;

        protected virtual void Awake()
        {
            anim = GetComponentInChildren<Animator>();
        }
    }
}