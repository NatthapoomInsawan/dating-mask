using System;
using System.Collections.Generic;
using UnityEngine;
using static CharacterData;

[CreateAssetMenu(menuName = "Dialogue/DialogueData/Dialogue")]
public class DialogueData : ScriptableObject
{
    [Serializable]
    public class DialogueSentence
    {
        public string CharacterName;
        public CharacterExpression ExpressionKey;
        public string SentenceText;

        [SerializeField] public List<DialogueChoice> Choices = new();

        [SerializeField] private List<string> characterNames = new();

#if UNITY_EDITOR
        public void CacheCharacterNames(IEnumerable<CharacterData> characters)
        {
            characterNames.Clear();
            foreach (var character in characters)
            {
                if (character == null)
                    continue;
                characterNames.Add(character.CharacterName);
            }
            characterNames.RemoveAll(character => character == null);

            if (!characterNames.Contains(CharacterName))
                CharacterName = characterNames.Count > 0 ? characterNames[0] : string.Empty;

        }
#endif

    }

    [Serializable]
    public class DialogueChoice
    {
        public string ChoiceKey;
        public CharacterExpression ResponseExpressionKey;
        public string ResponseText;
        public List<GameEvent> choiceEvents = new();
    }

    public Sprite SceneBackground => sceneBackground;

    [Header("Scene Background")]
    [SerializeField] private Sprite sceneBackground;
    [Header("Characters")]
    [SerializeField] private List<CharacterData> characters = new();
    [Header("Sentences")]
    [SerializeReference] private List<DialogueSentence> dialogueSentences = new();

    public IEnumerable<CharacterData> GetCharacters() => characters;
    public IEnumerable<DialogueSentence> GetDialogueSentences() => dialogueSentences;

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var sentence in dialogueSentences)
        {
            if (sentence == null)
                continue;
            sentence.CacheCharacterNames(characters);
        }   
    }
#endif


}
