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
    public string description;
    private int tileIndex = 0;
    TouchScreenKeyboard keyboard;
    private void Start() {
        transform.localPosition = new Vector3(0, 0, 0);
   }
   public string GetCorrectAnswer()
   {
        return correctAnswer;
   }

    public GameObject[] GetTileGameObjects()
   {
        return answer;
   }

    public void DeleteTile()
    {
        if (tileIndex != 0)
        {
            if(answer[tileIndex - 1].GetComponent<TMP_InputField>().readOnly == false)
            {
            answer[tileIndex - 1].GetComponent<TMP_InputField>().text = "";
            } 
            else
            {
                answer[tileIndex - 2].GetComponent<TMP_InputField>().text = "";
            }
        }        
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
            //Debug.Log("Correct!");
            if (Input.anyKeyDown)
            {
                FindObjectOfType<AudioManager>().Play("wordcorrect");               
            }
            isSolved = true;
        }
        else
        {
            //Debug.Log("Wrong!");
            if (Input.anyKeyDown)
            {
                FindObjectOfType<AudioManager>().Play("wordwrong");               
            }
            isSolved = false;
        }
   }


   public void GetFirstEmptyTile(bool showKeyboard)
   {
        //if word is cleared after wrong answer, force active input field on first tile
        if (answer[0].GetComponent<TMP_InputField>().text == "")
        {
            //focus player to type in this tile
            // answer[0].GetComponent<TMP_InputField>().interactable = true;
            // answer[0].GetComponent<TMP_InputField>().ActivateInputField();
            // answer[0].GetComponent<TMP_InputField>().shouldHideSoftKeyboard = false;

            if (answer[0].GetComponent<TMP_InputField>().isFocused == false && answer[0].GetComponent<TMP_InputField>().interactable == true)
            {
                //answer[0].GetComponent<TMP_InputField>().shouldHideSoftKeyboard = true;
                answer[0].GetComponent<TMP_InputField>().interactable = false;
                answer[0].GetComponent<TMP_InputField>().DeactivateInputField();
                GameObject.Find("CrossWordCanvass").GetComponent<WordCheck>().showKeyboard = false;
            }
            else if (showKeyboard == true)
            {
                answer[0].GetComponent<TMP_InputField>().interactable = true;
                answer[0].GetComponent<TMP_InputField>().ActivateInputField();
                //answer[0].GetComponent<TMP_InputField>().shouldHideSoftKeyboard = false;
            }
           

            //force uppercase
            answer[0].GetComponent<TMP_InputField>().onValidateInput += delegate (string s, int i, char c) { return char.ToUpper(c); };
        }
        else
        {
            //Debug.Log(answer.Length);
            for (int i = 0; i < answer.Length; i++)
            {
                //activate input field on first empty tile
                //Debug.Log(answer[i].name);
                if (answer[i].GetComponent<TMP_InputField>().text == "")
                {
                    if (Input.anyKeyDown)
                    {
                        FindObjectOfType<AudioManager>().Play("tilesfx");
                    }
                    tileIndex = i;
                    //focus player to type in this tile

                    
                if (answer[i].GetComponent<TMP_InputField>().isFocused == false && answer[i].GetComponent<TMP_InputField>().interactable == true)
                {
                    //answer[i].GetComponent<TMP_InputField>().shouldHideSoftKeyboard = true;
                    answer[i].GetComponent<TMP_InputField>().interactable = false;
                    answer[i].GetComponent<TMP_InputField>().DeactivateInputField();
                    GameObject.Find("CrossWordCanvass").GetComponent<WordCheck>().showKeyboard = false;
                }
                else if (showKeyboard == true)
                {
                    answer[i].GetComponent<TMP_InputField>().interactable = true;
                    answer[i].GetComponent<TMP_InputField>().ActivateInputField();
                    //answer[i].GetComponent<TMP_InputField>().shouldHideSoftKeyboard = false;
                }
                    //force uppercase
                    answer[i].GetComponent<TMP_InputField>().onValidateInput += delegate (string s, int i, char c) { return char.ToUpper(c); };
                    break;
                }
            }
        }
   }

   
}