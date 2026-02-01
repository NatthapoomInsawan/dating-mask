using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static CharacterData;

public class EventSelectCharacterButton : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Image characterImage;
    [SerializeField] private Button button;

    public void Init(CharacterData characterData, Action onclick)
    {
        characterImage.sprite = characterData.CharacterSprites.FirstOrDefault(c => c.ExpressionKey == CharacterExpression.Neutral).Sprite;
        button.onClick.AddListener(() =>
        {
            GameplayManager.Instance.AudioManager.PlaySFX("click");
            onclick?.Invoke();
        });
    }

}
