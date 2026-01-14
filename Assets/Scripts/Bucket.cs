using UnityEngine;

public class Bucket : MonoBehaviour
{
    [SerializeField] private ParticleSystem own_particles;
    [SerializeField] private ParticleSystem hammer_particles;

    [ContextMenu("Trigger Upgrade Effects")]
    public void TriggerUpgradeEffects()
    {
        own_particles.Play();
        hammer_particles.Play();
    }
}
