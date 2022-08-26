using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class WordCheck : MonoBehaviour
{  
    [SerializeField] GameObject[] word;
    private Image[] currentTileSprites;
    [SerializeField] float clearDelay = 1f;
    private float timerSeconds = 1f;
    private bool timerReached = false;
    private TMP_InputField[] answer;
    private Animator[] animator;

    void Awake()
    {
      for (int i = 0; i < word.Length; i++)
      {
        if (word[i].tag == "Player")
        {
          currentTileSprites = word[i].GetComponentsInChildren<Image>();
        }
       }
    }

    void Update()
    {
        for (int i = 0; i < word.Length; i++)
      {
        if (word[i].tag == "Player" && currentTileSprites.Length != 0)
        {
          string playerAnswer = (word[i].GetComponent<TileCheck>().GetPlayerAnswer());
          string correctAnswer = word[i].GetComponent<TileCheck>().GetCorrectAnswer();

          if (playerAnswer.Length < correctAnswer.Length)
          {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAA");
            word[i].GetComponent<TileCheck>().GetFirstEmptyTile();
            timerReached = false;
            timerSeconds = clearDelay;
          }
          
          if (playerAnswer.Length == correctAnswer.Length)
          {
            Debug.Log("equal length");
            word[i].GetComponent<TileCheck>().CheckAnswer();
            animator = word[i].GetComponentsInChildren<Animator>();
            answer = word[i].GetComponentsInChildren<TMP_InputField>();

            // if correct
            if (word[i].GetComponent<TileCheck>().isSolved == true)
            {
              for (int j = 0; j < animator.Length; j++)
              {
                //change into green tile
                animator[j].SetBool("isSolved", true);
                animator[j].SetBool("hasAnswered", true);

                //make tile read only
                answer[j].readOnly = true;
                
              }
            }

            // if wrong
            else
            {
              // change into red tile
              for (int j = 0; j < animator.Length; j++)
              {
                animator[j].SetBool("hasAnswered", true);
              }
             
              //wait timer
              if (!timerReached)
              {
                timerSeconds -= Time.deltaTime;
                Debug.Log(timerSeconds);
              }
               if (!timerReached && timerSeconds < 0)
              {
                // change into yellow tile
              for (int j = 0; j < animator.Length; j++)
              {
                answer[j].text = "";
                animator[j].SetBool("hasAnswered", false);
              }
                timerReached = true;
              }
              
              
            }

          }
        }
      } 
    }
}
