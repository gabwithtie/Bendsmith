using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioOneShotter : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private AudioClip clip;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            audioSource.PlayOneShotSafe(clip);
        }
    }
}
