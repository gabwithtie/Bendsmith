using GabUnity;
using UnityEngine;

public class HammerStats : MonoSingleton<HammerStats>
{
    public enum UpgradeType
    {
        Durability,
        Force,
        Radius
    };

    [SerializeField] private int maxHammerDurability = 20;
    [SerializeField] private int interval_durability = 2;
    [SerializeField] private CurrencyChangeInfo cost_durability;
    [SerializeField] private UpgradeBucket bucket_durability;
    public static int MaxHammerDurability => Instance.maxHammerDurability;

    [SerializeField] private float hammerForce = 0.6f;
    [SerializeField] private float interval_force = 0.1f;
    [SerializeField] private CurrencyChangeInfo cost_force;
    [SerializeField] private UpgradeBucket bucket_force;
    public static float HammerForce => Instance.hammerForce;

    [SerializeField] private float hammerMaxRadius = 1.0f;
    [SerializeField] private float interval_radius = 0.3f;
    [SerializeField] private CurrencyChangeInfo cost_radius;
    [SerializeField] private UpgradeBucket bucket_radius;
    public static float HammingMaxRadius => Instance.hammerMaxRadius;

    public void Upgrade(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Durability:
                UpgradeDurability();
                break;
            case UpgradeType.Force:
                UpgradeForce();
                break;
            case UpgradeType.Radius:
                UpgradeRadius();
                break;
            default:
                break;
        }
    }

    private void OnBroke() => Popupper.Popup("Not enough currency.");

    public void UpgradeDurability()
    {
        if (CurrencyManager.Spend(cost_durability))
        {
            maxHammerDurability += interval_durability;
            bucket_durability.TriggerUpgradeEffects();
        }
        else
        {
            OnBroke();
        }
    }

    public void UpgradeForce()
    {
        if (CurrencyManager.Spend(cost_force))
        {
            hammerForce += interval_force;
            bucket_force.TriggerUpgradeEffects();
        }
        else
        {
            OnBroke();
        }
    }

    public void UpgradeRadius()
    {
        if (CurrencyManager.Spend(cost_radius))
        {
            hammerMaxRadius += interval_radius;
            bucket_radius.TriggerUpgradeEffects();
        }
        else
        {
            OnBroke();
        }
    }
}
