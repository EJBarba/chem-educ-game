using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayClick : MonoBehaviour {

    private new Collider collider;
    public Camera cam;
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cam.enabled)
        {
            if (Input.GetMouseButtonDown(0))
            {

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (collider.Raycast(ray, out hit, 100))
                {
                    this.GetComponent<DaiMangou.BridgedData.ClickListener>().SwitchRoute();
                }
            }
        }
    }
}
