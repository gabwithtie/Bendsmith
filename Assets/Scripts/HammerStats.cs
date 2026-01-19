using GabUnity;
using UnityEngine;

public class HammerStats : MonoSingleton<HammerStats>
{
    [SerializeField] private int maxHammerDurability = 50;
    [SerializeField] private CurrencyChangeInfo cost_durability;
    public static int MaxHammerDurability => Instance.maxHammerDurability;

    [SerializeField] private float hammerForce = 0.6f;
    [SerializeField] private CurrencyChangeInfo cost_force;
    public static float HammerForce => Instance.hammerForce;

    [SerializeField] private float hammerMaxRadius = 2.0f;
    [SerializeField] private CurrencyChangeInfo cost_radius;
    public static float HammingMaxRadius => Instance.hammerMaxRadius;

    public void UpgradeDurability()
    {
        if (CurrencyManager.Spend(cost_durability))
        {
            maxHammerDurability += 2;
        }
    }

    public void UpgradeForce()
    {
        if (CurrencyManager.Spend(cost_force))
        {
            hammerForce += 0.1f;
        }
    }

    public void UpgradeRadius()
    {
        if (CurrencyManager.Spend(cost_radius))
        {
            hammerMaxRadius += 0.1f;
        }
    }
}
