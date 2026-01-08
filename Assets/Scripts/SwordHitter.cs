using UnityEngine;

public class SwordHitter : MonoBehaviour
{
    [SerializeField]private Sword sword;
    [SerializeField]private float radius;
    [SerializeField]private float force;

    public void Hit(Vector3 pos)
    {
        sword.Hit(pos, force, radius);
    }
}
