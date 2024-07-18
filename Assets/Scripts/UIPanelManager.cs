using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIPanelManager : MonoBehaviour
{
    // private GameObject _leftUIPanel;
    private GameObject _rightUIPanel;

    public static Action toggleHideLeftUI;
    public static Action toggleHideRightUI;

    public static Action showLeftUI;
    public static Action showRightUI;

    public static Action hideLeftUI;
    public static Action hideRightUI;

    [SerializeField] private Button startButton;

    [SerializeField] private AudioSource startButtonSound;
    

    private void Awake()
    {
        // _leftUIPanel = GameObject.Find("Loosing UI Panel");
        _rightUIPanel = GameObject.Find("Right UI Panel");

        toggleHideRightUI += ToggleRightPanelVisibility;
        toggleHideLeftUI += ToggleLeftPanelVisibility;

        showLeftUI += ShowLeftPanel;
        showRightUI += ShowRightPanel;

        hideLeftUI += HideLeftPanel;
        hideRightUI += HideRightPanel;
        
        startButton.onClick.AddListener(StartButtonClicked);

        HideLeftPanel();
        HideRightPanel();
    }

    private void StartButtonClicked()
    {
        // hideRightUI?.Invoke();
        ConveyorBeltManager.OnStartGame?.Invoke();
        startButtonSound.Play();
    }

    private void OnDestroy()
    {
        toggleHideRightUI -= ToggleRightPanelVisibility;
        toggleHideLeftUI -= ToggleLeftPanelVisibility;

        showLeftUI -= ShowLeftPanel;
        showRightUI -= ShowRightPanel;

        hideLeftUI -= HideLeftPanel;
        hideRightUI -= HideRightPanel;
        
        startButton.onClick.RemoveListener(StartButtonClicked);
    }

    private void ToggleLeftPanelVisibility()
    {
        // _leftUIPanel.SetActive(!_leftUIPanel.activeSelf);
    }

    private void ShowLeftPanel()
    {
        // _leftUIPanel.SetActive(true);
    }

    private void HideLeftPanel()
    {
        // _leftUIPanel.SetActive(false);
    }

    private void ToggleRightPanelVisibility() => _rightUIPanel.SetActive(!_rightUIPanel.activeSelf);

    private void ShowRightPanel() => _rightUIPanel.SetActive(true);

    private void HideRightPanel() => _rightUIPanel.SetActive(false);
}