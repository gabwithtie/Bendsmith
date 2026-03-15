using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    public class AudioManager : Manager_Base<AudioManager>
    {
        struct SoundPlayedRecently
        {
            public AudioClip clip;
            public float age;
        }

        private List<SoundPlayedRecently> sounds_played_recently = new();
        [SerializeField] private float soundThreshold = 0.08f;

        [Header("Pooling Settings")]
        private Stack<AudioSource> audioPool = new Stack<AudioSource>();

        private void Update()
        {
            // Age tracking logic remains the same
            for (int i = 0; i < sounds_played_recently.Count; i++)
            {
                var currecent = sounds_played_recently[i];
                currecent.age += Time.deltaTime;
                sounds_played_recently[i] = currecent;

                if (sounds_played_recently[i].age > soundThreshold)
                {
                    sounds_played_recently.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Spawns a pooled AudioSource at a position with a specific 3D radius.
        /// </summary>
        public static void Spawn(AudioClip clip, Vector3 pos, float radius)
        {
            if (clip == null) return;

            AudioSource source;

            // 1. Get from pool or create new
            if (Instance.audioPool.Count > 0)
            {
                source = Instance.audioPool.Pop();
                source.gameObject.SetActive(true);
            }
            else
            {
                GameObject go = new GameObject("PooledAudioSource");
                source = go.AddComponent<AudioSource>();
            }

            // 2. Configure 3D properties
            source.transform.position = pos;
            source.clip = clip;
            source.spatialBlend = 1.0f; // Ensure it is 3D
            source.minDistance = radius * 0.1f; // Standard falloff start
            source.maxDistance = radius;
            source.rolloffMode = AudioRolloffMode.Linear;

            source.Play();

            // 3. Register for age-limiting logic
            RegisterPlayedAudio(clip);

            // 4. Start the cleanup routine
            Instance.StartCoroutine(Instance.ReturnToPool(source, clip.length));
        }

        private IEnumerator ReturnToPool(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);

            source.Stop();
            source.gameObject.SetActive(false);
            audioPool.Push(source);
        }

        public static bool CheckPlayedRecently(AudioClip clip)
        {
            foreach (var sound in Instance.sounds_played_recently)
            {
                if (sound.clip == clip && sound.age < Instance.soundThreshold)
                {
                    return true;
                }
            }
            return false;
        }

        public static void RegisterPlayedAudio(AudioClip clip)
        {
            SoundPlayedRecently newSound = new SoundPlayedRecently
            {
                clip = clip,
                age = 0f
            };
            Instance.sounds_played_recently.Add(newSound);
        }
    }
}