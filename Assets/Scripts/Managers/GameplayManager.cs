using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : Singleton<GameplayManager>
{
    public bool IsGameStopped => isGameStop;
    public GameDataManager GameDataManager => gameDataManager;
    public GameEventManager GameEventManager => gameEventManager;
    public AudioManager AudioManager => audioManager;

    [Header("Manager References")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameDataManager gameDataManager;
    [SerializeField] private GameEventManager gameEventManager;
    [SerializeField] private EventSceneManager eventSceneManager;

    private bool isGameStop = false;

    public void Init()
    {
        try
        {
            gameDataManager.Init();
            eventSceneManager.Init();
        }
        catch (Exception e)
        {
            Debug.LogError($"Game Manager Init Error! Exception thrown: {e}");
        }

        GameStart();
    }

    private void GameStart()
    {
        AudioManager.PlayBGM("silly");
        isGameStop = false;
        gameDataManager.OnCharacterAfftinityFull += (characterName) =>
        {
            isGameStop = true;
            eventSceneManager.GoodEndDialogue(characterName);
        };
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
