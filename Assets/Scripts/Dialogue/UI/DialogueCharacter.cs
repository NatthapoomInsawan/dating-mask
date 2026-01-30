using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCharacter : MonoBehaviour
{
    public string CharacterName => characterData.CharacterName;

    [Header("References")]
    [SerializeField] private Image characterImageRenderer;

    private CharacterData characterData;

    public void Init(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    public void SetCharacterExpression(string expressionKey)
    {
       CharacterData.CharacterExpression characterExpression = characterData.CharacterSprites.FirstOrDefault(spriteData => spriteData.ExpressionName == expressionKey);

        if (characterExpression.Sprite == null)
        {
            Debug.LogWarning($"Expression '{expressionKey}' not found for character '{characterData.CharacterName}'.");
            return;
        }

        characterImageRenderer.sprite = characterExpression.Sprite;    
    }
}
