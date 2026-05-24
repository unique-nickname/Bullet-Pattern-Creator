using UnityEngine;

public class StraightBullet : MonoBehaviour
{

    public float direction;
    public float speed;
    public float scale;
    public float lifeTime;
    public int id;

    float timer;

    void Start()
    {
        transform.localScale = Vector3.one * scale;
        transform.rotation = Quaternion.Euler(0, 0, direction);
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.right * speed * Time.fixedDeltaTime);

        timer += Time.fixedDeltaTime;
        if (timer * 1000 >= lifeTime) {
            Destroy(gameObject);
        }
    }
}
