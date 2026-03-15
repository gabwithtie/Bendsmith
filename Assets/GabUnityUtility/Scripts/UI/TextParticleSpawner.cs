using UnityEngine;

namespace GabUnity
{

    public class TextParticleSpawner : MonoBehaviour
    {

        [SerializeField] private string default_text;
        [SerializeField] private float fontSize;
        [SerializeField] private float lifetime;
        [SerializeField] private Color color;

        public void SpawnText(int number)
        {
            TextParticle.SpawnText(number.ToString(), transform.position, 1.0f, 1.0f, color);
        }

        public void SpawnText(float number)
        {
            TextParticle.SpawnText(number.ToString(), transform.position, 1.0f, 1.0f, color);
        }

        public void SpawnText(string text)
        {
            TextParticle.SpawnText(text, transform.position, 1.0f, 1.0f, color);
        }

        public void SpawnAt(Vector3 worldpos)
        {
            TextParticle.SpawnText(default_text, worldpos, 1.0f, 1.0f, color);
        }
    }
}
