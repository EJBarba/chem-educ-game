using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class WordCheck : MonoBehaviour
{  
    // [SerializeField] GameObject[] word;
    [SerializeField] List<GameObject> word = new List<GameObject>();
    [SerializeField] float clearDelay = 1f;
    private float timerSeconds = 1f;
    private bool timerReached = false;
    private GameObject[] answer;
    private int currentIndex = 0;
    [SerializeField] TMP_Text description;

    void Awake()
    {
      //scoreKeeper = FindObjectOfType<ScoreKeeper>();
      for (int i = 0; i < word.Count; i++)
      {
        if (word[i].tag == "Player")
        {
          answer = word[i].GetComponent<TileCheck>().GetTileGameObjects();
          description.text = word[i].GetComponent<TileCheck>().description;
        }
       }
    }

    void Update()
    {
      for (int i = 0; i < word.Count; i++)
      {
        answer = word[i].GetComponent<TileCheck>().GetTileGameObjects();
        
        
        if (word[i].tag == "Player" && answer.Length != 0)
        {
          description.text = word[i].GetComponent<TileCheck>().description;
          //check if player wins
          //Debug.Log("SCORE: " + scoreKeeper.GetCorrectAnswers());
          //Debug.Log("WORD LENGTH: " + word.Count);

          string playerAnswer = (word[i].GetComponent<TileCheck>().GetPlayerAnswer());
          string correctAnswer = word[i].GetComponent<TileCheck>().GetCorrectAnswer();

          if (playerAnswer.Length < correctAnswer.Length)
          {
            word[i].GetComponent<TileCheck>().GetFirstEmptyTile();
            timerReached = false;
            timerSeconds = clearDelay;
            

            // go to previous tile and delete. Ignore if first tile
            if (Input.GetKeyDown(KeyCode.Backspace) && playerAnswer.Length > 0)
            {
              if (answer[playerAnswer.Length - 1].GetComponent<TMP_InputField>().readOnly == false && answer[playerAnswer.Length - 1].GetComponent<TMP_InputField>().text.Length > 0)
              {
                answer[playerAnswer.Length - 1].GetComponent<TMP_InputField>().text = "";
              }
              else
              {
                answer[playerAnswer.Length - 2].GetComponent<TMP_InputField>().text = "";
              }
            }

            // if player presses Left Arrow, go to previous word
            // check if word.Length > 0
            if (Input.GetKeyDown(KeyCode.LeftArrow) && i > 0)
            {
              // assign new tile as player
              word[i - 1].tag = "Player";

              //remove current tile as player
              word[i].tag = "Untagged";
            }

            // if player presses Right Arrow, go to next word
            // check if key is released to reset to prevent "infinite loop" 
            
            if (Input.GetKeyDown(KeyCode.RightArrow) && i < word.Count - 1 && currentIndex == 0)
            {
              currentIndex = i + 1;
              word[currentIndex].tag = "Player";

              //remove current tile as player
              word[i].tag = "Untagged";
            }
            
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
              currentIndex = 0;
            }
          }
          
          if (playerAnswer.Length == correctAnswer.Length)
          {
            Debug.Log("equal length");
            word[i].GetComponent<TileCheck>().CheckAnswer();

            // if correct
            if (word[i].GetComponent<TileCheck>().isSolved == true)
            {
              for (int j = 0; j < answer.Length; j++)
              {
                //change into green tile
                answer[j].GetComponent<Animator>().SetBool("isSolved", true);
                answer[j].GetComponent<Animator>().SetBool("hasAnswered", true);

                //make tile read only
                answer[j].GetComponent<TMP_InputField>().readOnly = true;
                // toggle tile to be solved, to be skipped if tile is shared
                answer[j].GetComponent<TileSolveToggle>().TileSolved(true);

              }

              // check if very last word, this means player wins
              if (word.Count == 1)
              {
                Debug.Log("WIN!");
              }

              // go to next word
              else if (currentIndex == 0)
              {
                  //increment 1 to ScoreKeeper
                  //scoreKeeper.IncrementCorrectAnswers();
                  
                  currentIndex = i + 1;

                  if (currentIndex < word.Count)
                  {
                    word[currentIndex].tag = "Player";
                  }
                  else
                  {
                    word[0].tag = "Player";
                  }
                    
                  //remove current word as player
                  //word[i].tag = "Untagged";

                  word.RemoveAt(i);
                  currentIndex = 0;
              }
              
            }

            // if wrong
            else
            {
              
              // change into red tile
              for (int j = 0; j < answer.Length; j++)
              {
                // only change tiles if not solved / not green 
                if (answer[j].GetComponent<TileSolveToggle>().TileState() == false)
                {
                  answer[j].GetComponent<Animator>().SetBool("hasAnswered", true);
                  answer[j].GetComponent<TMP_InputField>().readOnly = true;
                }
                
              }
             
              //wait timer
              if (!timerReached)
              {
                timerSeconds -= Time.deltaTime;
                //Debug.Log(timerSeconds);
              }
               if (!timerReached && timerSeconds < 0)
              {
                // change into yellow tile
              
              for (int j = 0; j < answer.Length; j++)
              {
                //Debug.Log("cleared letter: " + answer[j].GetComponent<TMP_InputField>().text);
                // only change tiles if not solved / not green
                if (answer[j].GetComponent<TileSolveToggle>().TileState() == false)
                {
                  answer[j].GetComponent<TMP_InputField>().text = "";
                  answer[j].GetComponent<Animator>().SetBool("hasAnswered", false);
                  answer[j].GetComponent<TMP_InputField>().readOnly = false;
                }
                
              }
                timerReached = true;
              }
             
            }

          }
        }
      } 
    }
}
