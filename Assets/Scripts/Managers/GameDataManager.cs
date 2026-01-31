using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData 
{
    public int Mood;
    public int Energy;
}

[Serializable]
public class CharacterAffinityData
{
    public string CharacterName;
    public int Affinity;
}


public class GameDataManager : MonoBehaviour
{
    public event Action<string> OnCharacterAfftinityFull;

    public event Action<int> OnPlayerEnergyChanged;
    public event Action<int> OnPlayerMoodChanged;
    public event Action<int, string> OnCharacterAffinityChanged;
    public int PlayerMood => playerData.Mood;
    public int PlayerEnergy => playerData.Energy;
    public int MaxAffinity => maxAffinity;
    public PlayerSettings PlayerSettings => playerSettings;

    public IEnumerable<CharacterData> CharacterDatas => characterDatas;

    [Header("Character Data")]
    [SerializeField] private List<CharacterData> characterDatas = new ();
    [SerializeField] private int maxAffinity = 60;

    [Header("Player Settings")]
    [SerializeField] private PlayerSettings playerSettings;

    [Header("Runtime Data")]
    [SerializeField] private PlayerData playerData = new();
    [SerializeField] private List<CharacterAffinityData> characterAffinityDatas = new ();

    public void Init()
    {
        playerData = new() { Mood = playerSettings.StartMood, Energy = playerSettings.StartEnergy };

        characterAffinityDatas.Clear();
        foreach (var characterData in characterDatas)
            characterAffinityDatas.Add(new CharacterAffinityData() { CharacterName = characterData.CharacterName, Affinity = 0 });

        GameplayManager.Instance.GameEventManager.OnPlayerEventTrigger += OnPlayerEventTrigger;
        GameplayManager.Instance.GameEventManager.OnCharacterEventTrigger += OnCharacterEventTrigger;
    }

    public int GetCharacterAffinity(string characterName)
    {
        CharacterAffinityData target = characterAffinityDatas.Find(data => data.CharacterName == characterName);

        if (target == null)
        {
            Debug.LogWarning($"can't find character name: {characterName}");
            return 0;
        }
        return target.Affinity;
    }

    private void OnPlayerEventTrigger(GameEvent gameEvent)
    {
        switch (gameEvent.Type)
        {
            case GameEvent.EventType.ModifyMood:
                playerData.Mood += gameEvent.Value;
                playerData.Mood = Mathf.Clamp(playerData.Mood, 0, playerSettings.MaxMood);
                OnPlayerMoodChanged?.Invoke(gameEvent.Value);
                break;
            case GameEvent.EventType.ModifyEnergy:
                playerData.Energy += gameEvent.Value;
                playerData.Energy = Mathf.Clamp(playerData.Energy, 0, playerSettings.MaxEnergy);
                OnPlayerEnergyChanged?.Invoke(gameEvent.Value);
                break;
        }
    }


    private void OnCharacterEventTrigger(GameEvent gameEvent, string characterName)
    {
        switch (gameEvent.Type)
        {
            case GameEvent.EventType.ModifyAffinity:
                var target = characterAffinityDatas.Find(data => data.CharacterName == characterName);
                if (target != null)
                    target.Affinity += gameEvent.Value;
                else
                    Debug.LogWarning($"Can't find character name: {characterName}");
                OnCharacterAffinityChanged?.Invoke(gameEvent.Value, characterName);
                break;
        }

        CharacterAffinityData characterAffinityFull = characterAffinityDatas.Find(data => data.Affinity >= maxAffinity);

        if (characterAffinityFull != null)
            OnCharacterAfftinityFull?.Invoke(characterAffinityFull.CharacterName);
    }


}
