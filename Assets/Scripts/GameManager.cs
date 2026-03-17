using GabUnity;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoSingleton<GameManager>
{
    private Sword sword;
    private CurveGoalGenerator curve_goal_generator;

    enum GameMode
    {
        Main,
        Upgrade,
        Over
    }

    [Header("Runtime")]
    [SerializeField] private GameMode cur_mode = GameMode.Main;
    [SerializeField] private int _cur_durability = 1;
    [SerializeField] private float time_left = 0;
    [SerializeField] private int default_timelimit = 30;
    [SerializeField] private float cur_score = 0;

    private int cur_durability
    {
        get => _cur_durability;
        set {
            _cur_durability = value;
            onChangeDurability.Invoke(_cur_durability);
        }
    }

    [Header("Level")]
    [SerializeField] private int current_set_index = 0;
    [SerializeField] private int current_level_index = 0;
    [SerializeField] private int min_max_level = 2;
    [SerializeField] private int max_max_level = 10;
    [SerializeField] private int min_bend = 2;
    [SerializeField] private int max_bend = 5;
    [SerializeField] private int peak_set = 20;
    [SerializeField] private int bad_sword_penalty = 2;
    [Header("Currency")]
    /// <summary>
    /// Defines how much currency is awarded per score point (0-100)
    /// </summary>
    [SerializeField] private float score_to_currency_mult = 0.5f;
    [SerializeField] private CurrencyInfo currencyInfo;

    [Header("Mode Switching")]
    /// <summary>
    /// Event invoked when the player submits their work. Returns the currency achieved.
    /// </summary>
    [SerializeField] private UnityEvent<int> onSubmit;
    [SerializeField] private ActionRequest set_upgrade_mode_request;
    [SerializeField] private ActionRequest set_main_mode_request;
    [SerializeField] private ActionRequest game_over_request;
    [Header("Events")]
    [SerializeField] private UnityEvent<float> onChangeTimeRatio;
    [SerializeField] private UnityEvent<int> onChangeDurability;
    [SerializeField] private UnityEvent<float> OnChangeScore;
    [SerializeField] private UnityEvent OnGoodHit;
    [SerializeField] private UnityEvent OnLateHit;
    [SerializeField] private UnityEvent OnEarlyHit;

    [Header("States")]
    [SerializeField] private bool submittable;

    public bool Submittable => submittable;
    public void SetSubmittable(bool _value) => submittable = _value;
    
    private ComboCounter _comboCounter;
    private int current_max_level => (int)Mathf.Lerp(min_max_level, max_max_level, (float)current_set_index / peak_set);

    protected override void Awake()
    {
        base.Awake();

        time_left = default_timelimit;

        sword = FindAnyObjectByType<Sword>();
        _comboCounter = FindAnyObjectByType<ComboCounter>();
        curve_goal_generator = FindAnyObjectByType<CurveGoalGenerator>();
    }

    private void Start()
    {
        SetMainMode();
    }

    private void Update()
    {
        if (cur_mode == GameMode.Main)
        {
            time_left -= Time.deltaTime;
            
            if (submittable)
            {
                onChangeTimeRatio.Invoke(time_left / default_timelimit);
            }

            if (time_left < 0 && submittable)
                SubmitWork();

            if (cur_durability <= 0)
            {
                SetGameOver();
            }
        }
    }

    public void AddHeat(float secs)
    {
        time_left += secs;
    }

    [ContextMenu("Submit Current Work")]
    public void SubmitWork()
    {
        if (submittable == false) return;

        //SCORING
        float score = curve_goal_generator.Compare(sword.GetCurrentCurve()) * 100;

        if (score < 0)
            cur_durability -= bad_sword_penalty;

        int currency = Mathf.RoundToInt(score * score_to_currency_mult);
        CurrencyManager.Add(currencyInfo, currency);
        onSubmit.Invoke(currency);

        //RENEWAL
        time_left = default_timelimit;
        var newbendcount = (int)Mathf.Lerp(min_bend, max_bend, (float)current_set_index / peak_set);
        curve_goal_generator.BendCount = newbendcount;
        curve_goal_generator.GenerateCurveGoal();
        sword.ResetSword();
        SetSubmittable(false);

        //LEVELING
        current_level_index++;

        if (current_level_index >= current_max_level)
        {
            SetUpgradeMode();
            current_level_index = 0;
        }
    }

    public void CommitHit(Vector3 pos)
    {
        bool goodrelease = RhythmManager.IsGood(out int rhythmresult);
        Color textcolor = goodrelease ? Color.green : Color.red;
        var hittext = "";
        if (goodrelease)
        {
            hittext = "Good!";
            hittext += Environment.NewLine + "x" + _comboCounter.Combo;

            _comboCounter.RegisterCombo();

            OnGoodHit.Invoke();
        }
        else
        {
            if (rhythmresult < 0)
            {
                OnEarlyHit.Invoke();
                hittext = "Early";
            }
            else if (rhythmresult > 0)
            {
                OnLateHit.Invoke();
                hittext = "Late";
            }

            _comboCounter.ResetCombo();

            cur_durability--;
        }
    }

    public void OnActualHit()
    {
        cur_score = curve_goal_generator.Compare(sword.GetCurrentCurve());
        OnChangeScore.Invoke(cur_score);
    }

    [ContextMenu("Set Upgrade Mode")]
    public void SetUpgradeMode()
    {
        ActionRequestManager.Request(set_upgrade_mode_request);
    }

    [ContextMenu("Set Main Mode")]
    public void SetMainMode()
    {
        ActionRequestManager.Request(set_main_mode_request);
    }

    public void OnUpgradeMode()
    {
        cur_mode = GameMode.Upgrade;
    }

    public void OnMainMode()
    {
        cur_mode = GameMode.Main;

        cur_durability = HammerStats.MaxHammerDurability;
        onChangeDurability.Invoke(cur_durability);
    }

    public void SetGameOver()
    {
        cur_mode = GameMode.Over;

        ActionRequestManager.Request(game_over_request);
    }
}
