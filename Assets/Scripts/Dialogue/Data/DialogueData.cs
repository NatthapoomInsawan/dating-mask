using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueData/Dialogue")]
public class DialogueData : ScriptableObject
{

    [Serializable]
    public class DialogueSentence
    {
        public string CharacterName;
        public string ExpressionKey;
        public string SentenceText;

        [SerializeField] public List<DialogueChoice> Choices = new();
    }

    [Serializable]
    public class DialogueChoice
    {
        public string ChoiceKey;
        public string ResponseExpression;
        public string ResponseText; 
    }

    [Header("Characters")]
    [SerializeField] private List<CharacterData> characters = new();
    [Header("Sentences")]
    [SerializeReference] private List<DialogueSentence> dialogueSentences = new();

    public IEnumerable<CharacterData> GetCharacters() => characters;
    public IEnumerable<DialogueSentence> GetDialogueSentences() => dialogueSentences;

}
