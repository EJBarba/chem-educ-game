using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform firePoint;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);

        if (hitInfo)
        {
            Debug.Log(hitInfo.transform.name);
        }
    }
}
