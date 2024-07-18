using System;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private float gameDuration = 120f;
    public TMP_Text countdownText;
    public TMP_Text scoreText;
    public float timeLeft;
    private bool _gameStarted;

    [SerializeField] private AudioSource celebrationSound;
    
    
    public static Action<int> updateScore;

    private void Awake()
    {
        timeLeft = gameDuration;
        UpdateReadout();
    }

    private void OnEnable()
    {
        updateScore += UpdateScore;
        ConveyorBeltManager.OnStartGame += StartGame;
        ConveyorBeltManager.OnEndGame += EndGame;
        
        UpdateScore(0);
    }

    private void UpdateScore(int score)
    {
        scoreText.text = $"{score}";
    }

    private void OnDisable()
    {
        updateScore -= UpdateScore;
        ConveyorBeltManager.OnStartGame -= StartGame;
        ConveyorBeltManager.OnEndGame -= EndGame;
    }
    
    private void OnDestroy()
    {
        ConveyorBeltManager.OnStartGame -= StartGame;
        ConveyorBeltManager.OnEndGame -= EndGame;
    }

    private void StartGame()
    {
        timeLeft = gameDuration;
        _gameStarted = true;
        UIPanelManager.hideLeftUI?.Invoke();

        UpdateScore(0);
    }

    private void EndGame()
    {
        _gameStarted = false;
    }

    private void Update()
    {
        if (!_gameStarted) return;

        timeLeft -= Time.deltaTime;
        UpdateReadout();

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            UpdateReadout();
            _gameStarted = false;
            ConveyorBeltManager.OnSpawnWinningEnvelope?.Invoke();
            UIPanelManager.showLeftUI?.Invoke();
            UIPanelManager.showRightUI?.Invoke();
            ConveyorBeltManager.OnEndGame?.Invoke();
            celebrationSound.Play();
        }
    }

    private void UpdateReadout()
    {
        var minutes = Mathf.FloorToInt(timeLeft / 60f);
        var seconds = Mathf.FloorToInt(timeLeft - minutes * 60);
        var formattedTime = $"{minutes:00}:{seconds:00}";

        countdownText.text = formattedTime;
    }
}