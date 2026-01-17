using UnityEngine;

namespace GabUnity
{
    public class RhythmManager : MonoSingleton<RhythmManager>
    {
        [SerializeField] private float bpm = 120;
        [SerializeField] private int beatsperbar = 4;
        public static float Bpm => Instance.bpm;
        public static int BeatsPerBar => Instance.beatsperbar;
        [SerializeField] private AudioSource basis;

        public static float Time => Instance.basis.time;

        public void SetBpm(float _bpm)
        {
            this.bpm = _bpm;
        }
    }
}
