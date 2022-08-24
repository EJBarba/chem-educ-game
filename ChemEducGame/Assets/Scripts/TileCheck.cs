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
    private TileSolveToggle[] tileIsSolved;
    [SerializeField] float timeToClear = 1f;
   
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
        tileIsSolved = GetComponentsInChildren<TileSolveToggle>();

        if (playerAnswer == correctAnswer)
        {
            Debug.Log("Correct!");
            isSolved = true;

            for (int i = 0; i < tileIsSolved.Length; i++)
            {
                tileIsSolved[i].TileSolved();
                Debug.Log(tileIsSolved[i].TileState());
            }
        }
        else
        {
            Debug.Log("Wrong!");
            isSolved = false;

            answer = GetComponentsInChildren<TMP_InputField>();
            StartCoroutine(WordClear());
             
        }
   }

   IEnumerator WordClear()
    {
      yield return new WaitForSeconds(timeToClear);
      
      tileIsSolved = GetComponentsInChildren<TileSolveToggle>();
      answer = GetComponentsInChildren<TMP_InputField>();
      for (int i = 0; i < answer.Length; i++)
            {
                if (tileIsSolved[i].TileState() == false)
                {
                    // clear text
                    answer[i].text = "";
                }
            }
    }
}
