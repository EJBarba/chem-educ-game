using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    
    public ArcheryManager archeryManager;
    private void Start() {
        archeryManager = GameObject.Find("PlayerManager").GetComponent<ArcheryManager>();
    }
    public bool collided;

    public void Release()
    {
        PathPoints.instance.Clear();
        StartCoroutine(CreatePathPoints());
    }

    IEnumerator CreatePathPoints()
    {
        while (true)
        {
            if (collided) break;
            PathPoints.instance.CreateCurrentPathPoint(transform.position);
            yield return new WaitForSeconds(PathPoints.instance.timeInterval);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collided = true;
        Debug.Log(collision.gameObject.tag);

        // if collision.gameObject.tag == "Dummy" -> lose
        if (collision.gameObject.tag == "Dummy")
        {
            //archeryManager.birdCollided = true;
            //archeryManager.isTargetCollided = false;
            return;
        }
        if (collision.gameObject.tag == "Target")
        {
            //archeryManager.birdCollided = true;
            //archeryManager.isTargetCollided = true;
            return;
        }
        // if collision.gameObject.tag == "Target" -> win
    }
}
