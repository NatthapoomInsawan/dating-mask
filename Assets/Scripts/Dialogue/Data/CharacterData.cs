using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    #region class
    [Serializable]
    public struct CharacterExpression
    {
        public string ExpressionName;
        public Sprite Sprite;
    }
    #endregion

    public string CharacterName => characterName;
    public CharacterExpression[] CharacterSprites => characterSprites;

    [Header("Data")]
    [SerializeField] private string characterName;
    [SerializeField] private CharacterExpression[] characterSprites;
}
