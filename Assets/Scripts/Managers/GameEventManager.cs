using System;
using UnityEngine;

[Serializable]
public class GameEvent
{
    [Serializable]
    public enum EventType
    {
        ModifyMood,
        ModifyAffinity,
        ModifyEnergy
    }

    public EventType Type;
    public int Value;
}

public class GameEventManager : Singleton<GameEventManager>
{
    public event Action<GameEvent> OnPlayerEventTrigger;
    public event Action<GameEvent, string> OnCharacterEventTrigger;
    public void TriggerPlayerEvents(GameEvent gameEvent)
    {
        Debug.Log($"GameEvent Triggered: Type={gameEvent.Type}, Value={gameEvent.Value}");
        OnPlayerEventTrigger?.Invoke(gameEvent);
    }

    public void TriggerCharacterEvents(GameEvent gameEvent, string characterName)
    {
        Debug.Log($"GameEvent Triggered for {characterName}: Type={gameEvent.Type}, Value={gameEvent.Value}");
        OnCharacterEventTrigger?.Invoke(gameEvent, characterName);
    }

}
