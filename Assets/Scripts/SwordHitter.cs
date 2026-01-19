using GabUnity;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ComboCounter))]
public class SwordHitter : MonoBehaviour
{
    [SerializeField]private Sword sword;
    [SerializeField]private RadiusVisualizer radiusVisualizer;

    [SerializeField] private UnityEvent<float> onSetQuantizedDuration;
    [SerializeField] private UnityEvent<Vector3> onCommitHit;
    [SerializeField] private UnityEvent onActualHit;

    private ComboCounter _comboCounter;

    private bool _holding;
    private bool _waitingforhit;
    private Vector3 _holdpos;
    private float _dragdist;

    private void Awake()
    {
        _comboCounter = GetComponent<ComboCounter>();
    }

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

        if (_waitingforhit)
            return;
        
        float delay = QuantizedEventInvoker.InvokeOnNext(Hit);
        onSetQuantizedDuration.Invoke(delay);
        onCommitHit.Invoke(_holdpos);

        _waitingforhit = true;

        bool goodrelease = RhythmManager.IsGood(out int rhythmresult);
        Color textcolor = goodrelease ? Color.green : Color.red;
        var hittext = "";
        if(goodrelease)
        {
            hittext = "Good!";
            hittext += Environment.NewLine + "x" + _comboCounter.Combo;

            _comboCounter.RegisterCombo();
        }
        else
        {
            if (rhythmresult < 0)
                hittext = "Early";
            else if (rhythmresult > 0)
                hittext = "Late";

            _comboCounter.ResetCombo();
        }

        TextParticle.SpawnText(hittext, _holdpos, 1, 0.3f, textcolor);
    }

    public void Hit()
    {
        _dragdist = Mathf.Clamp(_dragdist, 0, HammerStats.HammingMaxRadius);
        sword.Hit(_holdpos, HammerStats.HammerForce, _dragdist);

        _waitingforhit = false;
        onActualHit.Invoke();
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
