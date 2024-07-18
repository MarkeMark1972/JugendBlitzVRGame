using JugendBlitz.scripts.runtime;
using JugendBlitz.scripts.runtime.Audio;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int ItemsInGame;
    public static bool GameInProgress;
    public static bool DidWinGame;

    private void Start()
    {
        // MusicPlayer.OnStartMusic?.Invoke(MusicType.MainMenuMusic);
        MusicPlayer.StartMusic(MusicType.MainMenuMusic);
    }

    private void OnEnable()
    {
        ConveyorBeltManager.SpawnNextItem += SpawnNextItem;
        ConveyorBeltManager.OnStartGame += StartGame;
        ConveyorBeltManager.OnEndGame += EndGame;
    }

    private void OnDisable()
    {
        ConveyorBeltManager.SpawnNextItem -= SpawnNextItem;
        ConveyorBeltManager.OnStartGame -= StartGame;
        ConveyorBeltManager.OnEndGame -= EndGame;
    }

    private void OnDestroy()
    {
        ConveyorBeltManager.SpawnNextItem -= SpawnNextItem;
        ConveyorBeltManager.OnStartGame -= StartGame;
        ConveyorBeltManager.OnEndGame -= EndGame;
    }

    private void StartGame()
    {
        GameSettings.instance.CorrectItemCounter = 0;
        GameInProgress = true;
        DidWinGame = false;

        MusicPlayer.StopAllAudio();
        MusicPlayer.StartMusic(MusicType.GameMusic);
    }

    private void EndGame()
    {
        GameInProgress = false;

        MusicPlayer.StopAllAudio();
        MusicPlayer.StartMusic(MusicType.MainMenuMusic);
        if (DidWinGame) MusicPlayer.StartMusic(MusicType.WinSound);

        // foreach (Transform child in GameObject.Find("Produce Spawn Holder").transform)
        foreach (var child in GameObject.FindGameObjectsWithTag("Item"))
        {
            Destroy(child.gameObject);
        }
    }

    private void SpawnNextItem(bool groundHit)
    {
        if (!groundHit) GameSettings.instance.CorrectItemCounter++;

        GameTimer.updateScore(GameSettings.instance.CorrectItemCounter);

        // if (correctItemCounter >= winAmount)
        // {
        //     Debug.Log("You win!");
        //     gameInProgress = false;
        //     didWinGame = true;
        //     ConveyorBeltManager.onSpawnWinningEnvelope?.Invoke();
        // }
    }
}