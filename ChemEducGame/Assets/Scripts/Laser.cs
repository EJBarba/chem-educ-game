using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform firePoint;
    public LineRenderer lineRenderer;
    public ArcheryManager archeryManager;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(FireLaser());
        }
    }
    public IEnumerator FireLaser()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);

        if (hitInfo)
        {
            lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, hitInfo.point);

            archeryManager.isLaserFired = true;
            if(hitInfo.transform.tag == "Target")
            {
                archeryManager.isTargetHit = true;
            }
            else if (hitInfo.transform.tag == "Dummy")
            {
                archeryManager.isTargetHit = false;
            }
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, firePoint.position + firePoint.right * 100);
            archeryManager.isLaserFired = false;
            archeryManager.isTargetHit = false;

        }
        
        lineRenderer.enabled = true;
		yield return new WaitForSeconds(0.02f);
		lineRenderer.enabled = false;
    }
}
