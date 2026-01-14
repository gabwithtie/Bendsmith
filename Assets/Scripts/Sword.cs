using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Sword : MonoBehaviour
{
    // Vertexes of the sword skinned mesh (assumed to be in order from hilt to tip)
    [SerializeField] private List<Transform> vertexes;

    [Header("Deformation Settings")]
    [SerializeField] private float default_length = 5.0f;
    [Range(0f, 1f)]
    [SerializeField] private float yDisplacementWeight = 0.2f;
    [SerializeField] private bool lockFirstVertex = true; // Toggle to anchor the hilt

    [SerializeField] private UnityEvent onReset;

    private void Start()
    {
        ResetSword();
    }

    public List<Vector3> GetCurrentCurve()
    {
        return vertexes.Select(v => v.localPosition).ToList();
    }

    public void ResetSword()
    {
        onReset.Invoke();
    }

    public void ResetVertexList()
    {
        // Reset vertexes to their initial straight configuration
        for (int i = 0; i < vertexes.Count; i++)
        {
            float t = (float)i / (vertexes.Count - 1);
            vertexes[i].localPosition = new Vector3(0, 0, t * default_length);
        }
    }

    // Displace the vertexes according to a hit at position pos, with given force and radius
    public void Hit(Vector3 pos, float force, float radius)
    {
        // If we lock the first vertex, we start the loop from index 1
        int startIndex = lockFirstVertex ? 1 : 0;

        int hitcount = 0;

        for (int i = startIndex; i < vertexes.Count; i++)
        {
            Transform vertex = vertexes[i];
            float sqrdistance = Vector3.SqrMagnitude(pos - vertex.position);

            if (sqrdistance < radius * radius)
            {
                hitcount++;
                float dist = Vector3.Distance(pos, vertex.position);

                float falloff = 1f - (dist / radius);
                Vector3 dir = (vertex.position - pos).normalized;

                if (dir == Vector3.zero) dir = -transform.up;

                Vector3 displacement = dir * force * falloff;
                displacement.y *= yDisplacementWeight;

                vertex.position += displacement;
            }
        }

        // Equalize spacing along the new curve
        if(hitcount > 0)
            RedistributeVertexes();
    }

    public void RedistributeVertexes()
    {
        if (vertexes == null || vertexes.Count < 2) return;

        float totalLength = 0;
        List<float> segmentLengths = new List<float>();

        for (int i = 0; i < vertexes.Count - 1; i++)
        {
            float dist = Vector3.Distance(vertexes[i].position, vertexes[i + 1].position);
            segmentLengths.Add(dist);
            totalLength += dist;
        }

        Vector3[] sampledPositions = vertexes.Select(v => v.position).ToArray();
        float targetInterval = totalLength / (vertexes.Count - 1);

        // This loop already protects the first (0) and last (count-1) indices
        for (int i = 1; i < vertexes.Count - 1; i++)
        {
            float targetDist = i * targetInterval;
            vertexes[i].position = GetPointOnPath(sampledPositions, targetDist);
        }
    }

    private Vector3 GetPointOnPath(Vector3[] path, float distance)
    {
        float accumulated = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            float segLen = Vector3.Distance(path[i], path[i + 1]);
            if (accumulated + segLen >= distance)
            {
                float t = (distance - accumulated) / segLen;
                return Vector3.Lerp(path[i], path[i + 1], t);
            }
            accumulated += segLen;
        }
        return path[path.Length - 1];
    }
}