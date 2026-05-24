using TMPro;
using UnityEngine;

public class PropertyInputField : MonoBehaviour
{
    private BulletProperty property;
    private InspectorUI inspector;

    [SerializeField] private TMP_InputField inputField;

    public void Initialize(BulletProperty property, InspectorUI inspector)
    {
        this.property = property;
        this.inspector = inspector;

        inputField.onValueChanged.RemoveListener(HandleChanged);
        inputField.onValueChanged.AddListener(HandleChanged);
    }

    private void HandleChanged(string value)
    {
        if (value == "" || value == "-" || value == ".")
            value = "0";

        inspector.OnPropertyChanged(property, value);
    }

    public void SetTextWithoutNotify(string value)
    {
        inputField.SetTextWithoutNotify(value);
    }
}