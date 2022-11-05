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
    [SerializeField] GameObject laser;
    private Laser laserScript;
    public bool isTargetHit = false;
    public bool hasDestroyed = false;
    public int playerChance = 1;
    public float waitSeconds = 3f;
    private bool endGame = false;

    private void Start() {
        laserScript = laser.GetComponent<Laser>();
    }
    void Update()
    {
        if(hasDestroyed == true && playerChance <= 0)
        {
            endGame = true;
        }
        if (endGame)
        {
            hasDestroyed = false;
            playerChance = 1;
            endGame = false;
            StartCoroutine(ShowResultsModal());
        }
    }

    IEnumerator ShowResultsModal()
    {
        laserScript.enabled = false;
        yield return new WaitForSeconds(waitSeconds);
        resultModal.SetActive(true);
        nextLevelButton.SetActive(true);
        laser.SetActive(false);
        if (isTargetHit == true)
        {
            resultText.text = "YOU WIN";     
        }
        else
        {
            resultText.text = "YOU LOSE";
        }
    }
           
}
