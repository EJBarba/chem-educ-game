using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3Player : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Target")
        {
            Destroy(other.gameObject);
            // add score
        }
        if (other.gameObject.tag == "Dummy")
        {
            // reduce lives or end game here
        }
          
    }
}
