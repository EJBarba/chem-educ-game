using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ArcheryManager : MonoBehaviour
{
    
    [SerializeField] GameObject resultModal;
    [SerializeField] TMP_Text itemText;
    [SerializeField] TMP_Text resultText;
    [SerializeField] GameObject laser;
    [SerializeField] Score scoreSO;
    [SerializeField] ScoreData scoreListSO;
    [SerializeField] GameObject world;
    [SerializeField] GameObject safeArea;
    private Laser laserScript;
    public bool hasDestroyed = false;
    public int playerChance = 1;
    public bool setToZero = false;
    public float waitSeconds = 3f;
    public bool endGame = false;
    public int score = 0;
    private AudioManager audioManager;
    public int scoreValueLaser = 5;
    public bool isLevel2 = false;
    public int level2Targets = 999;

    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (safeArea == null)
        {
            safeArea = GameObject.Find("SafeArea");
        }
    }

    private void Start() {
        laserScript = laser.GetComponent<Laser>();
        score = 0;
        scoreSO.updateScore(score);
    }
    void Update()
    {
        if(hasDestroyed == true && playerChance <= 0 || hasDestroyed == true && level2Targets <= 0)
        {
            endGame = true;
        }
        if (endGame)
        {
            hasDestroyed = false;
            endGame = false;
            StartCoroutine(ShowResultsModal());
        }
    }

    IEnumerator ShowResultsModal()
    {
        laserScript.enabled = false;
        audioManager.Stop("laserSFX");
        yield return new WaitForSeconds(waitSeconds);

        resultModal.SetActive(true);
        laser.SetActive(false);
        world.SetActive(false);
        
        safeArea.SetActive(false);
        
        itemText.text = "ITEM #" + scoreSO.name + " OUT OF " + scoreListSO.scoreList.Count;
        if(setToZero)
        {
            scoreSO.updateScore(0);
            resultText.text = "YOU LOSE";
            resultText.color = Color.red;
            audioManager.Play("bgmusicdefeat");
        }
        else
        {
            scoreSO.updateScore(scoreValueLaser);
            resultText.text = "YOU WIN!";
            resultText.color = Color.green;
            audioManager.Play("bgmusicvictory");
        } 
    }
           
}
