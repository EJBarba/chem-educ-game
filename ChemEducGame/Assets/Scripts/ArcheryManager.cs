using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArcheryManager : MonoBehaviour
{
    
    [SerializeField] GameObject resultModal;
    [SerializeField] TMP_Text resultText;
    public bool birdCollided = false;
    public bool isTargetCollided = false;
    private bool isHitOnce = true;
    void Update()
    {
        if(birdCollided == true && isHitOnce == true)
        {
            resultModal.SetActive(true);

            if (isTargetCollided == true)
            {
                resultText.text = "YOU WIN"; 
            }
            else
            {
                resultText.text = "YOU LOSE";
            }
            isHitOnce = false;
            return;
        }
    }
}
