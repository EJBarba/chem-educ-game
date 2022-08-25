using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class WordCheck : MonoBehaviour
{
    
    [SerializeField] GameObject[] word;
    private Image[] currentTileSprites;
     private Animator[] animator;

    void Awake()
    {
      for (int i = 0; i < word.Length; i++)
      {
        if (word[i].tag == "Player")
        {
          currentTileSprites = word[i].GetComponentsInChildren<Image>();
          animator = word[i].GetComponentsInChildren<Animator>();
        }
       }
    }

    void Start() 
    {
       for (int i = 0; i < word.Length; i++)
      {
        if (word[i].tag == "Player")
        {
          //activate input field on first empty tile
          word[i].GetComponent<TileCheck>().GetFirstEmptyTile();
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
          if (playerAnswer.Length == correctAnswer.Length)
            {
              Debug.Log("equal length");
              word[i].GetComponent<TileCheck>().CheckAnswer();
              
              if (word[i].GetComponent<TileCheck>().isSolved == true)
              {
                for (int j = 0; j < currentTileSprites.Length; j++)
                {
                  //currentTileSprites[j].sprite = correctTileSprite;
                  animator[j].SetBool("isSolved", true);
                  animator[j].SetBool("hasAnswered", true);
                }
              }
              else
              {
               for (int j = 0; j < currentTileSprites.Length; j++)
                {
                  //currentTileSprites[j].sprite = wrongTileSprite;
                  animator[j].SetBool("hasAnswered", true);
                }
              }     
            }
          else
          {
            //StartCoroutine(WordDelay(neutralTileSprite));
            for (int j = 0; j < currentTileSprites.Length; j++)
                {
                  //currentTileSprites[j].sprite = neutralTileSprite;
                  animator[j].SetBool("hasAnswered", false);
                }
          }
          // Debug.Log("player");
          // Debug.Log(word[i].GetComponent<TileCheck>().GetCorrectAnswer());
          
          // Debug.Log(word[i].GetComponent<TileCheck>().GetPlayerAnswer());
          
          //word[i].GetComponent<TileCheck>().CheckAnswer();
          //Debug.Log(word[i].GetComponent<TileCheck>().isSolved);
        }
      } 
    }

    // IEnumerator WordClear()
    // {
    //   yield return new WaitForSeconds(waitTime);
    //   for (int j = 0; j < currentTileSprites.Length; j++)
    //     {
          
    //     }
    // }
}
