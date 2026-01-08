using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CurveDisplayer : MonoBehaviour
{
    LineRenderer line_renderer;

    private void Awake()
    {
        line_renderer = GetComponent<LineRenderer>();
    }

    public void Display(List<Vector3> curve)
    {
        line_renderer.positionCount = curve.Count;
        line_renderer.SetPositions(curve.ToArray());
    }
}
