using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class WordCheck : MonoBehaviour
{  
    [SerializeField] List<GameObject> word = new List<GameObject>();
    [SerializeField] float clearDelay = 1f;
    private float timerSeconds = 1f;
    private bool timerReached = false;
    private GameObject[] answer;
    private int currentIndex = 0;
    [SerializeField] TMP_Text description;
    [SerializeField] GameObject winModal;
    [SerializeField] GameObject timer;
    [SerializeField] GameObject pauseButton;
    private bool isPlayed = false;
    private int winTime = 0;
    private bool playerWin = false;
    [HideInInspector]
    public PlayFabManager playFabManager;
    [HideInInspector]
    public ScoreKeeper scoreKeeper;
    AudioManager audioManager;
    void Awake()
    {
      playFabManager = GameObject.Find("PLAYER").GetComponent<PlayFabManager>();
      scoreKeeper = GameObject.Find("PLAYER").GetComponent<ScoreKeeper>();
      audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

      for (int i = 0; i < word.Count; i++)
      {
        if (word[i].tag == "Player")
        {
          answer = word[i].GetComponent<TileCheck>().GetTileGameObjects();
          description.text = word[i].GetComponent<TileCheck>().description;
          break;
        }
       }
    }

    void Start() 
    {
      audioManager.Stop("bgmusicmainmenu");
      audioManager.Play("bgmusic1");
    }

    void Update()
    {
      for (int i = 0; i < word.Count; i++)
      { 
        if (word[i].tag == "Player" && answer.Length != 0 && playerWin != true)
        {
          answer = word[i].GetComponent<TileCheck>().GetTileGameObjects();
          description.text = word[i].GetComponent<TileCheck>().description;

          string playerAnswer = (word[i].GetComponent<TileCheck>().GetPlayerAnswer());
          string correctAnswer = word[i].GetComponent<TileCheck>().GetCorrectAnswer();

          if (playerAnswer.Length < correctAnswer.Length)
          {
            word[i].GetComponent<TileCheck>().GetFirstEmptyTile();
            timerReached = false;
            timerSeconds = clearDelay;
            
            // go to previous tile and delete. Ignore if first tile // && playerAnswer.Length > 0
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
              word[i].GetComponent<TileCheck>().DeleteTile();
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
            Debug.Log(word[i].GetComponent<TileCheck>().GetPlayerAnswer());
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
                if(isPlayed == false)
                {
                  audioManager.Stop("bgmusic1");
                  audioManager.Play("bgmusicvictory");
                  isPlayed = true;
                }
                
                winTime = (int)timer.GetComponent<Timer>().timeRemaining;
                
                //display score in winModal, record if highscore 
                scoreKeeper.RecordScore(winTime);
                //online leaderboards
                playFabManager.SendLeaderBoard(winTime);

                winModal.SetActive(true);
                timer.SetActive(false);
                pauseButton.SetActive(false);
                description.text = "";
                playerWin = true;
              }

              // go to next word
              else if (currentIndex == 0)
              {                  
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

                  word[i].tag = "Untagged";
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
          break;
        }
      } 
    }
}
