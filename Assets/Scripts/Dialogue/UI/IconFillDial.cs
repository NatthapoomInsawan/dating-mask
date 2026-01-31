using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IconFillDial : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI valueText;

    private int currentValue;
    private int maxValue;

    public void Init(int currentValue, int maxValue)
    {
        this.currentValue = currentValue;
        this.maxValue = maxValue;

        UpdateFillImage();
    }

    public void SetValue(int newValue)
    {
        currentValue = newValue;
        UpdateFillImage ();
    }

    private void UpdateFillImage()
    {
        fillImage.fillAmount = ((float) currentValue) / maxValue;
        valueText.text = $"{currentValue}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        valueText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        valueText.gameObject.SetActive(false);
    }


}
