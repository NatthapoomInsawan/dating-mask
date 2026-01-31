using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EventSceneManager;

public class EventScenePanel : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private EventSceneSlot eventSceneSlotPrefab;
    [SerializeField] protected EventSelectCharacterButton selectCharacterButtonPrefab;

    [Header("References")]
    [SerializeField] private RectTransform eventSceneProgressContainer;
    [SerializeField] private RectTransform selectCharacterContainer;

    [SerializeField] private Image transition;

    private List<EventSceneSlot> eventSceneSlots = new ();

    private int currentStepIndex = -1;

    public void Init(EventSceneManager eventSceneManager, List<EventSceneType> eventScenes)
    {
        eventSceneManager.OnEventSceneChanged -= OnUpdateStep;
        eventSceneManager.OnEventSceneChanged += OnUpdateStep;


        ClearChildTransfom(eventSceneProgressContainer);
        eventSceneSlots.Clear();

        currentStepIndex = -1;
        foreach (var sceneType in eventScenes)
        {
            EventSceneSlot slot = Instantiate(eventSceneSlotPrefab, eventSceneProgressContainer);
            slot.Init(sceneType);
            eventSceneSlots.Add(slot);
        }
    }

    public async UniTask<string> SelectCharacter()
    {
        CharacterData selectedCharacter = null;

        foreach (var character in GameplayManager.Instance.GameDataManager.CharacterDatas)
        {
            EventSelectCharacterButton selectCharacterButton = Instantiate(selectCharacterButtonPrefab, selectCharacterContainer);
            selectCharacterButton.Init(character, () =>
            {
                selectedCharacter = character;
            });
        }

        await UniTask.WaitUntil(()=>selectedCharacter != null);
       
        ClearChildTransfom(selectCharacterContainer);
        return selectedCharacter.CharacterName;
    }

    public async UniTask Transition()
    {
        bool transitionCompletd = false;
        transition.DOFade(1f, 0.5f).OnComplete(() =>
        {
            transition.DOFade(0f, 0.5f).OnComplete(() =>
            {
                transitionCompletd = true;
            });
        });

        await UniTask.WaitUntil(() => transitionCompletd);
    }


    private void ClearChildTransfom(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

    private void OnUpdateStep()
    {
        if (GameplayManager.Instance.GameDataManager.PlayerEnergy > 0)
            currentStepIndex++;
        else
            currentStepIndex = eventSceneSlots.Count - 1;

        for (int i = 0; i < eventSceneSlots.Count; i++)
            eventSceneSlots[i].SetToggle(i == currentStepIndex);
    }

}
