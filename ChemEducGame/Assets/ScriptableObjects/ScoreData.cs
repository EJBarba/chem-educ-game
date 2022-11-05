using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "ScoreData", menuName = "ScoreData")]
public class ScoreData : ScriptableObject
{
    public List<Score> scoreList;
    public int getTotalScores()
    {
        int totalScore = 0;
        foreach (var item in scoreList)
        {
            totalScore += item.scoreValue;
        }
        return totalScore;
    }
}
