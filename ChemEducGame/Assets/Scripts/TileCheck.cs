using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileCheck : MonoBehaviour
{
    [SerializeField] string correctAnswer;
    [SerializeField] GameObject[] letters;
    private TMP_InputField[] answer;
    private  string playerAnswer;
    public bool isSolved = false;
   
   public string GetCorrectAnswer()
   {
        return correctAnswer;
   }

   public string GetPlayerAnswer()
   {
        answer = GetComponentsInChildren<TMP_InputField>();
        playerAnswer = "";

        for (int i = 0; i < answer.Length; i++)
        {
            playerAnswer += answer[i].text;
            //Debug.Log(playerAnswer + ": " + playerAnswer.Length);
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
}
