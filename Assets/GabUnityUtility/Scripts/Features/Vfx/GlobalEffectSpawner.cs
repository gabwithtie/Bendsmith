using UnityEngine;

namespace GabUnity
{
    public class GlobalEffectSpawner : MonoBehaviour
    {
        [SerializeField] private string effect_id;
        [SerializeField] private Vector3 default_direction;
        [SerializeField] private Vector3 default_pos;

        public string Effect_id { get => effect_id; set => effect_id = value; }

        public void SetPos(Vector3 pos)
        {
            default_pos = pos;
        }

        public void Spawn()
        {
            GlobalEffectManager.Spawn(effect_id, default_pos, default_direction);
        }

        public void SpawnPos(Vector3 pos)
        {
            GlobalEffectManager.Spawn(effect_id, pos, default_direction);
        }

        public void SpawnDir(Vector3 pos, Vector3 dir)
        {
            GlobalEffectManager.Spawn(effect_id, pos, dir);
        }
    }
}
