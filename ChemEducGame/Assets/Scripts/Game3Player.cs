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
        }
        if (other.gameObject.tag == "Dummy")
        {
            game3Manager.DummyHit();
        }
          
    }
}
