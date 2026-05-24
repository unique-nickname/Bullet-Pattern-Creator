using System.Collections.Generic;
using UnityEngine;

public class BulletPatternScrubber : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimelineRenderer timelineRenderer;
    [SerializeField] private Transform bulletPreviewParent;

    [Header("Bullet Prefabs")]
    [SerializeField] private GameObject previewBulletPrefab;

    // Current previewed bullet instances
    public readonly Dictionary<int, GameObject> bulletNodes = new();

    private int currentTimeMs = -1;

    private void Update()
    {
        if (timelineRenderer == null)
            return;

        int newTimeMs = Mathf.FloorToInt(timelineRenderer.playheadTimeMs);

        if (newTimeMs != currentTimeMs) {
            currentTimeMs = newTimeMs;
            RefreshPreview();
        }
    }

    public void RefreshPreview()
    {
        if (PatternDataManager.Instance.Bullets == null)
            return;

        for (int i = 0; i < PatternDataManager.Instance.Bullets.Count; i++) {
            BulletEvent bullet = PatternDataManager.Instance.Bullets[i];
            if (IsBulletAlive(bullet, currentTimeMs)) {
                Vector2 pos = GetBulletPosition(bullet, currentTimeMs);

                if (!bulletNodes.TryGetValue(i, out GameObject node) || node == null) {
 
                    node = Instantiate(
                        previewBulletPrefab,
                        bulletPreviewParent != null ? bulletPreviewParent : transform
                    );

                    BulletPreview preview = node.GetComponent<BulletPreview>();
                    if (preview != null) {
                        preview.id = i;
                        preview.direction = bullet.direction;
                        preview.speed = bullet.speed;
                        preview.scale = bullet.scale;
                        preview.lifeTime = bullet.life_time;
                    }
                        
                    bulletNodes[i] = node;
                }

                node.transform.localPosition = pos;
            } else {
                if (bulletNodes.TryGetValue(i, out GameObject node) && node != null) {
                    Destroy(node);
                }

                bulletNodes.Remove(i);
            }
        }
    }

    private bool IsBulletAlive(BulletEvent bullet, int t)
    {
        return t >= bullet.spawn_time && t <= bullet.spawn_time + bullet.life_time;
    }

    public Vector2 GetBulletPosition(BulletEvent bullet, int t)
    {
        float elapsed = (t - bullet.spawn_time) / 1000f;
        if (elapsed < 0f)
            return Vector2.zero;

        switch (bullet.type) {
            case "straight": {
                    float radians = bullet.direction * Mathf.Deg2Rad;
                    Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                    Vector2 scrubbedPosition = direction * bullet.speed * elapsed;
                    return bullet.start_position + scrubbedPosition;
                }

            default:
                return bullet.start_position;
        }
    }

    public void ClearAllPreviewBullets()
    {
        foreach (GameObject node in bulletNodes.Values) {
            if (node != null)
                Destroy(node);
        }

        bulletNodes.Clear();
    }
}