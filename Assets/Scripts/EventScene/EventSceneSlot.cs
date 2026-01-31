using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventSceneManager;

public class EventSceneSlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Toggle selectedToggle;
    [SerializeField] private TextMeshProUGUI eventNameText;
    public void Init(EventSceneType type)
    {
        eventNameText.text = type.ToString();
    }

    public void SetToggle(bool value) => selectedToggle.isOn = value;

}
