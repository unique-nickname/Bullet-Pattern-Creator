using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PreviewInput : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private RawImage previewImage;
    [SerializeField] private Camera previewCamera;

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransform rectTransform = previewImage.rectTransform;

        Vector2 localPoint;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint)) {
            return;
        }

        Rect rect = rectTransform.rect;

        float normalizedX = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
        float normalizedY = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

        Vector3 viewportPoint = new Vector3(
            normalizedX,
            normalizedY,
            Mathf.Abs(previewCamera.transform.position.z)
        );

        Vector3 worldPoint = previewCamera.ViewportToWorldPoint(viewportPoint);

        ToolHandler.Instance.HandleClick(worldPoint);
    }
}