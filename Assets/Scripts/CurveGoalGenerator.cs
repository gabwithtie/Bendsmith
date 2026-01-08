using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CurveGoalGenerator : MonoBehaviour
{
    [SerializeField] private int vertex_count = 16;
    [SerializeField] private int bend_count = 3;
    [SerializeField] private float amplitude_mult = 1.0f;
    [SerializeField] private UnityEvent<List<Vector3>> OnGenerate;

    private List<Vector3> current_curve;

    [ContextMenu("Generate New Curve")]
    public void GenerateCurveGoal()
    {
        current_curve = new List<Vector3>();

        // Randomize the "flavor" of the bends
        float randomPhase = Random.Range(0f, 100f);
        float randomFreqOffset = Random.Range(0.8f, 1.2f);

        for (int i = 0; i < vertex_count; i++)
        {
            float t = i / (float)(vertex_count - 1);

            // Standard sword axis (Z is length, X is bend)
            float zPos = t * 2.0f; // Assuming a 2-unit long sword

            // Create a compound wave based on bend_count
            // Using Sin(t * pi * bends) ensures the hilt (t=0) starts at 0
            float xPos = Mathf.Sin(t * Mathf.PI * bend_count * randomFreqOffset + randomPhase)
                         * t * amplitude_mult;

            current_curve.Add(transform.TransformPoint(new Vector3(xPos, 0, zPos)));
        }

        OnGenerate.Invoke(current_curve);
    }

    /// <summary>
    /// Compares the provided curve to the goal curve.
    /// Returns a float where 0 is perfect and higher is worse.
    /// </summary>
    public float Compare(List<Vector3> other)
    {
        if (current_curve == null || current_curve.Count < 2 || other == null || other.Count < 2)
            return float.MaxValue;

        float totalError = 0f;
        int sampleSteps = 20; // Fixed resolution makes it independent of vertex_count

        for (int i = 0; i < sampleSteps; i++)
        {
            float t = i / (float)(sampleSteps - 1);

            Vector3 goalPoint = GetPointOnPath(current_curve, t);
            Vector3 playerPoint = GetPointOnPath(other, t);

            totalError += Vector3.Distance(goalPoint, playerPoint);
        }

        return totalError / sampleSteps;
    }

    // Helper to sample any point along a list of positions using normalized time (0-1)
    private Vector3 GetPointOnPath(List<Vector3> path, float t)
    {
        t = Mathf.Clamp01(t);
        float segmentTarget = t * (path.Count - 1);
        int startIndex = Mathf.FloorToInt(segmentTarget);
        int endIndex = Mathf.CeilToInt(segmentTarget);

        if (startIndex == endIndex) return path[startIndex];

        float lerpT = segmentTarget - startIndex;
        return Vector3.Lerp(path[startIndex], path[endIndex], lerpT);
    }
}