using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform firePoint;
    public Transform ironSightPoint;
    public LineRenderer lineRenderer;
    public LineRenderer lineRendererIronSight;
    private Animator targetAnimator;
    private RaycastHit2D hitInfo;
    private RaycastHit2D hitInfoCheck;
    private void Start() {
        lineRendererIronSight.SetPosition(0, ironSightPoint.position);
    }
    void Update()
    {
        RaycastHit2D hitInfoIronSight = Physics2D.Raycast(ironSightPoint.position, ironSightPoint.right);

        if (Input.GetKey(KeyCode.UpArrow) && this.transform.rotation.z <= 0.3f)
        {
            this.transform.Rotate(0f,0f,0.1f);
            if (Input.GetKey("space"))
            {
                hitInfoCheck = Physics2D.Raycast(firePoint.position, firePoint.right);
                if (targetAnimator!= null && hitInfoCheck.transform != null && hitInfo.transform != null  && hitInfo.transform != hitInfoCheck.transform)
                {
                    targetAnimator.SetBool("TakeDamage", false);
                }
            } 
        }

        if (Input.GetKey(KeyCode.DownArrow) && this.transform.rotation.z >= -0.3f )
        { 
            this.transform.Rotate(0f,0f, -0.1f);
            if (Input.GetKey("space"))
            {
                hitInfoCheck = Physics2D.Raycast(firePoint.position, firePoint.right);
                if (targetAnimator!= null && hitInfoCheck.transform != null && hitInfo.transform != null  && hitInfo.transform != hitInfoCheck.transform)
                {
                    targetAnimator.SetBool("TakeDamage", false);
                }
            }    
        }
        
        if (Input.GetKeyUp("space"))
        {
            lineRenderer.enabled = false;
            if (targetAnimator != null)
            {
                targetAnimator.SetBool("TakeDamage", false);
            }
            
        }
        if (Input.GetKey("space"))
        {
            FireLaser();
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
    public void FireLaser()
    {
        hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);

        if (hitInfo)
        {
            lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, hitInfo.point);

            if(hitInfo.transform.tag == "Target" || hitInfo.transform.tag == "Dummy")
            {
                hitInfo.transform.GetComponent<DummyHealth>().TakeDamage(1);
                targetAnimator = hitInfo.transform.GetComponent<DummyHealth>().animator;
                targetAnimator.SetBool("TakeDamage", true);
            }
            else
            {
               if (targetAnimator)
            {
                targetAnimator.SetBool("TakeDamage", false);
            }
            }
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, firePoint.position + firePoint.right * 100);
            if (targetAnimator)
            {
                targetAnimator.SetBool("TakeDamage", false);
            }
            
        }
        lineRenderer.enabled = true;
    }
}
