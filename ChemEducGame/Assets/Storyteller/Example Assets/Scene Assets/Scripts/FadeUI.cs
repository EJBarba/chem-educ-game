using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeUI : MonoBehaviour
{

    public float FadeTime = 0.7f;
    private CanvasGroup canvasGroup;
    private bool fadeInState;
    private bool fadeOutState;


    void  Update()
    {
        if(fadeInState)
        {
            if (canvasGroup.alpha != 1)
                canvasGroup.alpha += 0.01f;
        }

        if(fadeOutState)
        {

        }

    }

    public void FadeIn(bool state)
    {
      //  FadeTime = fadeTime;
        fadeInState = state;
       canvasGroup = GetComponent<CanvasGroup>();
       
    }
    public void FadeOut(bool state)
    {
      //  FadeTime = fadeTime;
        fadeOutState = state;
        canvasGroup = GetComponent<CanvasGroup>();
    }
}
