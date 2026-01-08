using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GabUnity
{
    [RequireComponent(typeof(Collider))]
    public class PointerCatcher : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private UnityEvent<Vector3> OnClick;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            // 1. Retrieve the world position from the event's raycast result
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            
            OnClick.Invoke(clickPosition);
        }
    }
}