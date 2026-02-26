using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Animation))]
    public class ToggleAnimator : MonoBehaviour
    {
        private Animation _animator;
        [SerializeField] private AnimationClip entry_anim;
        [SerializeField] private AnimationClip exit_anim;

        [SerializeField] private bool toggle;

        private void Awake()
        {
            _animator = GetComponent<Animation>();
        }

        public void SetToggle(bool value)
        {
            if (value)
            {
                PlayOn();
            }
            else
            {
                PlayOff();
            }

            toggle = value;
        }

        public void PlayOn()
        {
            if (toggle)
                return;

            _animator.Play(entry_anim.name);
        }

        public void PlayOff()
        {
            if (!toggle)
                return;

            _animator.Play(exit_anim.name);
        }
    }
}
