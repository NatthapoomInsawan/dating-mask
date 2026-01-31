using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static CharacterData;

public class DialogueCharacter : MonoBehaviour
{
    public string CharacterName => characterData.CharacterName;

    [Header("References")]
    [SerializeField] private Image characterImageRenderer;

    private CharacterData characterData;

    public void Init(CharacterData characterData)
    {
        this.characterData = characterData;
        SetCharacterExpression(CharacterExpression.Neutral);
    }

    public void SetCharacterExpression(CharacterExpression expressionKey)
    {
        CharacterExpressionData characterExpression = characterData.CharacterSprites.FirstOrDefault(spriteData => spriteData.ExpressionKey == expressionKey);

        if (characterExpression.Sprite == null)
        {
            Debug.LogWarning($"Expression '{expressionKey}' not found for character '{characterData.CharacterName}'.");
            return;
        }

        characterImageRenderer.sprite = characterExpression.Sprite;    
    }
}
