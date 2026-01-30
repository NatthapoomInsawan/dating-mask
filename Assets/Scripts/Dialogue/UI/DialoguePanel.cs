using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static DialogueData;

public class DialoguePanel : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private DialogueCharacter dialogueCharacterPrefab;
    [SerializeField] private DialogueChoiceButton dialogueChoiceButtonPrefab;

    [Header("References")]
    [SerializeField] private RectTransform charactersContainer;
    [SerializeField] private RectTransform choiceContainer;

    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueSentenceText;

    [SerializeField] private Button nextSentenceButton;

    [Header("Data")]
    [SerializeField] private DialogueData dialogueData;

    private List<DialogueCharacter> dialogueCharacters = new ();

    private List<DialogueChoiceButton> dialogueChoiceButtons = new ();

    private Queue<DialogueSentence> sentencesQueue = new Queue<DialogueSentence>();
    
    private DialogueSentence currentSentence;

    private DialogueCharacter currentCharacter;

    private CancellationTokenSource sentenceDisplayCts;

    private void Awake()
    {
        Init();

        nextSentenceButton.onClick.AddListener(() =>
        {
            if (!sentenceDisplayCts.IsCancellationRequested)
                CancelToken();
            else
                GetNextSentences();
        });
    }

    public void Init()
    {
        foreach(var charData in dialogueData.GetCharacters())
        {
            DialogueCharacter dialogueCharacter = Instantiate(dialogueCharacterPrefab, charactersContainer);
            dialogueCharacter.Init(charData);
            dialogueCharacters.Add(dialogueCharacter);
        }

        sentencesQueue = new Queue<DialogueSentence>(dialogueData.GetDialogueSentences());
        
        GetNextSentences();
    }

    private void GetNextSentences()
    {
        if (sentencesQueue.Count == 0)
        {
            Debug.Log("Dialogue ended.");
            return;
        }

        if (currentSentence != null && dialogueChoiceButtons.Count == 0)
        {
            if (currentSentence.Choices.Count > 0)
            {
                InitChoice(currentSentence.Choices);
                return;
            }
        }
        else
            dialogueChoiceButtons.Clear();

       currentSentence = sentencesQueue.Dequeue();
       InitSentences(currentSentence);
    }

    private async UniTask DisplaySentenceTask(string sentenceText, CancellationTokenSource cts)
    {
        dialogueSentenceText.text = string.Empty;

        while (dialogueSentenceText.text.Length < sentenceText.Length && !cts.IsCancellationRequested)
        {
            foreach (char character in sentenceText)
            {
                if (await UniTask.Delay(30, cancellationToken: cts.Token).SuppressCancellationThrow())
                    break;

                dialogueSentenceText.text += character;
            }
        }

        if (dialogueSentenceText.text.Length < sentenceText.Length)
            dialogueSentenceText.text = sentenceText;
    }

    private void CancelToken()
    {
        if (sentenceDisplayCts != null)
            sentenceDisplayCts.Cancel();
    }

    private void InitSentences(DialogueSentence sentence)
    {
        sentenceDisplayCts = new CancellationTokenSource();
        characterNameText.text = sentence.CharacterName;
        currentCharacter = dialogueCharacters.FirstOrDefault(character => character.CharacterName == sentence.CharacterName);
        currentCharacter?.SetCharacterExpression(sentence.ExpressionKey);
        DisplaySentenceTask(sentence.SentenceText, sentenceDisplayCts).Forget();
    }

    private void InitChoice(List<DialogueChoice> choices)
    {
        foreach (var choice in choices)
        {
            DialogueChoiceButton dialogueChoiceButton = Instantiate(dialogueChoiceButtonPrefab, choiceContainer);
            dialogueChoiceButton.Init(choice, () =>
            {
                sentenceDisplayCts = new CancellationTokenSource();
                currentCharacter?.SetCharacterExpression(choice.ResponseExpression);
                DisplaySentenceTask(choice.ResponseText, sentenceDisplayCts).Forget();

                foreach (var button in dialogueChoiceButtons)
                    Destroy(button.gameObject);
            });
            
            dialogueChoiceButtons.Add(dialogueChoiceButton);
        }
    }

}
