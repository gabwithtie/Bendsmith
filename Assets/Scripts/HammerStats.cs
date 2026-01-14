using GabUnity;
using UnityEngine;

public class HammerStats : MonoSingleton<HammerStats>
{
    [SerializeField] private int hammerDurability = 50;
    [SerializeField] private float hammerForce = 0.6f;
    [SerializeField] private float hammerMaxRadius = 2.0f;
}
