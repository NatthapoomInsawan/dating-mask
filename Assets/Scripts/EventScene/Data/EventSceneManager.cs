using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventSceneManager : MonoBehaviour
{
    public event Action OnEventSceneChanged;

    public enum EventSceneType
    {
        Random,
        Character,
        Rest
    }

    [Header("References")]
    [SerializeField] private EventScenePanel eventScenePanel;
    [SerializeField] private DialoguePanel dialoguePanel;

    [Header("Character Event Scene Group")]
    [SerializeField] private List<EventSceneGroup> characterEventSceneGroups = new();

    [Header("Random Event Scene")]
    [SerializeField] private EventSceneGroup randomEventSceneGroup;

    [Header("Rest Event Scene")]
    [SerializeField] private EventScene tiredRestEventScene;
    [SerializeField] private EventScene goodRestEventScene;

    [Header("Settings")]
    [SerializeField] private int maxRandomEvent = 2;
    [SerializeField] private int maxCharacterEvent = 2;
    [SerializeField] private int maxEvents = 4;

    private Queue<EventSceneType> eventQueue = new();    

    [SerializeField] private List<EventScene> playedEventScene = new();

    private EventScene currentEventScene;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        ResetEventScene();
    }

    private void ResetEventScene()
    {
        playedEventScene.Clear();
        eventQueue.Clear();

        eventQueue = new();
        while (eventQueue.Count < maxEvents-1)
        {
            EventSceneType eventType = EventSceneType.Random;

            int randomEventCount = eventQueue.Count(e => e == EventSceneType.Random);
            int characterEventCount = eventQueue.Count(e => e == EventSceneType.Character);

            if (randomEventCount < maxRandomEvent && characterEventCount < maxCharacterEvent)
                eventType = (EventSceneType)Random.Range(0, 2);
            else if (randomEventCount >= maxRandomEvent)
                eventType = EventSceneType.Character;
            else
                eventType = EventSceneType.Random;

            Debug.Log($"event type {Random.Range(0, 2)}");

            switch (eventType)
            {
                case EventSceneType.Random:
                    eventQueue.Enqueue(EventSceneType.Random);
                    break;
                case EventSceneType.Character:
                    eventQueue.Enqueue(EventSceneType.Character);
                    break;
            }
        }

        eventQueue.Enqueue(EventSceneType.Rest);

        eventScenePanel.Init(this, eventQueue.ToList());

        ExcuteEventQueue();
    }

    private async void ExcuteEventQueue()
    {
        if (eventQueue.Count == 0)
        {
            ResetEventScene();
            return;
        }

        EventSceneType randomEventType = eventQueue.Dequeue();
        OnEventSceneChanged?.Invoke();

        switch (randomEventType)
        {
            case EventSceneType.Random:
                currentEventScene = randomEventSceneGroup.GetRandomEventScene(playedEventScene);
                break;
            case EventSceneType.Character:
                string characterEventName = await eventScenePanel.SelectCharacter();
                EventSceneGroup characterEventSceneGroup = characterEventSceneGroups.First(eventGroup => eventGroup.EventSceneGroupName == characterEventName);
                currentEventScene = characterEventSceneGroup.GetRandomEventScene(playedEventScene);
                break;
            case EventSceneType.Rest:
                bool isGoodRest = true;
                currentEventScene = isGoodRest ? goodRestEventScene : tiredRestEventScene;
                break;
        }

        StartDialogue(currentEventScene);
    }

    private async void StartDialogue(EventScene eventScene)
    {
        foreach (DialogueData dialogue in eventScene.GetDialogues())
        {
            dialoguePanel.Init(dialogue);
            await UniTask.WaitUntil(() => dialoguePanel.IsDialogueCompleted);
            await eventScenePanel.Transition();
        }

        foreach (GameEvent gameEvent in eventScene.GetEndEvents())
            GameEventManager.Instance.TriggerPlayerEvents(gameEvent);

        playedEventScene.Add(eventScene);
        ExcuteEventQueue();
    }
}
