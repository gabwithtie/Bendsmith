using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Sword : MonoBehaviour
{
    // Vertexes of the sword skinned mesh (assumed to be in order from hilt to tip)
    [SerializeField] private List<Transform> vertexes;
    [SerializeField] private List<Transform> reflect_list;

    [Header("Deformation Settings")]
    [SerializeField] private float length = 5.0f;
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
            vertexes[i].localPosition = new Vector3(0, 0, t * length);
        }

        ApplyToReflect();
    }

    public void ApplyToReflect()
    {
        for(int i = 0; i < reflect_list.Count; i++)
        {
            reflect_list[i].position = vertexes[i].position;
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
            pos.y = vertex.position.y;

            float sqrdistance = Vector3.SqrMagnitude(pos - vertex.position);

            if (sqrdistance < radius * radius)
            {
                hitcount++;
                float dist = Vector3.Distance(pos, vertex.position);

                float falloff = 1f - (dist / radius);
                Vector3 dir = (vertex.position - pos).normalized;

                if (dir == Vector3.zero) dir = -transform.up;

                Vector3 displacement = dir * force * falloff;
                displacement.y = 0;

                vertex.position += displacement;
            }
        }

        // Equalize spacing along the new curve
        if(hitcount > 0)
            RedistributeVertexes();

        ApplyToReflect();
    }

    public void RedistributeVertexes()
    {
        if (vertexes == null || vertexes.Count < 2) return;

        // 1. Calculate the current actual length of the curve
        float actualLength = 0;
        for (int i = 0; i < vertexes.Count - 1; i++)
        {
            actualLength += Vector3.Distance(vertexes[i].position, vertexes[i + 1].position);
        }

        // 2. Determine the redistribution length (cannot exceed maxLength)
        float clampedLength = Mathf.Min(actualLength, length);

        // Store positions before moving anything for sampling
        Vector3[] sampledPositions = vertexes.Select(v => v.position).ToArray();

        // 3. Calculate intervals based on the clamped length
        float targetInterval = clampedLength / (vertexes.Count - 1);

        // 4. Update the tip position if the sword was too long
        // This pulls the tip back toward the hilt along the existing path
        if (actualLength > length)
        {
            vertexes[vertexes.Count - 1].position = GetPointOnPath(sampledPositions, length);
        }

        // 5. Redistribute intermediate vertices
        for (int i = 1; i < vertexes.Count - 1; i++)
        {
            float targetDist = i * targetInterval;
            vertexes[i].position = GetPointOnPath(sampledPositions, targetDist);
        }

        ApplyToReflect();
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