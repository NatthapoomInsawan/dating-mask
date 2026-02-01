
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EventSceneManager;

public class EventSceneSlot : MonoBehaviour
{
    [Serializable]
    private class EventSceneIconData
    {
        public EventSceneType key;
        public Sprite IconSprite;
    }

    [Header("References")]
    [SerializeField] private Toggle selectedToggle;
    [SerializeField] private Image iconImage;

    [Header("Sprite")]
    [SerializeField] private List<EventSceneIconData> iconDatas;

    public void Init(EventSceneType type)
    {
        EventSceneIconData iconData = iconDatas.Find(data => data.key == type);

        if (iconData != null)
            iconImage.sprite = iconData.IconSprite;
        else
            Debug.LogWarning($"No icon with type {type.ToString()}");

    }

    public void SetToggle(bool value) => selectedToggle.isOn = value;

}
