using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static HammerStats;

public class UpgradeBucket : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private UpgradeType upgradeType;

    [SerializeField] private ParticleSystem own_particles;
    [SerializeField] private ParticleSystem hammer_particles;

    [SerializeField] private UnityEvent OnClick;
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

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        HammerStats.Instance.Upgrade(upgradeType);
        OnClick.Invoke();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        this.SetAmbient(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        this.SetAmbient(false);
    }
}
