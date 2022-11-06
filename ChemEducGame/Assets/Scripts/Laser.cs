using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform firePoint;
    public LineRenderer lineRendererMainLaser;
    public LineRenderer lineRendererIronSight;
    private Animator targetAnimator;
    private RaycastHit2D hitInfo;
    private RaycastHit2D lastHitInfo;
    private RaycastHit2D hitInfoCheck;
    private Ray2D ray;
    void Update()
    {
        FireLaser(firePoint, lineRendererIronSight, false);
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
            lineRendererMainLaser.enabled = false;
            if (targetAnimator != null)
            {
                targetAnimator.SetBool("TakeDamage", false);
            }
        }
        if (Input.GetKey("space"))
        {
            FireLaser(firePoint, lineRendererMainLaser, true);
        }
    }
    public void FireLaser(Transform pointOfOrigin, LineRenderer lineRenderer, bool mainLaser)
    {
        ray = new Ray2D(pointOfOrigin.position, pointOfOrigin.right);

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        float remainingLength = 100;

        for (int i = 0; i < 10; i++)
        {
            hitInfo = Physics2D.Raycast(ray.origin, ray.direction, remainingLength);

            if (hitInfo)
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hitInfo.point);
                remainingLength -= Vector2.Distance(ray.origin, hitInfo.point);

                // Get the reflected vector of the raycast.
                Vector2 updatedDirection = Vector2.Reflect(ray.direction, hitInfo.normal);
                // Create new Ray object & set origin to be 0.01f away from hitpoint so the line is not colliding with the gameobject collider.
                ray = new Ray2D(hitInfo.point + updatedDirection * 0.01f, updatedDirection);
                
                if (hitInfo.transform.tag == "Target" || hitInfo.transform.tag == "Dummy")
                {
                    if (mainLaser)
                    {
                        hitInfo.transform.GetComponent<DummyHealth>().TakeDamage(1);
                        targetAnimator = hitInfo.transform.GetComponent<DummyHealth>().animator;
                        targetAnimator.SetBool("TakeDamage", true); 
                    }
                    break;
                }
            }
            else
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLength);
            }
        }
        lineRenderer.enabled = true;
    }
}