using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Serializable]
    public enum CharacterExpression
    {
        Neutral = 0,
        Happy = 1,
        Sad = 2,
        Angry = 3,
        Surprised
    }

    #region class
    [Serializable]
    public struct CharacterExpressionData
    {
        public CharacterExpression ExpressionKey;
        public Sprite Sprite;
    }
    #endregion

    public string CharacterName => characterName;
    public CharacterExpressionData[] CharacterSprites => characterSprites;

    [Header("Data")]
    [SerializeField] private string characterName;
    [SerializeField] private CharacterExpressionData[] characterSprites;
}
