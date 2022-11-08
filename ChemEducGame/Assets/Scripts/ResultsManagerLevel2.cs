using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ResultsManagerLevel2 : MonoBehaviour
{
    AudioManager audioManager;
    [SerializeField] ScoreData level1TotalScore;
    [SerializeField] ScoreData level2TotalScore;
    [SerializeField] TextMeshProUGUI level1TotalScoreUI;
    [SerializeField] TextMeshProUGUI level2TotalScoreUI;
    [SerializeField] TextMeshProUGUI totalScoreUIText;

    void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    void Start()
    {
        audioManager.StopAllBGMusic();
        audioManager.Play("missionCompleteMusic");

        int totalScore = 0;
        int level1Score = 0;
        int level2Score = 0;

        for (int item = 0; item < level1TotalScore.scoreList.Count; item++)
        {
            level1Score += level1TotalScore.scoreList[item].scoreValue;
        }

        for (int item = 0; item < level2TotalScore.scoreList.Count; item++)
        {
            level2Score += level2TotalScore.scoreList[item].scoreValue;
        }

        totalScore = level1Score + level2Score;

        level1TotalScoreUI.text = level1Score.ToString();
        level2TotalScoreUI.text = level2Score.ToString();
        totalScoreUIText.text = totalScore.ToString();
    }
}
