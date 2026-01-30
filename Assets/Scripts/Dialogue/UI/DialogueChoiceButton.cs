using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueData;

public class DialogueChoiceButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI choiceText;
    [SerializeField] private Button choiceButton;

    public void Init(DialogueChoice dialogueChoice, Action onClick = null)
    {
        choiceText.text = dialogueChoice.ChoiceKey;

        choiceButton.onClick.AddListener(() =>
        {
            onClick?.Invoke();
            choiceButton.interactable = false;
        });
    }
}
