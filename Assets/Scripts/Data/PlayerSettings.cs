using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public int StartMood => startMood;
    public int MaxMood => maxMood;
    public int StartEnergy => startEnergy;
    public int MaxEnergy => maxEnergy;

    [Header("Settings")]
    [SerializeField] private int startMood = 60;
    [SerializeField] private int maxMood = 60;
    [SerializeField] private int startEnergy = 60;
    [SerializeField] private int maxEnergy = 60;
}
