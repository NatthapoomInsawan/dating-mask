using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : Singleton<GameplayManager>
{
    public bool IsGameStopped => isGameStop;
    public GameDataManager GameDataManager => gameDataManager;
    public GameEventManager GameEventManager => gameEventManager;

    [Header("Manager References")]
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
