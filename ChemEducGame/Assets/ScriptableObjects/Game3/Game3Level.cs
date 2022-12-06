using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class Game3Level : ScriptableObject
{
    public int currentLevel;
    public void nextLevel()
    {
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
        currentLevel = currentLevel + 1;

        #if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh()
        #endif
        ;
    }
    public void resetLevel()
    {
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
        currentLevel = 0;

        #if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh()
        #endif
        ;
    }
}
