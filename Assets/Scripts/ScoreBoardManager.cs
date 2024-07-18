using TMPro;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    [SerializeField] private TMP_Text joyScoreText;
    [SerializeField] private TMP_Text sadnessScoreText;

    private int _joyScore;
    private int _sadnessScore;

    private void OnEnable()
    {
        TrashBase.JoyScoreIncreased += OnHandleJoyScoreIncreased;
        TrashBase.SadnessScoreIncreased += OnHandleSadnessScoreIncreased;
    }

    private void OnDisable()
    {
        TrashBase.SadnessScoreIncreased -= OnHandleSadnessScoreIncreased;
    }

    private void OnHandleJoyScoreIncreased(int newScore)
    {
        Debug.Log("Score Joy has been updated to: " + newScore);

        _joyScore += newScore;
        joyScoreText.text = _joyScore.ToString();
    }

    private void OnHandleSadnessScoreIncreased(int newScore)
    {
        Debug.Log("Score Sadness has been updated to: " + newScore);

        _sadnessScore += newScore;
        sadnessScoreText.text = _sadnessScore.ToString();
    }

    private void Start()
    {
        joyScoreText.text = "0";
        sadnessScoreText.text = "0";
    }
}