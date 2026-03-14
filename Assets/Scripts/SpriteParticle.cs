using UnityEngine;

namespace GabUnity
{
    public class SpriteParticle : MonoSingleton<SpriteParticle>
    {
        private struct ActiveSprite
        {
            public SpriteRenderer SR;
            public Vector3 StartPosition;
            public Color StartColor;
            public float ElapsedTime;
            public float MaxLifetime;
            public bool IsActive;
        }

        [Header("Pool Settings")]
        [SerializeField] public int poolSize = 20;
        [SerializeField] public GameObject spritePrefab; // Prefab must have a SpriteRenderer on root

        [Header("Animation Settings")]
        [SerializeField] public float hoverSpeed = 1f;

        private ActiveSprite[] _particles;
        private int _nextIndex = 0;

        private void Start()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            _particles = new ActiveSprite[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(spritePrefab, transform);
                obj.SetActive(false);

                var sr = obj.GetComponent<SpriteRenderer>();
                if (sr == null)
                {
                    Debug.LogError("SpriteParticle: spritePrefab must have a SpriteRenderer component on the root.");
                    sr = obj.AddComponent<SpriteRenderer>();
                }

                _particles[i] = new ActiveSprite
                {
                    SR = sr,
                    IsActive = false
                };
            }
        }

        public static void SpawnSprite(Sprite sprite, Vector3 position, float scale, float lifetime, Color color)
        {
            ref ActiveSprite p = ref Instance._particles[Instance._nextIndex];

            p.SR.gameObject.SetActive(true);
            p.SR.sprite = sprite;
            p.SR.color = color;
            p.SR.transform.position = position;
            p.SR.transform.localScale = Vector3.one * scale;

            p.StartPosition = position;
            p.StartColor = color;
            p.ElapsedTime = 0f;
            p.MaxLifetime = lifetime;
            p.IsActive = true;

            Instance._nextIndex = (Instance._nextIndex + 1) % Instance.poolSize;
        }

        private void Update()
        {
            if (MainCamera.Instance == null) return;

            for (int i = 0; i < _particles.Length; i++)
            {
                if (!_particles[i].IsActive) continue;

                _particles[i].ElapsedTime += Time.deltaTime;

                if (_particles[i].ElapsedTime >= _particles[i].MaxLifetime)
                {
                    _particles[i].IsActive = false;
                    _particles[i].SR.gameObject.SetActive(false);
                    continue;
                }

                float t = _particles[i].ElapsedTime / _particles[i].MaxLifetime;

                // Hover up
                _particles[i].SR.transform.position = _particles[i].StartPosition + Vector3.up * (hoverSpeed * _particles[i].ElapsedTime);

                // Fade out
                Color c = _particles[i].StartColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                _particles[i].SR.color = c;

                // Optional: face camera (billboard)
                var cam = MainCamera.Instance.transform;
                _particles[i].SR.transform.forward = cam.forward;
            }
        }
    }
}