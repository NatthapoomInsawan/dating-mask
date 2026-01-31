using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EventScene/EventSceneGroup")]
public class EventSceneGroup : ScriptableObject
{
    public string EventSceneGroupName;
    [Header("Event Scenes")]
    [SerializeField] private List<EventScene> eventScenes;

    public IEnumerable<EventScene> GetAllEventScnes() => eventScenes;

    public EventScene GetRandomEventScene(List<EventScene> duplicatedScene = null)
    {
        if (eventScenes.Count == 0)
        {
            Debug.LogWarning($"EventSceneGroup '{EventSceneGroupName}' has no EventScenes.");
            return null;
        }
        int randomIndex = Random.Range(0, eventScenes.Count);

        if (duplicatedScene != null)
        {
            if (duplicatedScene.Contains(eventScenes[randomIndex]))
                return GetRandomEventScene(duplicatedScene);
        }

        return eventScenes[randomIndex];
    }


}
