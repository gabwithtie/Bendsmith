using UnityEngine;

namespace GabUnity
{
    public class ScaleLerper : MonoBehaviour
    {
        [SerializeField] private Vector3 scale_a;
        [SerializeField] private Vector3 scale_b;

        [SerializeField] private float lerp_speed = 10;

        [SerializeField] private float target_t = 0;

        private float current_t = 0;

        public void SetTargetT(float t)
        {
            target_t = t;
        }

        private void Update()
        {
            var new_t = Mathf.MoveTowards(current_t, target_t, lerp_speed * Time.deltaTime);

            if (Mathf.Abs(new_t - current_t) > 0.001f)
            {
                current_t = new_t;
                transform.localScale = Vector3.Lerp(scale_a, scale_b, current_t);
            }
        }
    }
}
