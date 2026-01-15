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

    public List<Vector3> GetCurrentCurve() => current_curve;

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
            float zPos = t; // Assuming a 2-unit long sword

            // Create a compound wave based on bend_count
            // Using Sin(t * pi * bends) ensures the hilt (t=0) starts at 0
            float xPos = Mathf.Sin(t * Mathf.PI * bend_count * randomFreqOffset + randomPhase)
                         * t * amplitude_mult;

            current_curve.Add(new Vector3(xPos, 0, zPos));
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
            return 0;

        return GetNormalizedPositionScore(other, current_curve, amplitude_mult * 0.5f);
    }

    /// <summary>
    /// Compares two curves by normalizing their Z-range to [0, 1] and 
    /// grading the difference in X-displacement.
    /// Returns 1 for a perfect match, 0 for maximum deviation.
    /// </summary>
    public float GetNormalizedPositionScore(List<Vector3> playerSword, List<Vector3> goalCurve, float maxXError = 0.5f)
    {
        if (playerSword == null || goalCurve == null || playerSword.Count < 2 || goalCurve.Count < 2)
            return 0f;

        // 1. Normalize both curves to a local [0,1] Z-space
        List<Vector3> normPlayer = NormalizeCurve(playerSword);
        List<Vector3> normGoal = NormalizeCurve(goalCurve);

        float totalXError = 0f;
        int sampleSteps = 25;

        // 2. Sample and compare X values at standardized Z-intervals
        for (int i = 0; i < sampleSteps; i++)
        {
            float t = i / (float)(sampleSteps - 1);

            // Find the X value at the specific normalized Z progress
            float xA = GetXAtNormalizedZ(normPlayer, t);
            float xB = GetXAtNormalizedZ(normGoal, t);

            totalXError += Mathf.Abs(xA - xB);
        }

        float averageXError = totalXError / sampleSteps;

        // 3. Map error to 0-1 score based on allowed X tolerance
        return Mathf.Clamp01(1f - (averageXError / maxXError));
    }

    private List<Vector3> NormalizeCurve(List<Vector3> rawPoints)
    {
        List<Vector3> normalized = new List<Vector3>();

        // Find bounds for normalization
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        Vector3 startPos = rawPoints[0];

        foreach (var p in rawPoints)
        {
            if (p.z < minZ) minZ = p.z;
            if (p.z > maxZ) maxZ = p.z;
        }

        float zRange = maxZ - minZ;
        if (zRange < 0.0001f) zRange = 1f;

        foreach (var p in rawPoints)
        {
            // Shift so first X is 0, and Z is scaled between 0 and 1
            Vector3 normpos = p - startPos;
            normpos /= zRange;

            normalized.Add(normpos);
        }

        return normalized;
    }

    private float GetXAtNormalizedZ(List<Vector3> normPath, float t)
    {
        // Find the segment where the normalized Z matches t
        for (int i = 0; i < normPath.Count - 1; i++)
        {
            if (t >= normPath[i].z && t <= normPath[i + 1].z)
            {
                float segmentT = (t - normPath[i].z) / (normPath[i + 1].z - normPath[i].z);
                return Mathf.Lerp(normPath[i].x, normPath[i + 1].x, segmentT);
            }
        }
        return normPath[normPath.Count - 1].x;
    }
}