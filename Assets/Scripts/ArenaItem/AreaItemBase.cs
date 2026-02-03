namespace ArenaItem
{
    using UnityEngine;

    public class AreaItemBase : MonoBehaviour
    {
        protected const string ANIM_TRIGGER_PLAY = "Play";
        protected Animator anim;

        void Awake()
        {
            anim = GetComponentInChildren<Animator>();
        }
    }
}