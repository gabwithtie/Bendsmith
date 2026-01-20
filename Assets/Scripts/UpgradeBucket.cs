using UnityEngine;
using UnityEngine.Events;

public class UpgradeBucket : MonoBehaviour
{
    [SerializeField] private ParticleSystem own_particles;
    [SerializeField] private ParticleSystem hammer_particles;

    [SerializeField] private UnityEvent<bool> OnSetAmbient;
    [SerializeField] private UnityEvent OnAmbientEnable;
    [SerializeField] private UnityEvent OnAmbientDisable;

    public void SetAmbient(bool _value)
    {
        OnSetAmbient.Invoke(_value);

        if (_value)
            OnAmbientEnable.Invoke();
        else
            OnAmbientDisable.Invoke();
    }

    [ContextMenu("Trigger Upgrade Effects")]
    public void TriggerUpgradeEffects()
    {
        own_particles.Play();
        hammer_particles.Play();
        OnSetAmbient.Invoke(false);
    }
}
