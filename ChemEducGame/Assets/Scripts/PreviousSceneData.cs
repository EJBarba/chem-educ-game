using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousSceneData : MonoBehaviour
{
    public static PreviousSceneData instance;
    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);    
    }
    public string previousScene;
}
