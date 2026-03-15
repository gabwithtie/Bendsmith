using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class MaterialPropertyController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Material targetMaterial;
    [SerializeField] private string propertyName = "_FloatValue";

    // Storing the ID is more efficient than using the string name repeatedly
    private int propertyID;
    private float value_cache;

    void Awake()
    {
        if (targetMaterial != null)
        {
            // Convert the string name to a unique integer ID
            propertyID = Shader.PropertyToID(propertyName);
        }
        else
        {
            Debug.LogWarning("MaterialPropertyController: No material assigned!");
        }
    }

    /// <summary>
    /// Public function to update the float property from other scripts or UI events.
    /// </summary>
    /// <param name="newValue">The value to set the property to.</param>
    public void UpdateFloatProperty(float newValue)
    {
        SetOnly(newValue);
        CommitValue();
    }
    public void SetOnly(float newValue)
    {
        value_cache = newValue;
    }

    public void CommitValue()
    {
        if (targetMaterial != null)
        {
            targetMaterial.SetFloat(propertyID, value_cache);
        }
    }
}