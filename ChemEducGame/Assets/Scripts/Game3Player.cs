using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3Player : MonoBehaviour
{
    [SerializeField] Game3Manager game3Manager;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Target")
        {
            Destroy(other.gameObject);
            game3Manager.TargetHit(other.gameObject.name);
            
            // add score
        }
        if (other.gameObject.tag == "Dummy")
        {
            // reduce lives or end game here
        }
          
    }
}
