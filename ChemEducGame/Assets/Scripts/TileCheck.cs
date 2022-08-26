using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileCheck : MonoBehaviour
{
    [SerializeField] string correctAnswer;
    [SerializeField] GameObject[] answer;
    private  string playerAnswer;
 
    public bool isSolved = false;
   
   public string GetCorrectAnswer()
   {
        return correctAnswer;
   }

    public GameObject[] GetTileGameObjects()
   {
        return answer;
   }

 

   public string GetPlayerAnswer()
   {
        playerAnswer = "";

        for (int i = 0; i < answer.Length; i++)
        {
            playerAnswer += answer[i].GetComponent<TMP_InputField>().text;
        }
        return playerAnswer;
   }

   public void CheckAnswer()
   {
        GetPlayerAnswer();
        GetCorrectAnswer();

        if (playerAnswer == correctAnswer)
        {
            Debug.Log("Correct!");
            isSolved = true;
        }
        else
        {
            Debug.Log("Wrong!");
            isSolved = false;
        }
   }

   public void GetFirstEmptyTile()
   {
        //if word is cleared after wrong answer, force active input field on first tile
        if (answer[0].GetComponent<TMP_InputField>().text == "")
        {
            answer[0].GetComponent<TMP_InputField>().ActivateInputField();
        }
        else
        {
            for (int i = 0; i < answer.Length; i++)
            {
                //activate input field on first empty tile
                if (answer[i].GetComponent<TMP_InputField>().text == "")
                {
                    answer[i].GetComponent<TMP_InputField>().ActivateInputField();
                }
            }
        }
   }
}