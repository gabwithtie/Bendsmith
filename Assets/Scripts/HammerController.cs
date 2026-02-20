using GabUnity;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ComboCounter))]
public class HammerController : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private int cur_durability = 1;

    [Header("Visuals")]
    [SerializeField]private RadiusVisualizer radiusVisualizer;

    [Header("Events")]
    [SerializeField] private UnityEvent<float> onSetQuantizedDuration;
    [SerializeField] private UnityEvent<int> onChangeDurability;
    [SerializeField] private UnityEvent<Vector3> onCommitHit;
    [SerializeField] private UnityEvent onActualHit;
    [SerializeField] private ActionRequest hammerbreak_request;

    private Sword _sword;
    private ComboCounter _comboCounter;

    private bool _holding;
    private bool _waitingforhit;
    private Vector3 _holdpos;
    private Vector3 _committedpos;
    private float _dragdist;

    private void Awake()
    {
        _sword = FindAnyObjectByType<Sword>();
        _comboCounter = GetComponent<ComboCounter>();
    }

    public void NewStage()
    {
        cur_durability = HammerStats.MaxHammerDurability;
        onChangeDurability.Invoke(cur_durability);
    }

    public void StartHold(Vector3 pos)
    {
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

        float delay = QuantizedEventInvoker.GetNextInvokationFromNow();
        Invoke(nameof(Hit), delay);
        onSetQuantizedDuration.Invoke(delay);
        onCommitHit.Invoke(_committedpos);

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

            cur_durability--;
            onChangeDurability.Invoke(cur_durability);

            if (cur_durability <= 0)
            {
                //End Game Here
                ActionRequestManager.Request(hammerbreak_request);
            }
        }

        TextParticle.SpawnText(hittext, _committedpos, 1, 0.3f, textcolor);
    }

    public void Hit()
    {
        _dragdist = Mathf.Clamp(_dragdist, 0, HammerStats.HammingMaxRadius);
        _sword.Hit(_committedpos, HammerStats.HammerForce, _dragdist);

        _waitingforhit = false;
        onActualHit.Invoke();
    }

    private void Update()
    {
        if (_holding)
        {
            _dragdist += Mathf.LerpUnclamped(0, HammerStats.HammingMaxRadius, Time.deltaTime / RhythmManager.SecondsPerBeat);
            _dragdist = Mathf.Clamp(_dragdist, 0, HammerStats.HammingMaxRadius);
            radiusVisualizer.SetT(_dragdist);
        }
    }
}
