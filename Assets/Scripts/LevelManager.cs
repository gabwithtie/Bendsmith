using UnityEngine;
using GabUnity;


public class LevelManager : MonoSingleton<LevelManager>
{
    private Sword sword;
    private CurveDisplayer curve_displayer;
    private CurveGoalGenerator curve_goal_generator;

    protected override void Awake()
    {
        base.Awake();

        sword = FindAnyObjectByType<Sword>();
        curve_displayer = FindAnyObjectByType<CurveDisplayer>();
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
}
