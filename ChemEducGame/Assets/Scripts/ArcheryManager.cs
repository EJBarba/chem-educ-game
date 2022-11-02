using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ArcheryManager : MonoBehaviour
{
    
    [SerializeField] GameObject resultModal;
    [SerializeField] TMP_Text resultText;
    [SerializeField] GameObject nextLevelButton;
    public bool isTargetHit = false;
    public bool isLaserFired = false;
    void Update()
    {
        if(isLaserFired == true)
        {
            resultModal.SetActive(true);
            nextLevelButton.SetActive(true);
            if (isTargetHit == true)
            {
                resultText.text = "YOU WIN";     
            }
            else
            {
                resultText.text = "YOU LOSE";
            }
            
        }
        
        return;
    }
    
}
