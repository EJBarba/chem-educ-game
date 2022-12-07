using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3PlayerController : MonoBehaviour
{
    [SerializeField] GameObject player;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && player.transform.position.y < 0.75)
        {
            player.transform.position += new Vector3(0f, 2f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && player.transform.position.y > -3.25)
        {
            player.transform.position += new Vector3(0f,-2f, 0f);
        }

            
    }
}
