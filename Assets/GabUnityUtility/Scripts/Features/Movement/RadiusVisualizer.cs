using UnityEngine;

namespace GabUnity
{
    public class RadiusVisualizer : MonoBehaviour
    {
        [SerializeField] private float mult;
        [SerializeField] private GameObject visualizer_object;

        public void SetPosition(Vector3 pos)
        {
            this.transform.position = pos;
        }

        public void SetT(float t)
        {
            visualizer_object.transform.localScale = new Vector3(t, t, t) * mult;
            visualizer_object.SetActive(t > 0.001f);
        }

        public void SetMult(float _mult)
        {
            this.mult = _mult;
        }
    }
}
