using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreKeeper : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    int score = 0;
    int highScore = 0;

    private void Start() {
        highScore = PlayerPrefs.GetInt("highscore", 0);
        scoreText.text ="SCORE: " + score.ToString();
        highScoreText.text ="HIGH SCORE: " + highScore.ToString();
    }

    public void RecordScore(int newScore)
    {
        scoreText.text = "SCORE: " + newScore.ToString();
        if (newScore > highScore)
        {
            PlayerPrefs.SetInt("highscore", newScore);
            highScoreText.text = "HIGH SCORE: " + newScore.ToString();
        }
        else
        {
            highScoreText.text = "HIGH SCORE: " + highScore.ToString();
        }
        
        
        
    }    
}
