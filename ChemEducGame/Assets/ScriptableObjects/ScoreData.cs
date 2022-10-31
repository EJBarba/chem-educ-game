using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "ScoreData", menuName = "ScoreData")]
public class ScoreData : ScriptableObject
{
    public List<int> scoreList;

    public void updateCrosswordScores(int index, int score)
    {
        EditorUtility.SetDirty(this);
        scoreList[index] = score;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
