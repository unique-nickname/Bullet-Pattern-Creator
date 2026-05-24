using UnityEngine;
using UnityEngine.UI;

public class TimelineRenderer : MaskableGraphic
{
    [Header("Timeline")]
    public float totalDurationMs = 12000f;
    public float baseVisibleDurationMs = 5000f;
    public float visibleStartMs = 0f;
    [SerializeField] private float visibleDurationMs;
    public float zoom = 1f;
    public float playheadTimeMs = 0f;

    [Header("UI")]
    public Scrollbar scrollbar;

    [Header("Ticks")]
    public float minorTickMs = 100f;
    public float majorTickMs = 500f;
    public float minorTickHeight = 10f;
    public float majorTickHeight = 20f;
    public float lineWidth = 1f;

    protected override void Start()
    {
        base.Start();
        RecalculateView();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect r = rectTransform.rect;

        DrawTicks(vh, r, minorTickMs, minorTickHeight);
        DrawTicks(vh, r, majorTickMs, majorTickHeight);
        DrawPlayhead(vh, r);
    }

    void DrawTicks(VertexHelper vh, Rect rect, float stepMs, float height)
    {
        if (stepMs <= 0f || visibleDurationMs <= 0f)
            return;

        float endMs = visibleStartMs + visibleDurationMs;
        float firstTick = Mathf.Ceil(visibleStartMs / stepMs) * stepMs;

        for (float t = firstTick; t <= endMs; t += stepMs) {
            float normalized = (t - visibleStartMs) / visibleDurationMs;
            float x = rect.xMin + normalized * rect.width;

            AddVerticalQuad(
                vh,
                x - lineWidth * 0.5f,
                rect.yMax - height,
                lineWidth,
                height,
                color
            );
        }
    }

    void DrawPlayhead(VertexHelper vh, Rect rect)
    {
        if (playheadTimeMs < visibleStartMs || playheadTimeMs > visibleStartMs + visibleDurationMs)
            return;

        float x = TimeToLocalX(playheadTimeMs);

        AddVerticalQuad(
            vh,
            x - 1f,
            rect.yMin,
            4f,
            rect.height,
            Color.red
        );
    }

    void AddVerticalQuad(VertexHelper vh, float x, float y, float width, float height, Color32 c)
    {
        int start = vh.currentVertCount;

        vh.AddVert(new Vector3(x, y), c, Vector2.zero);
        vh.AddVert(new Vector3(x + width, y), c, Vector2.zero);
        vh.AddVert(new Vector3(x + width, y + height), c, Vector2.zero);
        vh.AddVert(new Vector3(x, y + height), c, Vector2.zero);

        vh.AddTriangle(start + 0, start + 1, start + 2);
        vh.AddTriangle(start + 2, start + 3, start + 0);
    }

    public void Refresh()
    {
        SetVerticesDirty();
        UpdateScrollbarVisuals();
    }

    public void Pan(float completion)
    {
        float maxStart = Mathf.Max(0f, totalDurationMs - visibleDurationMs);
        visibleStartMs = maxStart * Mathf.Clamp01(completion);

        Refresh();
    }

    private void RecalculateView()
    {
        visibleDurationMs = baseVisibleDurationMs / zoom;
        visibleDurationMs = Mathf.Min(visibleDurationMs, totalDurationMs);

        float maxStart = Mathf.Max(0f, totalDurationMs - visibleDurationMs);
        visibleStartMs = Mathf.Clamp(visibleStartMs, 0f, maxStart);

        Refresh();
    }

    private void UpdateScrollbarVisuals()
    {
        if (scrollbar == null)
            return;

        float maxStart = Mathf.Max(0f, totalDurationMs - visibleDurationMs);

        scrollbar.size = totalDurationMs <= 0f
            ? 1f
            : Mathf.Clamp01(visibleDurationMs / totalDurationMs);

        scrollbar.value = maxStart <= 0f
            ? 0f
            : visibleStartMs / maxStart;
    }

    public float TimeToLocalX(float timeMs)
    {
        Rect rect = rectTransform.rect;
        float normalized = (timeMs - visibleStartMs) / visibleDurationMs;
        return rect.xMin + normalized * rect.width;
    }

    public float LocalXToTime(float localX)
    {
        Rect rect = rectTransform.rect;
        float normalized = Mathf.InverseLerp(rect.xMin, rect.xMax, localX);
        return visibleStartMs + normalized * visibleDurationMs;
    }

    public void SetPlayhead(float timeMs)
    {
        playheadTimeMs = Mathf.Clamp(timeMs, 0f, totalDurationMs);
        Refresh();
    }

    public void ZoomAroundLocalX(float localX, float zoomFactor)
    {
        zoom = Mathf.Clamp(zoom * zoomFactor, 0.25f, 4f);

        Rect rect = rectTransform.rect;

        float cursorNormalized = Mathf.InverseLerp(rect.xMin, rect.xMax, localX);
        float cursorTime = visibleStartMs + cursorNormalized * visibleDurationMs;

        float newVisibleDuration = baseVisibleDurationMs / zoom;
        newVisibleDuration = Mathf.Min(newVisibleDuration, totalDurationMs);

        visibleStartMs = cursorTime - cursorNormalized * newVisibleDuration;
        visibleDurationMs = newVisibleDuration;

        float maxStart = Mathf.Max(0f, totalDurationMs - visibleDurationMs);
        visibleStartMs = Mathf.Clamp(visibleStartMs, 0f, maxStart);

        Refresh();
        UpdateScrollbarVisuals();
    }
}