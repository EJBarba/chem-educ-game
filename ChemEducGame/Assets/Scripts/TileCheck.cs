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
    [SerializeField] float timeToClear = 1f;
   
   public string GetCorrectAnswer()
   {
        return correctAnswer;
   }

   public string GetPlayerAnswer()
   {
        playerAnswer = "";

        for (int i = 0; i < answer.Length; i++)
        {
            playerAnswer += answer[i].GetComponent<TMP_InputField>().text;
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

            for (int i = 0; i < answer.Length; i++)
            {
                answer[i].GetComponent<TileSolveToggle>().TileSolved();
                Debug.Log(answer[i].GetComponent<TileSolveToggle>().TileState());
            }
        }
        else
        {
            Debug.Log("Wrong!");
            isSolved = false;
            StartCoroutine(WordClear()); 
        }
   }

   IEnumerator WordClear()
    {
      yield return new WaitForSeconds(timeToClear);
      
      for (int i = 0; i < answer.Length; i++)
            {
                Debug.Log(answer[i].GetComponent<TileSolveToggle>().TileState());
                if (answer[i].GetComponent<TileSolveToggle>().TileState() == false)
                {
                    answer[i].GetComponent<TMP_InputField>().text = "";
                }
            }
    }
}
