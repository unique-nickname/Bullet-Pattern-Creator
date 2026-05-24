using UnityEngine;
using UnityEngine.EventSystems;

public class TimelineInput : MonoBehaviour, IScrollHandler, IPointerDownHandler, IDragHandler
{
    [SerializeField] private TimelineRenderer timeline;
    [SerializeField] private float scrollZoomStrength = 0.1f;

    public void OnScroll(PointerEventData eventData)
    {
        if (timeline == null)
            return;

        RectTransform rectTransform = timeline.rectTransform;
        Vector2 localPoint;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
            return;

        float scroll = eventData.scrollDelta.y;
        if (Mathf.Approximately(scroll, 0f))
            return;

        float zoomFactor = 1f + scroll * scrollZoomStrength;
        zoomFactor = Mathf.Max(0.1f, zoomFactor);

        timeline.ZoomAroundLocalX(localPoint.x, zoomFactor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetPlayheadFromPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetPlayheadFromPointer(eventData);
    }

    private void SetPlayheadFromPointer(PointerEventData eventData)
    {
        if (timeline == null)
            return;

        RectTransform rectTransform = timeline.rectTransform;
        Vector2 localPoint;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
            return;

        float timeMs = timeline.LocalXToTime(localPoint.x);
        timeline.SetPlayhead(timeMs);
    }
}