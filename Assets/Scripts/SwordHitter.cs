using GabUnity;
using UnityEngine;

public class SwordHitter : MonoBehaviour
{
    [SerializeField]private Sword sword;
    [SerializeField]private float max_radius;
    [SerializeField]private float max_holdtime;
    [SerializeField]private float force;
    [SerializeField]private RadiusVisualizer radiusVisualizer;

    private bool _holding;
    private float _holdtime;
    private Vector3 _holdpos;

    public void StartHold(Vector3 pos)
    {
        _holdpos = pos;
        _holding = true;

        radiusVisualizer.SetPosition(pos);
        radiusVisualizer.SetMult(max_radius);
        _holdtime = 0;
    }

    public void StopHold()
    {
        if (!_holding)
            return;

        _holding = false;
        radiusVisualizer.SetT(0);
    }

    public void Hit()
    {
        var t = _holdtime / max_holdtime;

        sword.Hit(_holdpos, force, max_radius * t);
    }

    private void Update()
    {
        if (_holding)
        {
            _holdtime += Time.deltaTime;
            _holdtime = Mathf.Clamp(_holdtime, 0, max_holdtime);

            radiusVisualizer.SetT(_holdtime / max_holdtime);
        }
    }
}
