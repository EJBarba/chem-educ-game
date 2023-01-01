using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3TouchControls : MonoBehaviour
{
    [SerializeField] GameObject player;
    public void MovePlayerUp()
    {
        if (player.transform.position.y < 0.75)
        {
            player.transform.position += new Vector3(0f, 2f, 0f); 
        }
    }
    public void MovePlayerDown()
    {
        if (player.transform.position.y > -3.25)
        {
           player.transform.position += new Vector3(0f,-2f, 0f);  
        }
    }
}
