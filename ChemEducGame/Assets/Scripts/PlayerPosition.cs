using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPosition : MonoBehaviour
{
    private GameObject currentGameObjectPlayer;
    private  GameObject[] playerTiles;
    
    void Awake() {
        FindPlayer();
        FocusTile();
    }
    
    void Update()
    {
        if (currentGameObjectPlayer != null)
        {
            FocusTile();
        }
        // else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     FindPlayer();
        //     FocusTile();
        // }
        // else
        // {
        //     FindPlayer();
        // }
        
    }

    void FindPlayer()
    {
        currentGameObjectPlayer = GameObject.FindWithTag("Player");
        playerTiles = currentGameObjectPlayer.GetComponent<TileCheck>().GetTileGameObjects();
    }

    void FocusTile()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            for (int i = 0; i < playerTiles.Length; i++)
            {
                if (playerTiles[i].GetComponent<TMP_InputField>().text == "")
                {
                    gameObject.transform.position =  playerTiles[i].transform.position;
                    break;
                }
            }
        }
    }
}
