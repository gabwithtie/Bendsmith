using GabUnity;
using System;
using UnityEngine;
using UnityEngine.Events;

public class HammerController : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField]private RadiusVisualizer radiusVisualizer;

    [Header("Events")]
    [SerializeField] private UnityEvent<float> onSetQuantizedDuration;
    [SerializeField] private UnityEvent<Vector3> onCommitHit;
    [SerializeField] private UnityEvent<Vector3, float> onActualHit;

    private Sword _sword;

    private bool _holding;
    private bool _waitingforhit;
    private Vector3 _holdpos;
    private Vector3 _committedpos;
    private float _dragdist;
    private float _committedDragdist;

    private void Awake()
    {
        _sword = FindAnyObjectByType<Sword>();
    }

    public void StartHold(Vector3 pos)
    {
        if (!GameManager.Instance.Submittable)
            return;

        _dragdist = 0;
        _holdpos = pos;
        _holding = true;

        radiusVisualizer.SetPosition(pos);
    }

    public void StopHold()
    {
        if (!_holding)
            return;

        _holding = false;
        radiusVisualizer.SetT(0);

        if (_waitingforhit)
            return;
        
        _waitingforhit = true;
        _committedpos = _holdpos;
        _committedDragdist = _dragdist;

        float delay = QuantizedEventInvoker.GetNextInvokationFromNow();
        Invoke(nameof(Hit), delay);
        onSetQuantizedDuration.Invoke(delay);
        onCommitHit.Invoke(_committedpos);
    }

    public void Hit()
    {
        _committedDragdist = Mathf.Clamp(_committedDragdist, 0, HammerStats.HammingMaxRadius);
        onActualHit.Invoke(_committedpos, _committedDragdist);
        _sword.Hit(_committedpos, HammerStats.HammerForce, _committedDragdist);

        _waitingforhit = false;
    }

    private void Update()
    {
        if (_holding)
        {
            _dragdist += (Time.deltaTime / RhythmManager.SecondsPerBeat) * HammerStats.HammingMaxRadius;
            _dragdist = Mathf.Clamp(_dragdist, 0, HammerStats.HammingMaxRadius);
            radiusVisualizer.SetT(_dragdist);
        }
    }
}
