using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchedIDCard : MonoBehaviour
{
    [UnityEngine.Serialization.FormerlySerializedAs("tag")]
    public string TargetTag ="";
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == TargetTag)
        {
            gameObject.GetComponent<ExampleConditions2>().HasIDCard = true;
        }
    }
}
