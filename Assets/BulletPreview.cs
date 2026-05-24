using Unity.VisualScripting;
using UnityEngine;

public class BulletPreview : MonoBehaviour
{
    public float direction;
    public float speed;
    public float scale;
    public float lifeTime;
    public int id;

    private void Start()
    {
        transform.localScale = Vector3.one * scale;
        transform.rotation = Quaternion.Euler(0, 0, direction);
    }

    private void Update()
    {
        transform.localScale = Vector3.one * scale;
        transform.rotation = Quaternion.Euler(0, 0, direction);
    }
}
