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

    [Header("Player Stat")]
    [SerializeField] private IconFillDial energyFillDial;
    [SerializeField] private IconFillDial moodFillDial;

    [Header("Transition")]
    [SerializeField] private Image transition;

    private List<EventSceneSlot> eventSceneSlots = new ();

    private int currentStepIndex = -1;

    private bool isInit;

    public void Init(EventSceneManager eventSceneManager, List<EventSceneType> eventScenes)
    {
        if (!isInit)
        {
            energyFillDial.Init(GameplayManager.Instance.GameDataManager.PlayerEnergy, GameplayManager.Instance.GameDataManager.PlayerSettings.MaxEnergy);
            moodFillDial.Init(GameplayManager.Instance.GameDataManager.PlayerMood, GameplayManager.Instance.GameDataManager.PlayerSettings.MaxMood);

            GameplayManager.Instance.GameDataManager.OnPlayerEnergyChanged += (changedValue) =>
            {
                energyFillDial.SetValue(GameplayManager.Instance.GameDataManager.PlayerEnergy);
            };

            GameplayManager.Instance.GameDataManager.OnPlayerMoodChanged += (changedValue) =>
            {
                moodFillDial.SetValue(GameplayManager.Instance.GameDataManager.PlayerMood);
            };
        }

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

        isInit = true;
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
