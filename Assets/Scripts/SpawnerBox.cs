using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerBox : MonoBehaviour
{
    [Header("Bounds Settings")]
    [SerializeField] private Vector3 boxSize = Vector3.one;
    [SerializeField] private Color boundsColor = Color.green;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private int max_objects = 10;
    [SerializeField] private float objectRadius = 0.5f;
    [SerializeField] private int maxAttempts = 10; // To prevent infinite loops

    [Header("Events")]
    [SerializeField] private UnityEvent<Vector3> onObjectDestroyed;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0;
            if (spawnedObjects.Count < max_objects)
            {
                SpawnRandom();
            }
        }
    }

    private void SpawnRandom()
    {
        Vector3 spawnWorldPos = Vector3.zero;
        bool validPositionFound = false;

        // Try several times to find a clear spot
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomLocalPos = new Vector3(
                Random.Range(-0.5f, 0.5f) * boxSize.x,
                Random.Range(-0.5f, 0.5f) * boxSize.y,
                Random.Range(-0.5f, 0.5f) * boxSize.z
            );

            spawnWorldPos = transform.TransformPoint(randomLocalPos);

            if (IsPositionClear(spawnWorldPos))
            {
                validPositionFound = true;
                break;
            }
        }

        if (validPositionFound)
        {
            GameObject newObj = Instantiate(prefabToSpawn, spawnWorldPos, Quaternion.identity);
            spawnedObjects.Add(newObj);
        }
    }

    private bool IsPositionClear(Vector3 targetWorldPos)
    {
        // Two spheres overlap if distance is less than sum of radii
        float minSafeDistance = objectRadius * 2f;
        float sqrMinSafeDistance = minSafeDistance * minSafeDistance;

        foreach (GameObject obj in spawnedObjects)
        {
            if (obj == null) continue;

            if (Vector3.SqrMagnitude(obj.transform.position - targetWorldPos) < sqrMinSafeDistance)
            {
                return false; // Too close to an existing object
            }
        }

        return true;
    }

    public void Hit(Vector3 hitPos, float hitRadius)
    {
        float combinedRadius = hitRadius + objectRadius;
        float sqrRadius = combinedRadius * combinedRadius;

        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = spawnedObjects[i];

            if (obj == null)
            {
                spawnedObjects.RemoveAt(i);
                continue;
            }

            if (Vector3.SqrMagnitude(obj.transform.position - hitPos) < sqrRadius)
            {
                Vector3 destroyedPos = obj.transform.position;
                spawnedObjects.RemoveAt(i);
                Destroy(obj);
                onObjectDestroyed?.Invoke(destroyedPos);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = boundsColor;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawSphere(Vector3.zero, objectRadius);
    }
}