using GabUnity;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ComboCounter))]
public class SwordHitter : MonoBehaviour
{
    [SerializeField] private Sword sword;
    [SerializeField] private RadiusVisualizer radiusVisualizer;

    [SerializeField] private UnityEvent<float> onSetQuantizedDuration;
    [SerializeField] private UnityEvent<Vector3> onCommitHit;
    [SerializeField] private UnityEvent onActualHit;

    // --- new sprite fields ---
    [Header("Hit Feedback Sprites")]
    [SerializeField] private Sprite hitSpriteGood;
    [SerializeField] private Sprite hitSpriteEarly;
    [SerializeField] private Sprite hitSpriteLate;
    [SerializeField] private float hitSpriteScale = 1f;
    [SerializeField] private float hitSpriteLifetime = 0.6f;

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

        // choose sprite based on result (no tinting)
        Sprite chosen = null;
        if (goodrelease)
        {
            chosen = hitSpriteGood;
            _comboCounter.RegisterCombo();
        }
        else
        {
            if (rhythmresult < 0)
                chosen = hitSpriteEarly;
            else if (rhythmresult > 0)
                chosen = hitSpriteLate;

            _comboCounter.ResetCombo();
        }

        if (chosen != null)
        {
            // Pass Color.white to avoid tinting the sprite
            SpriteParticle.SpawnSprite(chosen, _holdpos, hitSpriteScale, hitSpriteLifetime, Color.white);
        }
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
