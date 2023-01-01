using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3TouchControls : MonoBehaviour
{
    [SerializeField] GameObject player;
    public void MovePlayerUp()
    {
        player.transform.position += new Vector3(0f, 2f, 0f);   
    }
    public void MovePlayerDown()
    {
        player.transform.position += new Vector3(0f,-2f, 0f);   
    }
    
}
