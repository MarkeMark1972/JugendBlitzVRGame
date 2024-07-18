using System;
using System.Collections;
using System.Collections.Generic;
using JugendBlitz.scripts.runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InstructionsImageSlider : MonoBehaviour
{
    [SerializeField] private List<Sprite> images;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button rightButtonFromCongrats;
    [SerializeField] private Button startButton;
    [FormerlySerializedAs("_image")] [SerializeField] private Image image;

    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject congratulationsPanel;

    [SerializeField] private TMP_Text scoreText;
    
    private Animation _mainMenuAnimations;

    private int _currentId;
    private bool _gamePlayed;

    public static Action ShowStartMenuClicked;

    private void Awake()
    {
        // this.gameObject.SetActive(false);
        _mainMenuAnimations = GetComponent<Animation>();
        
        ConveyorBeltManager.OnEndGame += EndGame;
        ConveyorBeltManager.OnStartGame += StartGame;

        UIPanelManager.showLeftUI += ShowLeftUI;

        welcomePanel.SetActive(true);
        congratulationsPanel.SetActive(false);
        rightButtonFromCongrats.enabled = false;
    }

    private void OnEnable()
    {
        ShowStartMenuClicked += ShowMenuStartGameAnimation;
    }

    private void OnDisable()
    {
        ShowStartMenuClicked -= ShowMenuStartGameAnimation;
    }
    
    private void ShowLeftUI()
    {
        scoreText.text = GameSettings.instance.CorrectItemCounter.ToString();

        welcomePanel.SetActive(false);
        congratulationsPanel.SetActive(true);
    }

    private void Start()
    {
        leftButton.onClick.AddListener(LeftButtonClicked);
        rightButton.onClick.AddListener(RightButtonClicked);
        rightButtonFromCongrats.onClick.AddListener(RightButtonFromCongratsClicked);
        UpdateImage();
        startButton.gameObject.SetActive(false);
        UpdateVisibility();

        scoreText.text = "0";

        StartCoroutine(ShowMenuAnimation());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) StartCoroutine(ShowMenuAnimation());
        if (Input.GetKeyDown(KeyCode.P)) StartCoroutine(HideMenuAnimation());
    }

    private IEnumerator ShowMenuAnimation()
    {
        _mainMenuAnimations.Play("Main Menu Show Animation");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ShowMenuEndGameAnimation()
    {
        _mainMenuAnimations.Play("Main Menu Show End Game Animation");
        yield return new WaitForSeconds(1f);
    }
    
    private void ShowMenuStartGameAnimation()
    {
        _currentId = 0;
        UpdateImage();
        startButton.gameObject.SetActive(true);
        UpdateVisibility();
        welcomePanel.SetActive(true);
        congratulationsPanel.SetActive(false);
        _mainMenuAnimations.Play("Main Menu Show Animation");
    }

    private IEnumerator HideMenuAnimation()
    {
        _mainMenuAnimations.Play("Main Menu Hide Animation");
        yield return new WaitForSeconds(1f);
        _currentId = 0;
        UpdateImage();
        startButton.gameObject.SetActive(false);
        UpdateVisibility();
    }

    private void OnDestroy()
    {
        leftButton.onClick.RemoveListener(LeftButtonClicked);
        rightButton.onClick.RemoveListener(RightButtonClicked);
        rightButtonFromCongrats.onClick.RemoveListener(RightButtonFromCongratsClicked);

        ConveyorBeltManager.OnEndGame -= EndGame;
        ConveyorBeltManager.OnStartGame -= StartGame;
        UIPanelManager.showLeftUI -= ShowLeftUI;
    }

    private void StartGame()
    {
        // StartCoroutine(_gamePlayed ? ShowMenuEndGameAnimation() : HideMenuAnimation());
        StartCoroutine(HideMenuAnimation());
    }

    private void EndGame()
    {
        // StartCoroutine(GameManager.didWinGame ? ShowMenuEndGameAnimation() : ShowMenuAnimation());
        StartCoroutine(ShowMenuEndGameAnimation());

        _gamePlayed = true;
        _currentId = 0;
        UpdateImage();
        startButton.gameObject.SetActive(false);
        UpdateVisibility();

        scoreText.text = GameSettings.instance.CorrectItemCounter.ToString();
        welcomePanel.SetActive(false);
        congratulationsPanel.SetActive(true);

        // StartCoroutine(ShowMenuStartGameAnimation());
    }

    private void LeftButtonClicked()
    {
        UpdateCurrentId(-1);
        UpdateImage();
    }

    private void RightButtonClicked()
    {
        UpdateCurrentId(1);
        UpdateImage();
    }

    private void RightButtonFromCongratsClicked()
    {
        UpdateCurrentId(0);
        UpdateImage();
        welcomePanel.SetActive(true);
        congratulationsPanel.SetActive(false);
    }

    private void UpdateCurrentId(int direction)
    {
        _currentId = (_currentId + direction) % images.Count;
        if (_currentId < 0) _currentId = images.Count - 1;

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        startButton.gameObject.SetActive(_currentId == images.Count - 1);
        leftButton.interactable = _currentId > 0;
        rightButton.interactable = _currentId < images.Count - 1;
    }

    private void UpdateImage() => image.sprite = images[_currentId];
}