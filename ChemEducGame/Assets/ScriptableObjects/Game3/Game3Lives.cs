using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class Game3Lives : ScriptableObject
{
    public int lives;
    [SerializeField] int maxLives = 3;
    public void decreaseLife()
    {
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
        lives = lives - 1;

        #if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh()
        #endif
        ;
    }
    public void resetLives()
    {
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
        lives = maxLives;

        #if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh()
        #endif
        ;
    }
}
