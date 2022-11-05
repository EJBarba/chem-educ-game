using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform firePoint;
    public Transform ironSightPoint;
    public LineRenderer lineRenderer;
    public ArcheryManager archeryManager;
    public LineRenderer lineRendererIronSight;
    private int laserHit = 0;
    
    private void Start() {
        lineRendererIronSight.SetPosition(0, ironSightPoint.position);
    }
    void Update()
    {
        RaycastHit2D hitInfoIronSight = Physics2D.Raycast(ironSightPoint.position, ironSightPoint.right);

        if (Input.GetKeyUp("space"))
        {
            lineRenderer.enabled = false;
        }
        if (Input.GetKey("space"))
        {
            StartCoroutine(FireLaser());
        }

        if (hitInfoIronSight)
        {
          lineRendererIronSight.SetPosition(1, hitInfoIronSight.point);  
        }

        if(!hitInfoIronSight)
        {
            lineRendererIronSight.SetPosition(1, ironSightPoint.position + firePoint.right * 100);
        }
        
        
    }
    public IEnumerator FireLaser()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);

        if (hitInfo)
        {
            lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, hitInfo.point);

            //archeryManager.isLaserFired = true;
            if(hitInfo.transform.tag == "Target")
            {
                //archeryManager.isTargetHit = true;
                laserHit += 1;
                Debug.Log(laserHit);
                
            }
            else if (hitInfo.transform.tag == "Dummy")
            {
                //archeryManager.isTargetHit = false;
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
    }
}
