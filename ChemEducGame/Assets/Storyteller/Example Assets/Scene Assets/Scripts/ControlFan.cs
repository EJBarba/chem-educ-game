using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlFan : MonoBehaviour {

    public Slider TheSlider;
    public Text TheText;
    public RotateThisObject target;
    // Use this for initialization
    private void Start()
    {
        TheSlider.value = target.z_Rotation;
    }

    void Update () {

        target.z_Rotation = TheSlider.value * 2;
        TheText.text = target.z_Rotation.ToString();
	}
}
