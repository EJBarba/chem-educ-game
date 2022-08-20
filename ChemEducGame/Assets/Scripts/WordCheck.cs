using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WordCheck : MonoBehaviour
{
    private TextMeshProUGUI[] tileList;
    [SerializeField] GameObject[] word;
    [SerializeField] TextMeshProUGUI example;

    void Awake()
    {
      for (int i = 0; i < word.Length; i++)
      {
        if (word[i].tag == "Player")
        {
          // Debug.Log("player");
          // Debug.Log(word[i].GetComponent<TileCheck>().GetCorrectAnswer());
          // Debug.Log(word[i].GetComponent<TileCheck>().GetCorrectAnswer().Length);
          // Debug.Log(word[i].GetComponent<TileCheck>().GetPlayerAnswer());
          // Debug.Log(word[i].GetComponent<TileCheck>().GetPlayerAnswer().Length);
          // word[i].GetComponent<TileCheck>().CheckAnswer();
          // Debug.Log(word[i].GetComponent<TileCheck>().isSolved);
        }
       }
    }
    void Update()
    {
        for (int i = 0; i < word.Length; i++)
      {
        if (word[i].tag == "Player")
        {
          // Debug.Log("player");
          // Debug.Log(word[i].GetComponent<TileCheck>().GetCorrectAnswer());
          
          // Debug.Log(word[i].GetComponent<TileCheck>().GetPlayerAnswer());
          
          word[i].GetComponent<TileCheck>().CheckAnswer();
          Debug.Log(word[i].GetComponent<TileCheck>().isSolved);
        }
       } 
    }
}
