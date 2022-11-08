using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultsManager : MonoBehaviour
{
    AudioManager audioManager;
    [SerializeField] List<Score> scoreList;
    [SerializeField] List<TextMeshProUGUI> levelList;
    [SerializeField] TextMeshProUGUI totalScoreUIText;

    private int totalScore = 0;

    void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    void Start()
    {
        audioManager.StopAllBGMusic();
        audioManager.Play("missionCompleteMusic");

        for (int item = 0; item < scoreList.Count; item++)
        {
            levelList[item].text = scoreList[item].scoreValue.ToString();
            totalScore += scoreList[item].scoreValue;
        }

        totalScoreUIText.text = totalScore.ToString();
    }
}
