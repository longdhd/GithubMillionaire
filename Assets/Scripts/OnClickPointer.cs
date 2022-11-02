using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickPointer : MonoBehaviour, IPointerDownHandler
{
    public enum PointerState
    {
        BLANK,
        LOCK,
        FINAL
    }
    public PointerState pointerState = PointerState.BLANK;
    private static GameObject LASTPOINTERDOWNOBJECT = null;//Need to record the last click event's object, so that double clicking doesn't occur when skipping across objects (avoid accidental double click). I tried looking through PointerEventData, but none of the values I expected came back with a record of the previously clicked/hovered object.
    public void OnPointerDown(PointerEventData eventData)
    {
        bool bSameObjectClicked = LASTPOINTERDOWNOBJECT == eventData.pointerCurrentRaycast.gameObject;

        if (!bSameObjectClicked)
            pointerState = PointerState.LOCK;

        if (bSameObjectClicked)
        {
            pointerState = PointerState.FINAL;
        }

        LASTPOINTERDOWNOBJECT = eventData.pointerCurrentRaycast.gameObject;

    }
}