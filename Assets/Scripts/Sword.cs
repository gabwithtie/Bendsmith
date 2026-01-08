using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private List<Rigidbody> segmentbodies = new List<Rigidbody>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponentsInChildren<Rigidbody>(true, segmentbodies);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
