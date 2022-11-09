using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickPointer : MonoBehaviour, IPointerDownHandler
{
    public enum PointerState
    {
        IDLE,
        LOCK,
        FINAL
    }
    public PointerState pointerState = PointerState.IDLE;
    private static GameObject LASTPOINTERDOWNOBJECT = null;//Need to record the last click event's object, so that double clicking doesn't occur when skipping across objects (avoid accidental double click). I tried looking through PointerEventData, but none of the values I expected came back with a record of the previously clicked/hovered object.
    const float doubleClickTime = 0.5f;
    const float singleClickTime = 0.2f;
    public void OnPointerDown(PointerEventData eventData)
    {
        bool bSameObjectClicked = LASTPOINTERDOWNOBJECT == eventData.pointerCurrentRaycast.gameObject;
        float timeSinceLastClick = Time.unscaledTime - eventData.clickTime;
        if (!bSameObjectClicked)
            pointerState = PointerState.LOCK;

        if (timeSinceLastClick > singleClickTime && bSameObjectClicked)
        {
            pointerState = PointerState.FINAL;
        }

        LASTPOINTERDOWNOBJECT = eventData.pointerCurrentRaycast.gameObject;
    } 
}