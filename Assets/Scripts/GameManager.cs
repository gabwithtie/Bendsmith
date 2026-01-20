using GabUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoSingleton<GameManager>
{
    private Sword sword;
    private CurveGoalGenerator curve_goal_generator;

    [Header("Level")]
    [SerializeField] private int current_level_index = 0;
    [SerializeField] private int current_max_level = 5;
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
    
    [Header("States")]
    [SerializeField] private bool submittable;

    public void SetSubmittable(bool _value) => submittable = _value;

    protected override void Awake()
    {
        base.Awake();

        sword = FindAnyObjectByType<Sword>();
        curve_goal_generator = FindAnyObjectByType<CurveGoalGenerator>();
    }

    private void Start()
    {
        SetMainMode();
    }

    [ContextMenu("Submit Current Work")]
    public void SubmitWork()
    {
        if (submittable == false) return;

        //SCORING
        float score = curve_goal_generator.Compare(sword.GetCurrentCurve()) * 100;
        int currency = Mathf.RoundToInt(score * score_to_currency_mult);
        CurrencyManager.Add(currencyInfo, currency);
        onSubmit.Invoke(currency);

        //RENEWAL
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

    public void SetGameOver()
    {

    }
}
