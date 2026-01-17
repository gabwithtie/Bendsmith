using GabUnity;
using UnityEngine;
using UnityEngine.Events;

public class SwordHitter : MonoBehaviour
{
    [SerializeField]private Sword sword;
    [SerializeField]private RadiusVisualizer radiusVisualizer;

    [SerializeField] private UnityEvent<float> onSetQuantizedDuration;
    [SerializeField] private UnityEvent<Vector3> onCommitHit;

    private bool _holding;
    private Vector3 _holdpos;
    private float _dragdist;

    public void StartHold(Vector3 pos)
    {
        _dragdist = 0;
        _holdpos = pos;
        _holding = true;

        radiusVisualizer.SetPosition(pos);
    }

    public void OnDrag(Vector3 pos)
    {
        _dragdist = (_holdpos - pos).magnitude;
    }

    public void StopHold()
    {
        if (!_holding)
            return;

        _holding = false;
        radiusVisualizer.SetT(0);

        float delay = QuantizedEventInvoker.GetNextInvokationFromNow();
        onSetQuantizedDuration.Invoke(delay);

        onCommitHit.Invoke(_holdpos);
    }

    public void Hit()
    {
        _dragdist = Mathf.Clamp(_dragdist, 0, HammerStats.HammingMaxRadius);
        sword.Hit(_holdpos, HammerStats.HammerForce, _dragdist);
    }

    private void Update()
    {
        if (_holding)
        {
            _dragdist = Mathf.Clamp(_dragdist, 0, HammerStats.HammingMaxRadius);
            radiusVisualizer.SetT(_dragdist);
        }
    }
}
