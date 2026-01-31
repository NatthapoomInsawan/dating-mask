using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueData;

public class DialoguePanel : MonoBehaviour
{
    public bool IsDialogueCompleted => isDialogueCompleted;

    [Header("Prefabs")]
    [SerializeField] private DialogueCharacter dialogueCharacterPrefab;
    [SerializeField] private DialogueChoiceButton dialogueChoiceButtonPrefab;

    [Header("References")]
    [SerializeField] private RectTransform charactersContainer;
    [SerializeField] private RectTransform choiceContainer;

    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueSentenceText;

    [SerializeField] private Image sceneBackground;

    [SerializeField] private Button nextSentenceButton;

    [Header("Data")]
    [SerializeField] private DialogueData dialogueData;

    [Header("Dialouge Settings")]
    [SerializeField] private int typingDelay = 40;

    private List<DialogueCharacter> dialogueCharacters = new ();
    private List<DialogueChoiceButton> dialogueChoiceButtons = new ();

    private Queue<DialogueSentence> sentencesQueue = new Queue<DialogueSentence>();
    
    private DialogueSentence currentSentence;
    private DialogueCharacter currentCharacter;

    private CancellationTokenSource sentenceDisplayCts;

    private bool isDialogueCompleted = false;

    private void Awake()
    {
        nextSentenceButton.onClick.AddListener(() =>
        {
            if (isDialogueCompleted)
                return;

            if (!sentenceDisplayCts.IsCancellationRequested)
                CancelToken();
            else
                GetNextSentences();
        });
    }

    public void Init(DialogueData dialogueData)
    {
        isDialogueCompleted = false;
        this.dialogueData = dialogueData;

        if (dialogueData.SceneBackground != null)
        {
            sceneBackground.sprite = dialogueData.SceneBackground;
            sceneBackground.gameObject.SetActive(true);
        }
        else
            sceneBackground.gameObject.SetActive(false);

        foreach (var charData in dialogueData.GetCharacters())
        {
            DialogueCharacter dialogueCharacter = Instantiate(dialogueCharacterPrefab, charactersContainer);
            dialogueCharacter.Init(charData);
            dialogueCharacters.Add(dialogueCharacter);
        }

        gameObject.SetActive(true);

        sentencesQueue = new Queue<DialogueSentence>(dialogueData.GetDialogueSentences());
        
        GetNextSentences();
    }

    private void GetNextSentences()
    {
        if (sentencesQueue.Count == 0)
        {
            gameObject.SetActive(false);
            foreach (Transform child in charactersContainer)
                Destroy(child.gameObject);
            dialogueCharacters.Clear();
            isDialogueCompleted = true;
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
                if (await UniTask.Delay(typingDelay, cancellationToken: cts.Token).SuppressCancellationThrow())
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
                currentCharacter?.SetCharacterExpression(choice.ResponseExpressionKey);
                DisplaySentenceTask(choice.ResponseText, sentenceDisplayCts).Forget();

                foreach (var choiceEvent in choice.choiceEvents)
                    GameEventManager.Instance.TriggerCharacterEvents(choiceEvent, currentSentence.CharacterName);

                foreach (var button in dialogueChoiceButtons)
                    Destroy(button.gameObject);

                nextSentenceButton.interactable = true;
            });
            
            dialogueChoiceButtons.Add(dialogueChoiceButton);
        }

        nextSentenceButton.interactable = false;
    }

}
