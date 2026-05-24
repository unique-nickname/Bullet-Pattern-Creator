using UnityEngine;

public enum EditorTool
{
    Select = 0,
    Place = 1,
    Erase = 2
}

public class ToolHandler : MonoBehaviour
{
    public EditorTool CurrentTool { get; private set; } = EditorTool.Select;
    public PatternData patternData => PatternDataManager.Instance.CurrentPattern;
    public TimelineRenderer timelineRenderer;
    public BulletPatternScrubber scrubber;
    public InspectorUI inspectorUI;

    public BulletEvent selectedBullet;
    public Collider2D previewBullet;

    private BulletEvent savedBullet;

    public static ToolHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D)) {
            DuplicateBullet();
        }

        if (selectedBullet == null)
            return;
        if (previewBullet == null && selectedBullet.type != "") {
            previewBullet = GetPreviewFromEvent(selectedBullet)?.GetComponent<Collider2D>();
            if (previewBullet != null) {
                previewBullet.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public void HandleClick(Vector3 position)
    {
        switch (CurrentTool) {
            case EditorTool.Select:
                SelectBulletEvent(position);
                break;
            case EditorTool.Place:
                PlaceBulletEvent(position);
                break;
            case EditorTool.Erase:
                EraseBulletEvent(position);
                break;
        }
    }

    void PlaceBulletEvent(Vector2 position)
    {
        BulletEvent newBullet = new BulletEvent {
            type = "straight",
            direction = 0f,
            speed = 3f,
            scale = 0.5f,
            start_position = position,
            spawn_time = (int)timelineRenderer.playheadTimeMs,
            life_time = 1000
        };
        patternData.bullets.Add(newBullet);
        patternData.bullets.Sort((a, b) => a.spawn_time.CompareTo(b.spawn_time));
        selectedBullet = newBullet;
        inspectorUI.SetFields(selectedBullet);
        scrubber.RefreshPreview();
        if (previewBullet != null) {
            previewBullet.transform.GetChild(0).gameObject.SetActive(false);
        }
        previewBullet = GetPreviewFromEvent(selectedBullet).GetComponent<Collider2D>();
        if (previewBullet != null) {
            previewBullet.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    void EraseBulletEvent(Vector2 position)
    {
        Collider2D previewBullet = Physics2D.OverlapPoint(position, LayerMask.GetMask("PreviewWorld"), 0f, 0f);
        if (previewBullet == null) {
            return;
        }

        if (patternData.bullets[previewBullet.GetComponent<BulletPreview>().id] == selectedBullet) {
            selectedBullet = null;
            inspectorUI.SetFields(new BulletEvent());
        }

        patternData.bullets.Remove(patternData.bullets[previewBullet.GetComponent<BulletPreview>().id]);
        scrubber.ClearAllPreviewBullets();
        scrubber.RefreshPreview();
        Destroy(previewBullet.gameObject);
    }

    void SelectBulletEvent(Vector2 position)
    {
        if (previewBullet != null) {
            previewBullet.transform.GetChild(0).gameObject.SetActive(false);
        }
        previewBullet = Physics2D.OverlapPoint(position, LayerMask.GetMask("PreviewWorld"), 0f, 0f);
        if (previewBullet == null) {
            inspectorUI.SetFields(new BulletEvent());
            selectedBullet = null;
            return;
        }

        previewBullet.transform.GetChild(0).gameObject.SetActive(true);
        selectedBullet = patternData.bullets[previewBullet.GetComponent<BulletPreview>().id];
        inspectorUI.SetFields(selectedBullet);
    }

    public void UpdateBulletValues(BulletProperty property, float value)
    {
        if (selectedBullet == null)
            return;

        switch (property) {
            case BulletProperty.Speed:
                selectedBullet.speed = value;
                break;
            case BulletProperty.Direction:
                selectedBullet.direction = value;
                break;
            case BulletProperty.Scale:
                selectedBullet.scale = value;
                break;
            case BulletProperty.SpawnPositionX:
                selectedBullet.start_position.x = value;
                break; 
            case BulletProperty.SpawnPositionY:
                selectedBullet.start_position.y = value;
                break;
            case BulletProperty.Lifetime:
                selectedBullet.life_time = (int)value;
                break;
            case BulletProperty.SpawnTime:
                selectedBullet.spawn_time = (int)value;
                break;
        }
        scrubber.RefreshPreview();
    }

    void DuplicateBullet()
    {
        if (selectedBullet == null)
            return;

        BulletEvent newBullet = new BulletEvent {
            type = selectedBullet.type,
            direction = selectedBullet.direction,
            speed = selectedBullet.speed,
            scale = selectedBullet.scale,
            start_position = selectedBullet.start_position,
            spawn_time = (int)timelineRenderer.playheadTimeMs,
            life_time = selectedBullet.life_time
        };
        patternData.bullets.Add(newBullet);
        patternData.bullets.Sort((a, b) => a.spawn_time.CompareTo(b.spawn_time));
        selectedBullet = newBullet;
        inspectorUI.SetFields(selectedBullet);
        scrubber.RefreshPreview();
        if (previewBullet != null) {
            previewBullet.transform.GetChild(0).gameObject.SetActive(false);
        }
        previewBullet = GetPreviewFromEvent(selectedBullet).GetComponent<Collider2D>();
        if (previewBullet != null) {
            previewBullet.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    // -- Helpers --

    public void SetTool(int tool)
    {
        CurrentTool = (EditorTool)tool;
    }

    GameObject GetPreviewFromEvent(BulletEvent bulletEvent)
    {
        int index = patternData.bullets.IndexOf(bulletEvent);
        foreach (var bullet in scrubber.bulletNodes) {
            if (bullet.Key == index) {
                return bullet.Value;
            }
        }
        return null;
    }
}