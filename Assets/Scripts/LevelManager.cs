using UnityEngine;
using GabUnity;


public class LevelManager : MonoSingleton<LevelManager>
{
    private Sword sword;
    private CurveGoalGenerator curve_goal_generator;

    [SerializeField] private ActionRequest set_upgrade_mode_request;
    [SerializeField] private ActionRequest set_main_mode_request;

    protected override void Awake()
    {
        base.Awake();

        sword = FindAnyObjectByType<Sword>();
        curve_goal_generator = FindAnyObjectByType<CurveGoalGenerator>();
    }

    private void Start()
    {
        curve_goal_generator.GenerateCurveGoal();
    }

    public void SubmitWork()
    {
        float score = curve_goal_generator.Compare(sword.GetCurrentCurve()) * 100;

        TextParticle.SpawnText("Score: " + score.ToString(), sword.transform.position, 1, 1, Color.yellow);

        curve_goal_generator.GenerateCurveGoal();

        sword.ResetSword();
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
}
