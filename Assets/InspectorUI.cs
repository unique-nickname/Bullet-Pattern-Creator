using UnityEngine;
public enum BulletProperty
{
    Speed,
    Direction,
    Scale,
    SpawnPositionX,
    SpawnPositionY,
    Lifetime,
    SpawnTime
}

public class InspectorUI : MonoBehaviour
{
    [SerializeField] private PropertyInputField speedField;
    [SerializeField] private PropertyInputField directionField;
    [SerializeField] private PropertyInputField scaleField;
    [SerializeField] private PropertyInputField spawnPositionXField;
    [SerializeField] private PropertyInputField spawnPositionYField;
    [SerializeField] private PropertyInputField spawnTimeField;
    [SerializeField] private PropertyInputField lifetimeField;

    private void Start()
    {
        speedField.Initialize(BulletProperty.Speed, this);
        directionField.Initialize(BulletProperty.Direction, this);
        scaleField.Initialize(BulletProperty.Scale, this);
        spawnPositionXField.Initialize(BulletProperty.SpawnPositionX, this);
        spawnPositionYField.Initialize(BulletProperty.SpawnPositionY, this);
        spawnTimeField.Initialize(BulletProperty.SpawnTime, this);
        lifetimeField.Initialize(BulletProperty.Lifetime, this);
    }

    public void OnPropertyChanged(BulletProperty property, string value)
    {
        float parsedValue = float.Parse(value);
        ToolHandler.Instance.UpdateBulletValues(property, parsedValue);
    }

    public void SetFields(BulletEvent bullet)
    {
        speedField.SetTextWithoutNotify(bullet.speed.ToString());
        directionField.SetTextWithoutNotify(bullet.direction.ToString());
        scaleField.SetTextWithoutNotify(bullet.scale.ToString());
        spawnPositionXField.SetTextWithoutNotify(bullet.start_position.x.ToString());
        spawnPositionYField.SetTextWithoutNotify(bullet.start_position.y.ToString());
        spawnTimeField.SetTextWithoutNotify(bullet.spawn_time.ToString());
        lifetimeField.SetTextWithoutNotify(bullet.life_time.ToString());
    }
}