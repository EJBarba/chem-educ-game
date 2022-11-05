using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Score", menuName = "Score")]
public class Score : ScriptableObject
{
    public int scoreValue;
    public void updateScore(int score)
    {
        EditorUtility.SetDirty(this);
        scoreValue = score;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
