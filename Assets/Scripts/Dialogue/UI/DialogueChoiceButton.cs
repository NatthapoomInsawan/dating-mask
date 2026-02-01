using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueData;

public class DialogueChoiceButton : MonoBehaviour
{
    [Serializable]
    private class MaskData
    {
        public string Key;
        public Sprite MaskSprite;
    }

    [Header("References")]
    [SerializeField] private Button choiceButton;

    [Header("Mask Sprite")]
    [SerializeField] private List<MaskData> maskDatas = new();

    public void Init(DialogueChoice dialogueChoice, Action onClick = null)
    {
        choiceButton.onClick.AddListener(() =>
        {
            onClick?.Invoke();
            choiceButton.interactable = false;
        });
        
        MaskData maskData = maskDatas.Find(data => data.Key == dialogueChoice.ChoiceKey);
        if (maskData == null)
        {
            Debug.LogWarning($"No mask with choicekey: {dialogueChoice.ChoiceKey}");
            return;
        }

        choiceButton.image.sprite = maskData.MaskSprite;
    }
}
