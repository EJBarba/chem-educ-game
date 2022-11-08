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
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
        scoreValue = score;

        #if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh()
        #endif
        ;
    }
}
