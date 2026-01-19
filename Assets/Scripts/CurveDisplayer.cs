using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class CurveDisplayer : MonoBehaviour
{
    LineRenderer line_renderer;

    private List<Vector3> _curve_cache;

    private void Awake()
    {
        line_renderer = GetComponent<LineRenderer>();
    }

    public void SetCurve(List<Vector3> _curve)
    {
        _curve_cache = _curve;
    }

    public void OnSafeToChange()
    {
        line_renderer.positionCount = _curve_cache.Count;
        line_renderer.SetPositions(_curve_cache.ToArray());
    }
}
