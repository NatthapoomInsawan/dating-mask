using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EventScene/EventSceene/Data")]
public class EventScene : ScriptableObject
{
    [Header("Dialogues")]
    [SerializeField] private List<DialogueData> dialogues = new();

    [Header("Event End")]
    [SerializeField] private List<GameEvent> endEvents = new();
    public IEnumerable<DialogueData> GetDialogues() => dialogues;
    public IEnumerable<GameEvent> GetEndEvents() => endEvents;

}
